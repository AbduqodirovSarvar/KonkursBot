using KonkursBot.Db.Entities;
using KonkursBot.Interfaces;
using KonkursBot.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using static System.Net.WebRequestMethods;

namespace KonkursBot.Services
{
    public class MainMenuServiceHandler(
        ITelegramBotClient client,
        IAppDbContext appDbContext
        )
    {
        private readonly ITelegramBotClient _client = client;
        private readonly IAppDbContext _context = appDbContext;
        public async Task ShowMainMenu(Message message, Db.Entities.User user, CancellationToken cancellationToken)
        {
            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Kerakli bo'limni tanlang!",
                replyMarkup: KeyboardService.CreateReplyKeyboardMarkup(Buttons.MainMenuList, 2),
                cancellationToken: cancellationToken
                );
            return;
        }

        public async Task ClickCompitionButton(Message message, Db.Entities.User user, CancellationToken cancellationToken) 
        {
            await _client.SendTextMessageAsync(
            chatId: message.Chat.Id,
                text: $"Link: https://t.me/uzdevelop_bot?start={message.Chat.Id}",
                replyMarkup: KeyboardService.CreateReplyKeyboardMarkup(Buttons.MainMenuList, 2),
                cancellationToken: cancellationToken
                );
            return;
        }

        public async Task ClickGiftButton(Message message, Db.Entities.User user, CancellationToken cancellationToken)
        {
            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Sovga chiqadi",
                replyMarkup: KeyboardService.CreateReplyKeyboardMarkup(Buttons.MainMenuList, 2),
                cancellationToken: cancellationToken
                );
            return;
        }

        public async Task ClickScoreButton(Message message, Db.Entities.User user, CancellationToken cancellationToken)
        {
            var referals = await _context.Users.Where(x => x.ParentId == message.Chat.Id).ToListAsync();
            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Referallar soni: {referals.Count}",
                replyMarkup: KeyboardService.CreateReplyKeyboardMarkup(Buttons.MainMenuList, 2),
                cancellationToken: cancellationToken
                );
            return;
        }

        public async Task ClickConditionButton(Message message, Db.Entities.User user, CancellationToken cancellationToken)
        {
            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Shartlar chiqadi",
                replyMarkup: KeyboardService.CreateReplyKeyboardMarkup(Buttons.MainMenuList, 2),
                cancellationToken: cancellationToken
                );
            return;
        }
    }
}
