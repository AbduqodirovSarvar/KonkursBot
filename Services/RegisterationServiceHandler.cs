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
    public class RegisterationServiceHandler(
        ITelegramBotClient client, 
        IAppDbContext appDbContext,
        StateService stateService,
        MainMenuServiceHandler mainMenu
        )
    {
        private readonly ITelegramBotClient _client = client;
        private readonly IAppDbContext _context = appDbContext;
        private readonly StateService _state = stateService;
        private readonly MainMenuServiceHandler _mainMenu = mainMenu;

        private static readonly Db.Entities.User? RegisterUser = null;
        public async Task ClickStartButton(Message message, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, cancellationToken);
            if (user != null)
            {
                await _mainMenu.ShowMainMenu(message, user, cancellationToken);
                return;
            }

            RegisterUser = new Db.Entities.User()
            {
                TelegramId = message.Chat.Id
            };

            if (message.Text!.StartsWith("/start"))
            {
                Match match = Regex.Match(message.Text, @"\d+");

                if (match.Success)
                {
                    long id = long.Parse(match.Value);
                    var refuser = await _context.Users.FirstOrDefaultAsync(x => x.TelegramId == id, cancellationToken);
                    if (refuser != null)
                    {
                        RegisterUser
                        /*await _context.Users.AddAsync(new Db.Entities.User()
                        {
                            TelegramId = message.Chat.Id,
                            ParentId = refuser.TelegramId,
                            UserName = message.From?.Username
                        }, cancellationToken);*/
                    }
                }
            }
         /*   else
            {
                await _context.Users.AddAsync(new Db.Entities.User()
                {
                    TelegramId = message.Chat.Id,
                    UserName = message.From?.Username
                }, cancellationToken);
            }*/

            await _context.SaveChangesAsync(cancellationToken);

            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Assalomu aleykum, botimizga hush kelibsiz, foydalanish uchun tilni tanlang!\n" +
                "Hello, welcome to our bot, choose a language to use!\n" +
                "Здравствуйте, добро пожаловать к нашему боту, выберите язык для использования!",
                replyMarkup: KeyboardService.CreateReplyKeyboardMarkup([.. Buttons.Languages], 2),
                cancellationToken: cancellationToken
                );

            await _state.SetUserState(message.Chat.Id, StateList.register_choose_language);
            return;
        }

        public async Task ReceivedLanguage(Message message, CancellationToken cancellationToken)
        {
            var language = message.Text switch
            {
                "uz" => LanguageCode.uz,
                "en" => LanguageCode.en,
                "ru" => LanguageCode.ru,
                _ => LanguageCode.uz
            };
            var user = await _context.Users.FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, cancellationToken);
            if (user == null)
            {
                return;
            }

            user.LanguageCode = language;
            await _context.SaveChangesAsync(cancellationToken);

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
        }

        public async Task ReceivedFullName(Message message, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, cancellationToken);
            if (user == null)
            {
                return;
            }
            var text = user.LanguageCode switch
            {
                LanguageCode.uz => "Telefon raqamingizni yuboring!",
                LanguageCode.en => "Send your phone number!",
                LanguageCode.ru => "Отправьте свой номер телефона!",
                _ => "Telefon raqamingizni yuboring!"
            };
            user.FullName = message.Text;
            await _context.SaveChangesAsync(cancellationToken: cancellationToken);
            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: text,
                replyMarkup: KeyboardService.CreateContactRequestKeyboardMarkup(Buttons.ShareContact[(int)user.LanguageCode]),
                cancellationToken: cancellationToken
                );
            await _state.SetUserState(message.Chat.Id, StateList.register_get_contact);
            return;
        }

        public async Task ReceivedContact(Message message, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, cancellationToken);
            if (user == null)
            {
                return;
            }
            if(message.Contact?.PhoneNumber == null)
            {
                return;
            }
            user.PhoneNumber = message.Contact?.PhoneNumber ?? message.Text;
            await _context.SaveChangesAsync( cancellationToken: cancellationToken);
            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: Buttons.Congrats[(int)user.LanguageCode],
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
            await _state.DeleteState(message.Chat.Id);
            return;
        }
    }
}
