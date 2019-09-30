using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using THONK.Configuration;
using THONK.utils;

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
                builder.WithDescription($"{HelperFunctions.UserIdentity(before)} updated their nickname");
                builder.WithCurrentTimestamp();
                builder.AddField("Before", string.IsNullOrEmpty(before.Nickname)?"no nickname":before.Nickname);
                builder.AddField("After", string.IsNullOrEmpty(after.Nickname)?"no nickname":after.Nickname);
                await channel.SendMessageAsync("",false,builder.Build());
            }
        }

        /* TODO
         * test that function
         * 
         * idfk should work now still can't test it
         */
        async Task UserUpdated(SocketUser before, SocketUser after){
            foreach(var guild in _client.Guilds){
                var collection = guild.Users.Where(x=>x.Id==before.Id);
                if(!collection.Any())continue;
                SocketGuildUser guildUser = collection.First();
                if(string.IsNullOrEmpty(guildUser.Nickname)){
                    await guildUser.ModifyAsync(x=> x.Nickname=before.Username);
                }
                
                if(_config[guild.Id].BotLogChannel==null)continue;
                var channel = _config[guild.Id].BotLogChannel;
                var builder = new EmbedBuilder();
                builder.WithColor(Color.LightOrange);
                builder.WithDescription($"{HelperFunctions.NicknameOrUsername(before as SocketGuildUser)} updated their username");
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