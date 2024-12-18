namespace NotificationService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMqConsumer _consumer;

        public Worker(ILogger<Worker> logger, RabbitMqConsumer consumer)
        {
            _logger = logger;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationService Worker started at: {time}", DateTimeOffset.Now);

            try
            {
                await _consumer.StartListeningAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while running the RabbitMqConsumer.");
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationService Worker is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}

