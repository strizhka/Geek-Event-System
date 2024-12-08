using Microsoft.Extensions.Caching.Memory;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

public class RabbitMqConsumer
{
    private readonly string _hostName = "localhost";
    private readonly string _exchangeName = "token_validation";
    private readonly string _queueName = "community_queue";

    private static readonly MemoryCache TokenCache = new(new MemoryCacheOptions());

    public async Task StartListeningAsync()
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Fanout, durable: false);

        await channel.QueueDeclareAsync(_queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        await channel.QueueBindAsync(_queueName, _exchangeName, routingKey: "");

        Console.WriteLine(" [*] Waiting for validation results.");

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received validation result: {message}");

                await HandleMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [!] Error processing message: {ex.Message}");
            }
        };

        await channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);
        await Task.Delay(Timeout.Infinite);
    }

    private Task HandleMessageAsync(string message)
    {
        var parts = message.Split(", ");
        var token = parts[0].Split(":")[1];
        var isValid = parts[1].Split(":")[1] == "True";

        TokenCache.Set(token, isValid, TimeSpan.FromMinutes(15));
        Console.WriteLine($"Token '{token}' validation updated: {isValid}");

        return Task.CompletedTask;
    }

    public static bool IsTokenValid(string token)
    {
        return TokenCache.TryGetValue(token, out bool isValid) && isValid;
    }
}

