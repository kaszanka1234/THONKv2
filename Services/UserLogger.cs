using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using THONK.Configuration;

namespace THONK.Services{
    public class UserLogger{

        DiscordSocketClient _client {get;}
        Config _config {get;}

        async Task GuildMemberUpdated(SocketGuildUser before, SocketGuildUser after){
            if(_config[before.Guild.Id].BotLogChannel==null) return;
            var channel = _config[before.Guild.Id].BotLogChannel;
            if(before.Nickname!=after.Nickname){
                await channel.SendMessageAsync("a");
            }
        }

        public UserLogger(Config config, DiscordSocketClient client){
            _config = config;
            _client = client;
            _client.GuildMemberUpdated += GuildMemberUpdated;
        }
    }
}