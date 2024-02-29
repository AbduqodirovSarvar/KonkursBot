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
        MainMenuServiceHandler mainMenu,
        StateService stateService
        )
    {
        private readonly ILogger<UpdateHandlers> _logger = logger;
        private readonly IAppDbContext _context = appDbContext;
        private readonly RegisterationServiceHandler _registeration = registeration;
        private readonly MainMenuServiceHandler _mainMenu = mainMenu;
        private readonly StateService _stateService = stateService;

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
                _ => UnknownUpdateHandlerAsync(update)
            };

            await handler;
        }

        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            var state = _stateService.GetUserState(message.Chat.Id);
            if (state == null)
            {
                var result = state switch
                {
                    StateList.register_choose_language => _registeration.ReceivedLanguage(message, cancellationToken),
                    StateList.register_get_fullname => _registeration.ReceivedFullName(message, cancellationToken),
                    StateList.register_get_contact => _registeration.ReceivedContact(message, cancellationToken),
                    _ => _registeration.ClickStartButton(message, cancellationToken)
                };
                await result;
                return;
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, cancellationToken);
            if(user == null)
            {
                await _registeration.ClickStartButton(message, cancellationToken);
                return;
            }

            var msg = message.Text switch
            {
                "Konkursda qatnashish" or "Participation in the competition" or "Участие в конкурсе" => _mainMenu.ClickCompitionButton(message, user, cancellationToken),
                "Sovg'alar" or "Gifts" or "Подарки" => _mainMenu.ClickGiftButton(message, user, cancellationToken),
                "Ballarim" or "My scores" or "Мои результаты" => _mainMenu.ClickScoreButton(message, user, cancellationToken),
                "Reyting" or "Rating" or "Рейтинг" => _mainMenu.ClickRatingButton(message, user, cancellationToken),
                "Shartlar" or "Conditions" or "Условия" => _mainMenu.ClickConditionButton(message, user, cancellationToken),
                _ => _mainMenu.ShowMainMenu(message, user, cancellationToken)
            };
            await msg;

            return;
        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            _logger.LogInformation("Unknown message received");
            return Task.CompletedTask;
        }
    }
}
