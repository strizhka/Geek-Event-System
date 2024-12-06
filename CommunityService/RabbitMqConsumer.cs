using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

public class RabbitMqConsumer
{
    private readonly string _hostName = "localhost";
    private readonly string _exchangeName = "token_validation";
    private readonly string _queueName = "tokens";

    public async Task StartListeningAsync()
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: _queueName);

        await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Fanout);

        await channel.QueueBindAsync(queue: _queueName, exchange: _exchangeName, routingKey: _queueName);

        Console.WriteLine(" [*] Waiting for validation results.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received validation result: {message}");

            await HandleMessageAsync(message);
        };

        await channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);
    }

    private Task HandleMessageAsync(string message)
    {
        var parts = message.Split(", ");
        var token = parts[0].Split(":")[1];
        var isValid = parts[1].Split(":")[1] == "True";

        if (isValid)
        {
            Console.WriteLine($"Token {token} is valid. Proceeding with API logic.");
        }
        else
        {
            Console.WriteLine($"Token {token} is invalid. Rejecting API call.");
        }

        return Task.CompletedTask;
    }
}

