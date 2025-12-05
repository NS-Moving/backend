using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace backend;

public class Warmup
{
    private readonly ILogger<Warmup> _logger;

    public Warmup(ILogger<Warmup> logger)
    {
        _logger = logger;
    }

    [Function("Warmup")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation($"EmailNotification: Received warmup request.");
        return new OkObjectResult("EmailNotifier warmed up.");
    }
}