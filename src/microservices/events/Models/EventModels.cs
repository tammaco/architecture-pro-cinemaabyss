namespace EventsApi.Models;
using System.Text.Json.Serialization;

public class MovieEvent
{
    [JsonPropertyName("movie_id")]
    public int MovieId { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;
    
    [JsonPropertyName("user_id")]
    public int? UserId { get; set; }
    
    [JsonPropertyName("rating")]
    public float? Rating { get; set; }
    
    [JsonPropertyName("genres")]
    public List<string>? Genres { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class UserEvent
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;
    
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}

public class PaymentEvent
{
    [JsonPropertyName("payment_id")]
    public int PaymentId { get; set; }
    
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    
    [JsonPropertyName("method_type")]
    public string? MethodType { get; set; }
}

public class EventResponse
{
    public string Status { get; set; } = "success";
    public int Partition { get; set; }
    public long Offset { get; set; }
    public object Event { get; set; } = null!;
}

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
}