namespace KonkursBot.Db.Entities
{
    public class User
    {
        public long TelegramId { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public long? ParentId { get; set; }
        public LanguageCode? LanguageCode { get; set; }
        public DateTime? CreatedTime { get; private set; } = DateTime.UtcNow;
    }

    public enum LanguageCode
    {
        uz,
        en,
        ru
    }
}
