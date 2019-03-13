using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace THONK.Services{
    public class Start{
        private readonly IServiceProvider _services;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;

        public Start(IServiceProvider services, DiscordSocketClient client, CommandService commands, IConfigurationRoot config){
            _services = services;
            _config = config;
            _client = client;
            _commands = commands;
        }

        public async Task StartAsync(){
            string token = Environment.GetEnvironmentVariable("TOKEN");
            if(string.IsNullOrWhiteSpace(token)){
                throw new Exception("Please load your bot's token into TOKEN environment variable");
            }
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
    }
}