using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Contracts
{
    public static class ConfigFactory
    {
        private static readonly IConfiguration _configuration;

        static ConfigFactory()
        {
            _configuration = new ConfigurationBuilder()
              .AddEnvironmentVariables()
              .Build();
        }

        public static RabbitConnectionConfig GetRabbitConnectionConfig()
        {
            return _configuration.Get<RabbitConnectionConfig>();
        }
    }
}
