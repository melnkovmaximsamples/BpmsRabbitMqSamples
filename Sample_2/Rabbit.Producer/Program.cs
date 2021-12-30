using Rabbit.Contracts;
using RabbitMQ.Client;
using System.Text;

var rabbitConnectionConfig = ConfigFactory.GetRabbitConnectionConfig();
var connectionFactory = new ConnectionFactory()
{
    HostName = rabbitConnectionConfig.RABBITMQ_HOST,
    Port = rabbitConnectionConfig.RABBITMQ_PORT,
    UserName = rabbitConnectionConfig.RABBITMQ_USERNAME,
    Password = rabbitConnectionConfig.RABBITMQ_PASSWORD
};

Console.WriteLine(rabbitConnectionConfig.RABBITMQ_HOST);

using (var connection = connectionFactory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(rabbitConnectionConfig.RABBITMQ_QUEUE, durable: false, exclusive: false, autoDelete: false, arguments: null);

    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;

    while (true)
    {
        var message = $"Привет из прошлого! Сообщение создано {DateTime.Now}";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(string.Empty, rabbitConnectionConfig.RABBITMQ_QUEUE, properties, body);

        await Task.Delay(TimeSpan.FromSeconds(3));
    }
}