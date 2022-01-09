using Rabbit.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var rabbitConnectionConfig = ConfigFactory.GetRabbitConnectionConfig();
var factory = new ConnectionFactory
{
    HostName = rabbitConnectionConfig.RABBITMQ_HOST,
    Port = rabbitConnectionConfig.RABBITMQ_PORT,
    UserName = rabbitConnectionConfig.RABBITMQ_USERNAME,
    Password = rabbitConnectionConfig.RABBITMQ_PASSWORD
};

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    var queueName = channel.QueueDeclare().QueueName;
    // настройка консамера, чтобы принимал сообщение только после обработки предыдущего
    //channel.BasicQos(0, 1, false);
    channel.ExchangeDeclare(rabbitConnectionConfig.RABBITMQ_EXCHANGE, ExchangeType.Fanout);
    channel.QueueBind(
        queue: queueName, 
        exchange: rabbitConnectionConfig.RABBITMQ_EXCHANGE, 
        routingKey: string.Empty);

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (s, e) =>
    {
        var body = e.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var delay = TimeSpan.FromSeconds(rabbitConnectionConfig.RABBITMQ_HANDLE_MESSAGE_SECONDS);

        Thread.Sleep(delay);

        Console.WriteLine($"[Получено] {message}");

        // отправляем уведомление об успешно принятом сообщении
        channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
    };

    // autoAck - флаг автоматического помечения сообщений, как отработанные
    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

    await Task.Delay(TimeSpan.FromDays(1));
}