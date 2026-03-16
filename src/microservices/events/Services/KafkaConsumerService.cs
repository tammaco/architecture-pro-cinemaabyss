using Confluent.Kafka;
using System.Text.Json;

namespace EventsService.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly string[] _topics;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger)
    {
        _logger = logger;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "kafka:9092",
            GroupId = "events-service-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

        _topics = new[] { "movie-events", "user-events", "payment-events" };
        _consumer.Subscribe(_topics);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer service started");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(100));

                    if (consumeResult != null)
                    {
                        var message = consumeResult.Message.Value;
                        var topic = consumeResult.Topic;
                        var partition = consumeResult.Partition;
                        var offset = consumeResult.Offset;

                            var jsonDoc = JsonDocument.Parse(message);
                            var formattedJson = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions
                            {
                                WriteIndented = true
                            });

                            _logger.LogInformation($"Consumed message in {topic} [partition: {partition}, offset: {offset}]");
                    }

                    await Task.Delay(10, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in Kafka consumer");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer service stopping");
        }
        finally
        {
            _consumer.Close();
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}