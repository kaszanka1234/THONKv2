using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using THONK.Services;
using THONK.Database;

namespace THONK {
    public class Bot {

        public IConfigurationRoot Configuration { get; }

        public Bot(string[] args) {

            var builder = new ConfigurationBuilder();
            Dictionary<string, string> config = new Dictionary<string, string>{
                {"prefix", "/" }
            };
            builder.AddInMemoryCollection(config);
            Configuration = builder.Build();
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

            await provider.GetRequiredService<Start>().StartAsync();
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
            .AddSingleton(Configuration)
        ;
    }
}
