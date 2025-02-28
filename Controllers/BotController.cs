using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBot_Api.Handlers;
using TelegramBot_Api.Configuration;
using TelegramBot_Api.Services;

namespace TelegramBot_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController : ControllerBase
{
    private readonly IOptions<BotConfiguration> _config;
    private readonly UpdateHandler _updateHandler;
    private readonly UserService _userService;

    public BotController(
        IOptions<BotConfiguration> config, 
        UpdateHandler updateHandler, 
        UserService userService)
    {
        _config = config;
        _updateHandler = updateHandler;
        _userService = userService;
    }

    [HttpGet("setWebhook")]
    public async Task<string> SetWebHook(
        [FromServices] ITelegramBotClient bot, 
        CancellationToken ct)
    {
        var webhookUrl = _config.Value.BotWebhookUrl.AbsoluteUri;
        
        await bot.SetWebhook(
            webhookUrl, 
            allowedUpdates: [], 
            secretToken: _config.Value.SecretToken, 
            cancellationToken: ct);
        
        return $"Webhook set to {webhookUrl}";
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] Update update, 
        [FromServices] ITelegramBotClient bot, 
        CancellationToken ct)
    {
        if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != _config.Value.SecretToken)
            return Forbid();

        if (update.Message?.From == null)
            return BadRequest("Invalid update format.");
       
        var user = update.Message.From;

        await _userService.AddUserOrUpdate(
            user.Id,
            user.Username ?? "unknown",
            user.FirstName ?? "",
            user.LastName ?? "");

        await _updateHandler.HandleUpdateAsync(bot, update, ct);

        return Ok();
    }
}
    