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

        // помечаем наши сообщения как постоянные - тоже необходимо для того, чтобы после перезапуска
        // RabbitMQ они не были удалены. Сохраняет его на диск (но есть небольшой промежуток, когда оно может быть потеряно)
        properties.Persistent = true;
        // durable - настраиваем очередь на сохранение сообщений после перезапуска RabbitMQ
        // ВАЖНО: если очередь уже создана без этого флага, то это не поможет
        channel.QueueDeclare(rabbitConnectionConfig.RABBITMQ_QUEUE, durable: true, exclusive: false, autoDelete: false, null);

        while (cancellationToken.IsCancellationRequested == false)
        {
            var message = $"Отправляем сообщение с текущей датой {DateTime.Now}";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(string.Empty, rabbitConnectionConfig.RABBITMQ_QUEUE, basicProperties: properties, body: body);

            //Console.WriteLine($"В очередь '{rabbitConnectionConfig.RABBITMQ_QUEUE}' отправлено сообщение: '{message}'");

            await Task.Delay(300);
        }
    }
}, cancellationToken);