using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

public class RabbitMqConsumer
{
    private readonly string _hostName = "localhost";
    private readonly string _exchangeName = "token_validation";

    public async Task StartListeningAsync()
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Fanout);

        var queueDeclareResult = await channel.QueueDeclareAsync();
        string queueName = queueDeclareResult.QueueName;

        await channel.QueueBindAsync(queue: queueName, exchange: _exchangeName, routingKey: string.Empty);

        Console.WriteLine(" [*] Waiting for validation results.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received validation result: {message}");

            await HandleMessageAsync(message);
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private Task HandleMessageAsync(string message)
    {
        // Пример обработки сообщения
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

