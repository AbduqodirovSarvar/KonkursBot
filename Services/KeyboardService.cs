using Telegram.Bot.Types.ReplyMarkups;

namespace KonkursBot.Services
{
    public static class KeyboardService
    {
        public static ReplyKeyboardMarkup CreateReplyKeyboardMarkup(List<string> names, int? rows = 2)
        {
            List<KeyboardButton[]> buttonRows = [];
            List<KeyboardButton> buttons = [];
            foreach (var name in names)
            {
                if (buttons.Count == rows)
                {
                    buttonRows.Add([.. buttons]);
                    buttons.Clear();
                }
                buttons.Add(new KeyboardButton(name.ToString()));
            }
            if (buttons.Count > 0)
            {
                buttonRows.Add([.. buttons]);
            }
            return new ReplyKeyboardMarkup(buttonRows.ToArray()) { ResizeKeyboard = true };
        }

        public static ReplyKeyboardMarkup CreateContactRequestKeyboardMarkup(string keyboardName)
        {
            var contactRequestKeyboardMarkup = new ReplyKeyboardMarkup(
                          new KeyboardButton(keyboardName) { RequestContact = true })
            { ResizeKeyboard = true };

            return contactRequestKeyboardMarkup;
        }
    }
}
