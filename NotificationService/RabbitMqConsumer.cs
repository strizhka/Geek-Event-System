using NotificationService.Managers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RabbitMqConsumer
{
    private readonly string _hostName = "localhost";
    private readonly string _exchangeName = "events_changes";
    private readonly string _queueName = "notification_queue";

    private readonly NotificationManager _notificationManager;
    private readonly ILogger<RabbitMqConsumer> _logger;

    public RabbitMqConsumer(NotificationManager notificationManager, ILogger<RabbitMqConsumer> logger)
    {
        _notificationManager = notificationManager;
        _logger = logger;
    }

    public async Task StartListeningAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Fanout, durable: false);

        await channel.QueueDeclareAsync(_queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        await channel.QueueBindAsync(_queueName, _exchangeName, routingKey: "");

        _logger.LogInformation(" [*] Waiting for event changes...");

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation(" [x] Received: {Message}", message);

                await HandleMessageAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " [!] Error processing message");
            }
        };

        await channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(100, stoppingToken);
        }

        _logger.LogInformation(" [*] Consumer stopped.");
    }

    private async Task HandleMessageAsync(string message, CancellationToken stoppingToken)
    {
        _logger.LogInformation("Processing event change: {Message}", message);

        //var parts = message.Split(", ");
        //var eventTitle = parts[0].Split(":")[1];
        //var changeDescription = parts[1].Split(":")[1];
        //string? email = parts[2].Split(":")[1];

        //var recipientEmail = email ?? "dashakoshelek@gmail.com";
        //var subject = $"Changes in #{eventTitle}";
        //var body = $"Event #{eventTitle} {changeDescription}";

        //await _notificationManager.SendEmailAsync(recipientEmail, subject, body);
    }
}


