using AuthService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();

builder.Services.AddScoped<IAuthManager, AuthManager>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));

builder.Services.Configure<TokenSettings>(
    builder.Configuration.GetSection("TokenSettings"));

// ����������� TokenManager
builder.Services.AddScoped<ITokenManager, TokenManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ������ ���������: Middleware ��� �������� ������ ������ ���� �� UseAuthorization()
app.UseMiddleware<TokenValidationMiddleware>();

// ������� ������ ����� UseAuthorization()
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

