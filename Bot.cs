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
    // Main bot class
    public class Bot {

        // Define root of configruation object containing configs for all guilds
        // it's used as array (config[]) and contains
        // - command prefix as Prefix, string
        // - General channel as GeneralChannel, SocketTextChannel
        // - Announcements channel as AnnouncementsChannel, -//-
        // - Bot Log channel as BotLogChannel, -//-
        // - Log channel as LogChannel, -//-
        public IConfig Configuration {get;set;}

        // Main constructor for Bot class, instantinates new config object
        public Bot(string[] args) {
            Configuration = new Config();
        }

        // Main static entry point for program
        public static async Task RunAsync(string[] args) {
            // Create new bot instance and run it
            var bot = new Bot(args);
            await bot.RunAsync();
        }

        // Main bot run method
        public async Task RunAsync() {
            // Create new service collection and populate it with services
            var services = new ServiceCollection();
            ConfigureServices(services);

            // Build new service provider and start all services
            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<Logging>();
            provider.GetRequiredService<CommandHandler>();
            provider.GetRequiredService<RPUpdater>();
            provider.GetRequiredService<MessageLogger>();
            provider.GetRequiredService<UserLogger>();
            provider.GetRequiredService<Greeter>();

            // Start the bot and log in
            await provider.GetRequiredService<Start>().StartAsync();

            // Load configuration from database
            provider.GetRequiredService<ConfigLoader>();

            // Wait indefinetly so the program won't end
            await Task.Delay(-1);
        }

        // Method that populates the services so they can be easily accessed by modules
        // services are passed as parameters to module constructor if matching parameters are found
        // only the services with matching parameters are added and rest is ignored
        private void ConfigureServices(ServiceCollection services) => services
            // add new discord client to service collection
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 100
            }))
            // Add command service to service collection
            .AddSingleton(new CommandService(new CommandServiceConfig {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            }))
            // Add the rest of respective services
            .AddSingleton<CommandHandler>()
            .AddSingleton<Logging>()
            .AddSingleton<Start>()
            .AddSingleton<RPUpdater>()
            .AddSingleton<ConfigLoader>()
            .AddSingleton<MessageLogger>()
            .AddSingleton<UserLogger>()
            .AddSingleton<Greeter>()
            // Add configuration object to the collection
            .AddSingleton(Configuration)
        ;
    }
}
