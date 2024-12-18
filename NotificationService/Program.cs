using Microsoft.EntityFrameworkCore;
using NotificationService;
using NotificationService.Data;
using NotificationService.Managers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<NotificationManager>();
builder.Services.AddSingleton<RabbitMqConsumer>();
builder.Services.AddHostedService<Worker>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(connectionString));

var host = builder.Build();
host.Run();
