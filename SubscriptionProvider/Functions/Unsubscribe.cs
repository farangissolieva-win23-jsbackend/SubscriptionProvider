using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SubscriptionProvider.Functions;

public class Unsubscribe(ILogger<Unsubscribe> logger, DataContext context)
{
    private readonly ILogger<Unsubscribe> _logger = logger;
	private readonly DataContext _context = context;

	[Function("Unsubscribe")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
		try
		{
			var body = await new StreamReader(req.Body).ReadToEndAsync();
			if (!string.IsNullOrEmpty(body))
			{
				var entity = JsonConvert.DeserializeObject<SubscribeEntity>(body);
				if (entity != null)
				{
					var existingEntity = await _context.Subscribers.FirstOrDefaultAsync(x => x.Email == entity.Email);
					if (existingEntity != null)
					{
						_context.Subscribers.Remove(existingEntity);
						await _context.SaveChangesAsync();
						return new OkObjectResult(new { Status = 200, Message = "Subscriber is unsubscribed!" });
					}
					else
					{
						return new NotFoundObjectResult(new { Status = 404, Message = "Subscriber is not subscribed yet!" });
					}
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError($": Unsubscribe.Run :: {ex.Message}");
		}
		return new BadRequestObjectResult(new { Status = 400, Message = "Unable to unsubscribe!" });
	}
}
