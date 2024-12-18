using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

public class RabbitMqPublisher
{
    private readonly string _hostName = "localhost";
    private readonly string _exchangeName = "events_changes";

    public async Task SendEventChangeAsync(string eventName, string changeDescription, string email)
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Fanout);

        string message = $"{eventName} {changeDescription}, {email}";
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: _exchangeName, routingKey: "", body: body);
        Console.WriteLine($"[x] Sent event change: {message}");
    }
}

