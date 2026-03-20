using EventsApi.Services;
using EventsService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<KafkaProducerService>();
builder.Services.AddHostedService<KafkaConsumerService>();

var app = builder.Build();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = true }));

var port = Environment.GetEnvironmentVariable("PORT") ?? "8082";
app.Run($"http://0.0.0.0:{port}");