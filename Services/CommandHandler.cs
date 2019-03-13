using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace THONK.Services{
    public class CommandHandler{
        private readonly IServiceProvider _services;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;

        public CommandHandler(IServiceProvider services, DiscordSocketClient client, CommandService commands, IConfigurationRoot config){
            _client = client;
            _commands = commands;
            _config = config;
            _services = services;
            _client.MessageReceived += MessageRecievedAsync;
        }

        private async Task MessageRecievedAsync(SocketMessage m){
            if (!(m is SocketUserMessage msg)) return;
            if (msg.Author.IsBot)return;
            if(msg.Author.IsWebhook)return;
            if(msg.Author.Id==_client.CurrentUser.Id)return;

            var context = new SocketCommandContext(_client, msg);
            int argPos=0;
            if(msg.HasStringPrefix("/", ref argPos)||msg.HasMentionPrefix(_client.CurrentUser, ref argPos)){
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if(!result.IsSuccess){
                    await context.Channel.SendMessageAsync(result.ToString());
                }
            }
        }
    }
}