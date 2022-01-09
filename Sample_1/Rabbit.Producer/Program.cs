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
        channel.QueueDeclare(rabbitConnectionConfig.RABBITMQ_QUEUE, durable: false, exclusive: false, autoDelete: false);

        while (cancellationToken.IsCancellationRequested == false)
        {
            var message = $"Отправляем сообщение с текущей датой {DateTime.Now}";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(string.Empty, rabbitConnectionConfig.RABBITMQ_QUEUE, body: body);

            Console.WriteLine($"[Отправлено] В очередь '{rabbitConnectionConfig.RABBITMQ_QUEUE}' отправлено сообщение: '{message}'");

            await Task.Delay(2000);
        }
    }
}, cancellationToken);