using Rabbit.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var rabbitConnectionConfig = ConfigFactory.GetRabbitConnectionConfig();
var connectionFactory = new ConnectionFactory()
{
    HostName = rabbitConnectionConfig.RABBITMQ_HOST,
    Port = rabbitConnectionConfig.RABBITMQ_PORT,
    UserName = rabbitConnectionConfig.RABBITMQ_USERNAME,
    Password = rabbitConnectionConfig.RABBITMQ_PASSWORD
};

using (var connection = connectionFactory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(rabbitConnectionConfig.RABBITMQ_QUEUE, durable: false, exclusive: false, autoDelete: false, arguments: null);
    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (s, e) =>
    {
        var body = e.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine(message);
    };

    channel.BasicConsume(queue: rabbitConnectionConfig.RABBITMQ_QUEUE, autoAck: true, consumer: consumer);

    await Task.Delay(-1);
}