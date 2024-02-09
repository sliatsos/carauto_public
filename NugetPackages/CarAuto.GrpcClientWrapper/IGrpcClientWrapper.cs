using Grpc.Core;

namespace CarAuto.GrpcClientWrapper
{
    public interface IGrpcClientWrapper
    {
        /// <summary>
        /// GRPC call wrapper for asynchronous methods. For example: GreeterService.SayHelloAsync().
        /// </summary>
        /// <typeparam name="TClient">Grpc specific client type of the parameter.</typeparam>
        /// <typeparam name="TInput">Input type of the parameter.</typeparam>
        /// <typeparam name="TResponse">Response type of the function.</typeparam>
        /// <param name="serviceAddress">service address uri.</param>
        /// <param name="parameters">Parameters for parsing to delegate.</param>
        /// <param name="methodName">The servive to call from MockServer. For example: SayHello.</param>
        /// <param name="headers">Metadata headers to send.</param>
        TResponse CallGrpcService<TClient, TInput, TResponse>(TInput parameters, string methodName, Metadata headers = null);
        
        /// <summary>
        /// GRPC call wrapper for asynchronous methods. For example: GreeterService.SayHelloAsync().
        /// </summary>
        /// <typeparam name="TClient">Grpc specific client type of the parameter.</typeparam>
        /// <typeparam name="TInput">Input type of the parameter.</typeparam>
        /// <typeparam name="TResponse">Response type of the function.</typeparam>
        /// <param name="serviceAddress">service address uri.</param>
        /// <param name="parameters">Parameters for parsing to delegate.</param>
        /// <param name="methodName">The servive to call from MockServer. For example: SayHello.</param>
        /// <param name="headers">Metadata headers to send.</param>
        Task<TResponse> CallGrpcServiceAsync<TClient, TInput, TResponse>(TInput parameters, string methodName, Metadata headers = null);
    }
}
