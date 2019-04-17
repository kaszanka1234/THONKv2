using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using THONK.Configuration;

namespace THONK.Services{

    //////////////////////////////
    // Probably works but idk   //
    // TODO                     //
    // plz test                 //
    //////////////////////////////

    public class Greeter{

        IConfig _config {get;}
        DiscordSocketClient _client {get;}

        // Method to execute after new user joins the server
        public async Task UserJoined(SocketGuildUser user){
            // send a message to general channel if it's configured
            if(_config[user.Guild.Id].GeneralChannel!=null){
                var channel = _config[user.Guild.Id].GeneralChannel;
                string greetMessage = $"Hewwo {user.Mention}! Welcome on **{user.Guild.Name}**";
                await channel.SendMessageAsync(greetMessage);
            }
            // Log new user if log is configured
            if(_config[user.Guild.Id].BotLogChannel!=null){
                var channel = _config[user.Guild.Id].BotLogChannel;
                var builder = new EmbedBuilder();
                builder.WithColor(Color.Blue);
                builder.WithCurrentTimestamp();
                builder.WithDescription($"{user.Mention} ({user.Id}) just joined");
                await channel.SendMessageAsync("",false,builder.Build());
            }
        }

        // Method to execute after user leaves the server
        public async Task UserLeft(SocketGuildUser user){
            // Send a message if general chanel is configured
            if(_config[user.Guild.Id].GeneralChannel!=null){
                var channel = _config[user.Guild.Id].GeneralChannel;
                string name = string.IsNullOrEmpty(user.Nickname)?user.Username:user.Nickname;
                string leaveMessage = $"**{name}** left\nPress F to pay respects";
                await channel.SendMessageAsync(leaveMessage);
            }
            // Log the incident if log is configured
            if(_config[user.Guild.Id].BotLogChannel!=null){
                var channel = _config[user.Guild.Id].BotLogChannel;
                var builder = new EmbedBuilder();
                builder.WithColor(Color.DarkRed);
                builder.WithCurrentTimestamp();
                string name = string.IsNullOrEmpty(user.Nickname)?user.Username:user.Nickname;
                builder.WithDescription($"{user.Mention}, **{name}** ({user.Id}) left");
                await channel.SendMessageAsync("",false,builder.Build());
            }
        }

        public Greeter(IConfig config, DiscordSocketClient client){
            _config = config;
            _client = client;
            _client.UserJoined += UserJoined;
            _client.UserLeft += UserLeft;
        }
    }
}