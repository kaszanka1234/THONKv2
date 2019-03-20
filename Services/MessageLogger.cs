using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using THONK.Configuration;

namespace THONK.Services{
    public class MessageLogger{
        DiscordSocketClient _client {get;}
        Config _config {get;}

        async Task MessageDeleted(Cacheable<IMessage,ulong> cached, ISocketMessageChannel messageChannel){
            if(_config[(messageChannel as SocketTextChannel).Guild.Id].LogChannel==null)return;
            var builder = new EmbedBuilder();
            builder.WithColor(Color.Red);
            var channel = _config[(messageChannel as SocketTextChannel).Guild.Id].LogChannel;
            builder.WithCurrentTimestamp();
            if(!cached.HasValue){
                //cannot retrieve message
                builder.WithDescription($"cannot retrieve message deleted in {(messageChannel as SocketTextChannel).Mention}");
            }else{
                IMessage msg = cached.Value;
                builder.WithAuthor(msg.Author);
                builder.WithTitle($"Message was deleted in {(messageChannel as SocketTextChannel).Mention}");
                builder.WithDescription(msg.Content);
            }
            await channel.SendMessageAsync("",false,builder.Build());
        }

        async Task MessageEdited(Cacheable<IMessage,ulong> cached, SocketMessage newMessage, ISocketMessageChannel messageChannel){
            if(_config[(messageChannel as SocketTextChannel).Guild.Id].LogChannel==null)return;
            var channel = _config[(messageChannel as SocketTextChannel).Guild.Id].LogChannel;
            var builder = new EmbedBuilder();
            builder.WithAuthor(newMessage.Author);
            builder.WithColor(Color.Orange);
            builder.WithCurrentTimestamp();
            if(!cached.HasValue){
                builder.WithDescription("Cannot get old message");
                builder.AddField("after",newMessage.Content);
            }else{
                IMessage msg = cached.Value;
                if(string.IsNullOrEmpty(msg.Content)) return;
                builder.WithTitle($"Message edited in {(messageChannel as SocketTextChannel).Mention}");
                builder.AddField("Before",msg.Content);
                builder.AddField("After",newMessage.Content);
            }
            await channel.SendMessageAsync("",false,builder.Build());
        }

        public MessageLogger(DiscordSocketClient client, Config config){
            _config = config;
            _client = client;
            _client.MessageDeleted += MessageDeleted;
            _client.MessageUpdated += MessageEdited;
        }
    }
}