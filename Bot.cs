using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using THONK.Configuration;
using THONK.Database;
using THONK.Services;

namespace THONK {
    public class Bot {
        
        public Config Configuration {get;set;}

        public Bot(string[] args) {
            Configuration = new Config();
            Configuration[488843604731887641].Prefix="/";
        }

        public static async Task RunAsync(string[] args) {
            var bot = new Bot(args);
            await bot.RunAsync();
        }

        public async Task RunAsync() {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<Logging>();
            provider.GetRequiredService<CommandHandler>();
            provider.GetRequiredService<RPUpdater>();
            provider.GetRequiredService<MessageLogger>();
            provider.GetRequiredService<UserLogger>();

            await provider.GetRequiredService<Start>().StartAsync();

            provider.GetRequiredService<ConfigLoader>().LoadAll();
            await Task.Delay(-1);
        }

        private void ConfigureServices(ServiceCollection services) => services
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 100
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            }))
            .AddSingleton<CommandHandler>()
            .AddSingleton<Logging>()
            .AddSingleton<Start>()
            .AddSingleton<RPUpdater>()
            .AddSingleton<ConfigLoader>()
            .AddSingleton<MessageLogger>()
            .AddSingleton<UserLogger>()
            .AddSingleton(Configuration)
        ;
    }
}
