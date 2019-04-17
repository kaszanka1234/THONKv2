using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using THONK.Configuration;

namespace THONK.Services{

    // TODO
    //
    // documentation
    // xD
    public class UserLogger{

        DiscordSocketClient _client {get;}
        IConfig _config {get;}

        async Task GuildMemberUpdated(SocketGuildUser before, SocketGuildUser after){
            if(_config[before.Guild.Id].BotLogChannel==null) return;
            var channel = _config[before.Guild.Id].BotLogChannel;
            if(before.Nickname!=after.Nickname){
                var builder = new EmbedBuilder();
                builder.WithColor(Color.LightOrange);
                builder.WithDescription($"{before.Mention} updated their nickname");
                builder.WithCurrentTimestamp();
                builder.AddField("Before", string.IsNullOrEmpty(before.Nickname)?"no nickname":before.Nickname);
                builder.AddField("After", string.IsNullOrEmpty(after.Nickname)?"no nickname":after.Nickname);
                await channel.SendMessageAsync("",false,builder.Build());
            }
        }

        /* TODO
         * test that function
         */
        async Task UserUpdated(SocketUser before, SocketUser after){
            if(before.Username!=after.Username)return;
            foreach(var guild in _client.Guilds){
                if(_config[guild.Id].BotLogChannel==null)continue;
                var channel = _config[guild.Id].BotLogChannel;
                SocketGuildUser guildUser = guild.Users.Where(x=>x.Id==before.Id).First();
                var builder = new EmbedBuilder();
                builder.WithColor(Color.LightOrange);
                builder.WithDescription($"{guildUser.Mention} ({guildUser.Id}) updated their username");
                builder.WithCurrentTimestamp();
                builder.AddField("Before", before.Username);
                builder.AddField("After", after.Username);
                await channel.SendMessageAsync("",false,builder.Build());
            }
        }

        public UserLogger(IConfig config, DiscordSocketClient client){
            _config = config;
            _client = client;
            _client.GuildMemberUpdated += GuildMemberUpdated;
            _client.UserUpdated += UserUpdated;
        }
    }
}