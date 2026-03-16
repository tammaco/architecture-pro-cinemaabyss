using System.Net;

var monolith = GetEnv("MONOLITH_URL", "http://monolith:8080");
var movies = GetEnv("MOVIES_SERVICE_URL", "http://movies-service:8081");
var events = GetEnv("EVENTS_SERVICE_URL", "http://events-service:8082");
var port = GetEnv("PORT", "8000");
var pct = int.Parse(GetEnv("MOVIES_MIGRATION_PERCENT", "0"));
var gradual = GetEnv("GRADUAL_MIGRATION", "false") == "true";

var rand = new Random();
var listener = new HttpListener();
listener.Prefixes.Add($"http://*:{port}/");
listener.Start();

while (true)
{
    var context = listener.GetContext();
    ThreadPool.QueueUserWorkItem(_ => HandleRequest(context));
}

void HandleRequest(HttpListenerContext context)
{
    try
    {
        var request = context.Request;
        var response = context.Response;
        var path = request.Url!.AbsolutePath;

        var target = path.StartsWith("/events") ? events :
                     path.StartsWith("/movies") && gradual && rand.Next(100) < pct ? movies :
                     monolith;

        using var client = new HttpClient();
        var url = target + path + request.Url.Query;
        var httpRequest = new HttpRequestMessage(new HttpMethod(request.HttpMethod), url);

        foreach (string key in request.Headers)
        {
            if (key != "Host" && key != "Content-Length")
            {
                httpRequest.Headers.TryAddWithoutValidation(key, request.Headers[key]);
            }
        }

        if (request.HasEntityBody)
        {
            using var reader = new StreamReader(request.InputStream, System.Text.Encoding.UTF8);
            var body = reader.ReadToEnd();
            httpRequest.Content = new StringContent(body, System.Text.Encoding.UTF8, request.ContentType ?? "text/plain");
        }

        var httpResponse = client.Send(httpRequest);

        response.StatusCode = (int)httpResponse.StatusCode;
        
        foreach (var header in httpResponse.Headers)
        {
            response.Headers[header.Key] = string.Join(",", header.Value);
        }

        using var stream = httpResponse.Content.ReadAsStream();
        stream.CopyTo(response.OutputStream);
    }
    catch (Exception ex)
    {
        var response = context.Response;
        response.StatusCode = 500;
        var errorBytes = System.Text.Encoding.UTF8.GetBytes($"{{\"error\":\"{ex.Message}\"}}");
        response.OutputStream.Write(errorBytes, 0, errorBytes.Length);
    }
    finally
    {
        context.Response.OutputStream.Close();
    }
}

static string GetEnv(string key, string defaultValue) => Environment.GetEnvironmentVariable(key) ?? defaultValue;