using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

public class RabbitMqProducer
{
    private readonly string _hostName = "localhost";
    private readonly string _exchangeName = "token_validation";

    public async Task SendValidationResultAsync(string token, bool isValid)
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Fanout);

        var message = $"Token:{token}, IsValid:{isValid}";
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: _exchangeName, routingKey: string.Empty, body: body);
        Console.WriteLine($"[x] Sent validation result: {message}");
    }
}

