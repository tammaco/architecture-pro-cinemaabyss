using Confluent.Kafka;
using System.Text.Json;

namespace EventsApi.Services
{
    public class KafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;

        public KafkaProducerService(ILogger<KafkaProducerService> logger)
        {
            _logger = logger;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "kafka:9092"
            };

            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        public async Task<(int Partition, long Offset)> PublishAsync(string topic, object eventData)
        {
            var json = JsonSerializer.Serialize(eventData);
            var result = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });

            _logger.LogInformation("Published to {Topic} [partition: {Partition}, offset: {Offset}]",
                topic, result.Partition, result.Offset);

            return (result.Partition.Value, result.Offset.Value);
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}
