using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using THONK.Configuration;

namespace THONK.Services{
    public class Start{
        private readonly IServiceProvider _services;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfig _config;

        public Start(IServiceProvider services, DiscordSocketClient client, CommandService commands, IConfig config){
            _services = services;
            _config = config;
            _client = client;
            _commands = commands;
        }

        // Main bot starting method
        public async Task StartAsync(){
            // Get token from environment variable
            string token = Environment.GetEnvironmentVariable("TOKEN");
            
            // throw an exception if no token is provided
            if(string.IsNullOrWhiteSpace(token)){
                throw new Exception("Please load your bot's token into TOKEN environment variable");
            }

            // Login to discord and start the client
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Load all command modules from subdirectories
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
    }
}