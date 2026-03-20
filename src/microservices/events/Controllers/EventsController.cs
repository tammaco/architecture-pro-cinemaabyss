using EventsApi.Models;
using EventsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventsService.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly KafkaProducerService _kafkaProducer;
    private readonly ILogger<EventsController> _logger;

    private const string MovieTopic = "movie-events";
    private const string UserTopic = "user-events";
    private const string PaymentTopic = "payment-events";

    public EventsController(KafkaProducerService kafkaProducer, ILogger<EventsController> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    // GET: api/events/health
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new { status = true });
    }

    // POST: api/events/movie
    [HttpPost("movie")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateMovieEvent([FromBody] MovieEvent movieEvent)
    {
        try
        {
            if (movieEvent == null || movieEvent.MovieId <= 0)
            {
                return BadRequest(new ErrorResponse { Error = "Invalid movie event data" });
            }

            var enrichedEvent = new
            {
                id = $"{MovieTopic}-{Guid.NewGuid():N}",
                type = "movie",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                payload = movieEvent
            };

            var (partition, offset) = await _kafkaProducer.PublishAsync(MovieTopic, enrichedEvent);

            _logger.LogInformation("Movie event published successfully to topic {Topic} [partition: {Partition}, offset: {Offset}]", 
                MovieTopic, partition, offset);

            var response = new EventResponse
            {
                Partition = partition,
                Offset = offset,
                Event = enrichedEvent
            };

            return CreatedAtAction(nameof(CreateMovieEvent), response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating movie event");
            return StatusCode(500, new ErrorResponse { Error = ex.Message });
        }
    }

    // POST: api/events/user
    [HttpPost("user")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUserEvent([FromBody] UserEvent userEvent)
    {
        try
        {
            if (userEvent == null || userEvent.UserId <= 0)
            {
                return BadRequest(new ErrorResponse { Error = "Invalid user event data" });
            }

            var enrichedEvent = new
            {
                id = $"{UserTopic}-{Guid.NewGuid():N}",
                type = "user",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                payload = userEvent
            };

            var (partition, offset) = await _kafkaProducer.PublishAsync(UserTopic, enrichedEvent);
            _logger.LogInformation("User event published successfully to topic {Topic} [partition: {Partition}, offset: {Offset}]", 
                UserTopic, partition, offset);

            var response = new EventResponse
            {
                Partition = partition,
                Offset = offset,
                Event = enrichedEvent
            };

            return CreatedAtAction(nameof(CreateUserEvent), response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user event");
            return StatusCode(500, new ErrorResponse { Error = ex.Message });
        }
    }

    // POST: api/events/payment
    [HttpPost("payment")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePaymentEvent([FromBody] PaymentEvent paymentEvent)
    {
        try
        {
            if (paymentEvent == null || paymentEvent.PaymentId <= 0)
            {
                return BadRequest(new ErrorResponse { Error = "Invalid payment event data" });
            }

            var enrichedEvent = new
            {
                id = $"{PaymentTopic}-{Guid.NewGuid():N}",
                type = "payment",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                payload = paymentEvent
            };

            var (partition, offset) = await _kafkaProducer.PublishAsync(PaymentTopic, enrichedEvent);
            _logger.LogInformation("Payment event published successfully to topic {Topic} [partition: {Partition}, offset: {Offset}]", 
                PaymentTopic, partition, offset);

            var response = new EventResponse
            {
                Partition = partition,
                Offset = offset,
                Event = enrichedEvent
            };

            return CreatedAtAction(nameof(CreatePaymentEvent), response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment event");
            return StatusCode(500, new ErrorResponse { Error = ex.Message });
        }
    }
}