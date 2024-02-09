using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CarAuto.Kafka
{
    /// <summary>
    /// Base class for creating a long running Kafka Background Consumer.
    /// </summary>
    /// <typeparam name="TKey">The type of the key of the Kafka message.</typeparam>
    /// <typeparam name="TValue">The type of the value of the Kafka message.</typeparam>
    /// <typeparam name="TDeserializer">The type of the value deserializer for the Kafka message.</typeparam>
    public abstract class BackgroundConsumer<TKey, TValue, TDeserializer> : BackgroundService,
        IBackgroundConsumer<TKey, TValue, TDeserializer>
        where TValue : class, new()
        where TDeserializer : IDeserializer<TValue>, new()
    {
        private const int MaxRetries = 5;
        private readonly ILogger<BackgroundConsumer<TKey, TValue, TDeserializer>> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private int _retry;

        protected BackgroundConsumer(
            ILogger<BackgroundConsumer<TKey, TValue, TDeserializer>> logger,
            IConfiguration configuration,
            IHostApplicationLifetime hostApplicationLifetime,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _hostApplicationLifetime = hostApplicationLifetime ??
                                       throw new ArgumentNullException(nameof(hostApplicationLifetime));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        /// <summary>
        /// Override the Topics property to set the name of the topic(s) the instance of the background consumer subscribes to.
        /// </summary>
        public abstract string Topics { get; }

        internal async Task InvokeMessageHandlingAsync(ConsumeResult<TKey, TValue> consumeResult)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                if (consumeResult?.Message?.Headers == null)
                {
                    throw new ArgumentNullException(nameof(consumeResult));
                }

                await Task.Run(async () =>
                {
                    await ProcessMessage(consumeResult, scope).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }

        [ExcludeFromCodeCoverage]
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => OuterConsumeLoop(stoppingToken), stoppingToken);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Is called when a message is received on a topic. Override the methods to specify the logic to be executed for a received message.
        /// </summary>
        /// <param name="consumeResult">The received message including Headers and other metadata.</param>
        /// <param name="scope">The current DI scope.</param>
        protected abstract Task ProcessMessage(ConsumeResult<TKey, TValue> consumeResult, IServiceScope scope);

        [ExcludeFromCodeCoverage]
        private void OuterConsumeLoop(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"BackgroundConsumer {GetType()} is starting");
            stoppingToken.Register(() => _logger.LogDebug($"BackgroundConsumer {GetType()} is stopping"));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    StartConsumer(stoppingToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (!stoppingToken.IsCancellationRequested)
                    {
                        if (_retry < MaxRetries)
                        {
                            _retry++;
                            _logger.LogError($"Error during ConsumeLoop execution. Retrying. {_retry}/{MaxRetries}", exception);
                            Thread.Sleep(5000 * (int)Math.Pow(2, _retry));
                        }
                        else
                        {
                            _logger.LogCritical(
                                $"BackgroundConsumer {GetType()} failed and max retries reached. Stopping the application.",
                                exception);
                            _hostApplicationLifetime.StopApplication();
                        }
                    }
                }
            }

            _logger.LogDebug($"BackgroundConsumer {GetType()} is stopping");
        }

        [ExcludeFromCodeCoverage]
        private async Task StartConsumer(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig();
            _configuration.Bind("Kafka", config);
            config.EnableAutoCommit = false;
            config.AllowAutoCreateTopics = false;
            if (string.IsNullOrEmpty(config.GroupId))
            {
                _logger.LogError($"BackgroundConsumer {GetType()}: GroupId cannot be null or empty.");
                throw new ArgumentNullException(nameof(config), "GroupId cannot be null or empty.");
            }

            var consumerBuilder = new ConsumerBuilder<TKey, TValue>(config);
            consumerBuilder.SetValueDeserializer(new TDeserializer());
            using (var consumer = consumerBuilder.Build())
            {
                consumer.Subscribe(Topics);
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(stoppingToken);
                        _retry = 0;
                        _logger.LogTrace($"Message consumed: {consumeResult}");
                        await InvokeMessageHandlingAsync(consumeResult).ConfigureAwait(false);
                        consumer.Commit(consumeResult);
                    }
                    catch (ConsumeException consumeException)
                    {
                        _logger.LogError(consumeException, consumeException.Message, null);
                    }
                    catch (OperationCanceledException operationCanceledException)
                    {
                        _logger.LogError(operationCanceledException, operationCanceledException.Message, null);
                        throw;
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, exception.Message, null);
                    }
                }
            }
        }
    }
}