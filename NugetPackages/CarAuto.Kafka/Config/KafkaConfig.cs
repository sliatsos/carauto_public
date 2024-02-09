using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CarAuto.Kafka.Config
{
    public class KafkaConfig
    {
        public KafkaConfig(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.GetSection("Kafka").Bind(this);
        }

        public string Topic { get; set; }
    }
}
