using KonkursBot.Db.Entities;
using KonkursBot.Interfaces;
using KonkursBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KonkursBot.Services
{
    public partial class RegisterationServiceHandler(
        ITelegramBotClient client, 
        IAppDbContext appDbContext,
        MainMenuServiceHandler mainMenu
        )
    {
        private readonly ITelegramBotClient _client = client;
        private readonly IAppDbContext _context = appDbContext;
        private readonly MainMenuServiceHandler _mainMenu = mainMenu;

        public async Task ClickStartButton(Message message, CancellationToken cancellationToken)
        {
            if (message.Text!.StartsWith("/start"))
            {
                Match match = MyRegex().Match(message.Text);

                if (match.Success)
                {
                    long id = long.Parse(match.Value);
                    var refuser = await _context.Users.FirstOrDefaultAsync(x => x.TelegramId == id, cancellationToken);
                    if (refuser != null)
                    {
                        await _context.Users.AddAsync(new Db.Entities.User()
                        {
                            TelegramId = message.Chat.Id,
                            ParentId = refuser.TelegramId,
                            UserName = message.From?.Username
                        },  cancellationToken);
                    }
                }
            }
            else
            {
                await _context.Users.AddAsync(new Db.Entities.User()
                {
                    TelegramId = message.Chat.Id,
                    UserName = message.From?.Username
                }, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Assalomu aleykum, botimizga hush kelibsiz!\nTo'liq ism-familyangizni yuboring!",
                cancellationToken: cancellationToken
                );
            await StateService.SetUserState(message.Chat.Id, StateList.register_get_fullname);
            return;
        }

       /* public async Task ReceivedLanguage(Message message, Db.Entities.User user, CancellationToken cancellationToken)
        {
            var language = message.Text switch
            {
                "uz" => LanguageCode.uz,
                "en" => LanguageCode.en,
                "ru" => LanguageCode.ru,
                _ => LanguageCode.uz
            };
            if(user.LanguageCode == null)
            {
                user.LanguageCode = language;
                await _context.SaveChangesAsync(cancellationToken);
            }

            var text = message.Text switch
            {
                "uz" => "To'liq ismingizni yuboring",
                "en" => "Submit your full name",
                "ru" => "Отправьте свое полное имя",
                _ => "To'liq ismingizni yuboring"
            };

            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: text,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);

            await _state.SetUserState(message.Chat.Id, StateList.register_get_fullname);
            return;
        }*/


        public async Task ReceivedFullName(Message message, Db.Entities.User user, CancellationToken cancellationToken)
        {
            user.FullName = message.Text;
            await _context.SaveChangesAsync(cancellationToken: cancellationToken);
            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Telefon raqamingizni yuboring!",
                replyMarkup: KeyboardService.CreateContactRequestKeyboardMarkup("Raqamni yuborish"),
                cancellationToken: cancellationToken
                );
            await StateService.SetUserState(message.Chat.Id, StateList.register_get_contact);
            return;
        }

        public async Task ReceivedContact(Message message, Db.Entities.User user, CancellationToken cancellationToken)
        {
            if(user.FullName == null)
            {
                await ClickStartButton(message, cancellationToken);
                return;
            }
            if (message.Contact?.PhoneNumber == null)
            {
                return;
            }
            user.PhoneNumber = message.Contact.PhoneNumber;
            await _context.SaveChangesAsync( cancellationToken: cancellationToken);
            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Tabriklaymiz, siz muvaffaqiyatli ro'yxatdan o'tdingiz!",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
            await StateService.DeleteState(message.Chat.Id);
            await _mainMenu.ShowMainMenu(message, user, cancellationToken);
            return;
        }

        [GeneratedRegex(@"\d+")]
        private static partial Regex MyRegex();
    }
}
