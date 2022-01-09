using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Contracts
{
    public class RabbitConnectionConfig
    {
        public string RABBITMQ_HOST { get; set; } = null!;
        public int RABBITMQ_PORT { get; set; }
        public string RABBITMQ_USERNAME { get; set; } = null!;
        public string RABBITMQ_PASSWORD { get; set; } = null!;
        public string? RABBITMQ_QUEUE { get; set; }
        public string RABBITMQ_ROUTE { get; set; } = null!;
        public string RABBITMQ_EXCHANGE { get; set; } = null!;
        public int RABBITMQ_HANDLE_MESSAGE_SECONDS { get; set; }
        public string? RABBITMQ_MESSAGE { get; set; }
    }
}
