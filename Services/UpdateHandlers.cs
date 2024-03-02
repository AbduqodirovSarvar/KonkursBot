using KonkursBot.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KonkursBot.Services
{
    public class UpdateHandlers(
        ILogger<UpdateHandlers> logger,
        RegisterationServiceHandler registeration,
        IAppDbContext appDbContext,
        MainMenuServiceHandler mainMenu
        )
    {
        private readonly ILogger<UpdateHandlers> _logger = logger;
        private readonly IAppDbContext _context = appDbContext;
        private readonly RegisterationServiceHandler _registeration = registeration;
        private readonly MainMenuServiceHandler _mainMenu = mainMenu;

        public Task HandleErrorAsync(Exception exception)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            var handler = update switch
            {
                { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                _ => UnknownUpdateHandlerAsync()
            };

            await handler;
        }

        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, cancellationToken);
            if (user == null)
            {
                await _registeration.ClickStartButton(message, cancellationToken);
                return;
            }
            if (user.FullName == null)
            {
                if(StateService.GetUserState(message.Chat.Id) == StateList.register_get_fullname)
                {
                    await _registeration.ReceivedFullName(message, user, cancellationToken);
                    return;
                }
                await _registeration.ClickStartButton(message, cancellationToken);
                return;
            }
            if(user.PhoneNumber == null)
            {
                if (StateService.GetUserState(message.Chat.Id) == StateList.register_get_contact)
                {
                    await _registeration.ReceivedContact(message, user, cancellationToken);
                    return;
                }
                await _registeration.ClickStartButton(message, cancellationToken);
                return;
            }

            var msg = message.Text switch
            {
                "Konkursda qatnashish" => _mainMenu.ClickCompitionButton(message, user, cancellationToken),
                "Sovg'alar" => _mainMenu.ClickGiftButton(message, user, cancellationToken),
                "Ballarim" => _mainMenu.ClickScoreButton(message, user, cancellationToken),
                "Shartlar" => _mainMenu.ClickConditionButton(message, user, cancellationToken),
                _ => _mainMenu.ShowMainMenu(message, user, cancellationToken)
            };
            await msg;
            return;
        }

        private Task UnknownUpdateHandlerAsync()
        {
            _logger.LogInformation("Unknown message received");
            return Task.CompletedTask;
        }
    }
}
