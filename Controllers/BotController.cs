using KonkursBot.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace KonkursBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update, [FromServices] UpdateHandlers handleUpdateService, CancellationToken cancellationToken)
        {
            try
            {
                //Console.WriteLine("Received Message");
                await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                await handleUpdateService.HandleErrorAsync(ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
