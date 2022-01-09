using Rabbit.Contracts;
using RabbitMQ.Client;
using System.Text;

var rabbitConnectionConfig = ConfigFactory.GetRabbitConnectionConfig();
var factory = new ConnectionFactory
{
    HostName = rabbitConnectionConfig.RABBITMQ_HOST,
    Port = rabbitConnectionConfig.RABBITMQ_PORT,
    UserName = rabbitConnectionConfig.RABBITMQ_USERNAME,
    Password = rabbitConnectionConfig.RABBITMQ_PASSWORD
};

var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;
await Task.Run(async () =>
{
    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
    {
        var properties = channel.CreateBasicProperties();

        properties.Persistent = true;
        channel.ExchangeDeclare(rabbitConnectionConfig.RABBITMQ_EXCHANGE, ExchangeType.Direct);
        
        while (cancellationToken.IsCancellationRequested == false)
        {
            var message = $"Отправляем сообщение с текущей датой {DateTime.Now}";
            var body = Encoding.UTF8.GetBytes(rabbitConnectionConfig.RABBITMQ_MESSAGE);

            channel.BasicPublish(
                exchange: rabbitConnectionConfig.RABBITMQ_EXCHANGE, 
                routingKey: rabbitConnectionConfig.RABBITMQ_ROUTE, 
                basicProperties: properties, 
                body: body);

            await Task.Delay(300);
        }
    }
}, cancellationToken);