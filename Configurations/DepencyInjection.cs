using KonkursBot.Db;
using KonkursBot.Interfaces;
using KonkursBot.Models;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using Telegram.Bot;
using KonkursBot.Services;

namespace KonkursBot.Configurations
{
    public static class DepencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var botConfigurationSection = configuration.GetSection(BotOptions.Configuration);
            services.Configure<BotOptions>(botConfigurationSection);
            var botConfiguration = botConfigurationSection.Get<BotOptions>();

            raw.SetProvider(imp: new SQLite3Provider_e_sqlite3());
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("SQLiteConnection"));
            });
            Batteries.Init();

            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddScoped<UpdateHandlers>();

            services.AddControllers().AddNewtonsoftJson();

            services.AddHttpClient("konkurs")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotOptions? botConfig = sp.GetRequiredService<IConfiguration>().GetSection(BotOptions.Configuration).Get<BotOptions>();
                    TelegramBotClientOptions options = new(botConfig!.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

            services.AddHostedService<ConfigureWebhook>();
            services.AddScoped<MainMenuServiceHandler>();
            services.AddScoped<RegisterationServiceHandler>();
            services.AddScoped<GetUserDataServices>();
            return services;
        }
    }
}
