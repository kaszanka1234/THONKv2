using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using THONK.Configuration;

namespace THONK.Services{
    public class MessageLogger{
        DiscordSocketClient _client {get;}
        IConfig _config {get;}

        // Method to execute after message is deleted
        async Task MessageDeletedAsync(Cacheable<IMessage,ulong> cached, ISocketMessageChannel messageChannel){
            // return if log channel is diabled
            if(_config[(messageChannel as SocketTextChannel).Guild.Id].LogChannel==null)return;

            // get the log channel from config
            var channel = _config[(messageChannel as SocketTextChannel).Guild.Id].LogChannel;
            
            // create new embed, add color and current time
            var builder = new EmbedBuilder();
            builder.WithColor(Color.Red);
            builder.WithCurrentTimestamp();

            // Try to get message from cache
            if(!cached.HasValue){
                // Message is not in the cache
                // cannot retrieve message
                builder.WithDescription($"cannot retrieve message deleted in {(messageChannel as SocketTextChannel).Mention}");
            }else{
                // Get message contetns
                IMessage msg = cached.Value;
                // Ignore messages that are commands
                if(false){
                    // TODO
                }
                // Append autor name and message contents to embed
                builder.WithAuthor(msg.Author);
                builder.WithTitle($"Message was deleted in {(messageChannel as SocketTextChannel).Mention}");
                builder.WithDescription(msg.Content);
            }
            // Build and send the embed
            await channel.SendMessageAsync("",false,builder.Build());
        }

        // Method executed after message is edited
        async Task MessageEditedAsync(Cacheable<IMessage,ulong> cached, SocketMessage newMessage, ISocketMessageChannel messageChannel){
            // return if log is not configured
            if(_config[(messageChannel as SocketTextChannel).Guild.Id].LogChannel==null)return;
            // get log channel from config
            var channel = _config[(messageChannel as SocketTextChannel).Guild.Id].LogChannel;

            // create new embed, add color and current time
            var builder = new EmbedBuilder();
            builder.WithColor(Color.Orange);
            builder.WithCurrentTimestamp();
            
            // Try to get old message contetns from cache
            if(!cached.HasValue){
                // mesasge is not in the cahce
                // cannot retrieve message
                builder.WithDescription("Cannot get old message");
                // add only new message content
                builder.AddField("after",newMessage.Content);
            }else{
                // Get old message contents
                IMessage msg = cached.Value;
                // Ignore if the message is empty
                if(string.IsNullOrEmpty(msg.Content)) return;

                // Append message author and what was edited to embed
                builder.WithAuthor(newMessage.Author);
                builder.WithTitle($"Message edited in {(messageChannel as SocketTextChannel).Mention}");
                builder.AddField("Before",msg.Content);
                builder.AddField("After",newMessage.Content);
            }
            // build and send the embed
            await channel.SendMessageAsync("",false,builder.Build());
        }

        public MessageLogger(DiscordSocketClient client, IConfig config){
            _config = config;
            _client = client;
            _client.MessageDeleted += MessageDeletedAsync;
            _client.MessageUpdated += MessageEditedAsync;
        }
    }
}