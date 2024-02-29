using KonkursBot.Db.Entities;
using KonkursBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KonkursBot.Services
{
    public class MainMenuServiceHandler(
        ITelegramBotClient client,
        StateService stateService
        )
    {
        private readonly ITelegramBotClient _client = client;
        private readonly StateService _state = stateService;
        public async Task ShowMainMenu(Message message, Db.Entities.User user, CancellationToken cancellationToken)
        {
            List<string> list = Enumerable.Range(0, Buttons.MainMenu.GetLength(1)).Select(i => Buttons.MainMenu[(int)user.LanguageCode, i]).Cast<string>().ToList();
            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "",
                replyMarkup: KeyboardService.CreateReplyKeyboardMarkup(list, 2),
                cancellationToken: cancellationToken
                );
            await _state.DeleteState(message.Chat.Id);
            return;
        }

        public Task ClickCompitionButton(Message message, Db.Entities.User user, CancellationToken cancellationToken) 
        {
            return Task.CompletedTask;
        }

        public Task ClickGiftButton(Message message, Db.Entities.User user, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task ClickScoreButton(Message message, Db.Entities.User user, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task ClickConditionButton(Message message, Db.Entities.User user, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task ClickRatingButton(Message message, Db.Entities.User user, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
