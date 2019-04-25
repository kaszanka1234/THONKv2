using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
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
                int pos = 0;
                if((msg as SocketUserMessage).HasStringPrefix(_config[(messageChannel as SocketGuildChannel).Guild.Id].Prefix,ref pos)){
                    // still TODO
                    return;
                }
                // Ignore if author is self
                if(msg.Author==_client.CurrentUser){
                    return;
                }
                // Append autor name and message contents to embed
                builder.WithAuthor(msg.Author);
                builder.WithDescription($"**Message was deleted in {(messageChannel as SocketTextChannel).Mention}**");
                builder.Description += $"\n{msg.Content}";
                // TODO
                /*if(msg.Embeds.Count>0){
                    foreach(var embed in msg.Embeds){
                        await channel.SendMessageAsync("",false,embed as Embed);
                    }
                }*/

                // if message had attachments queue them for logging
                if(msg.Attachments.Count>0){
                    Task.Run(()=>QueueDeletedMessagesWithAttachments(msg as SocketUserMessage,channel,messageChannel as SocketTextChannel));
                    return;
                }
            }
            // Build and send the embed
            await channel.SendMessageAsync("",false,builder.Build());
        }

        // method handling deletion of messages with attachments
        private async Task QueueDeletedMessagesWithAttachments(SocketUserMessage msg, SocketTextChannel channel, SocketTextChannel messageChannel){
            IReadOnlyCollection<IAttachment> attachments = msg.Attachments;

            // create an embed at the start of the message, add author, color, description,
            // and id of deleted message
            var startBuilder = new EmbedBuilder();
            startBuilder.WithAuthor(msg.Author);
            startBuilder.WithColor(Color.Red);
            startBuilder.WithDescription($"**Message with attachments was deleted in {messageChannel.Mention}**");
            startBuilder.Description += $"\n{msg.Content}";
            startBuilder.AddField("Message ID",msg.Id);
            // send the embed
            await channel.SendMessageAsync("",false,startBuilder.Build());

            // create http client instance to get deleted attachments
            using(var client = new HttpClient()){
                // get enumerator of attachments list
                var enumerator = attachments.GetEnumerator();

                Stream attachment;
                string fName;

                // execute as long as there are items in the list
                while(enumerator.MoveNext()){
                    // get filename of the attachment
                    fName = enumerator.Current.Filename;

                    // only image attachments can be downloaded, other throw 404 error
                    // if attachment isn't an image just display name, size
                    // and id of original message
                    if(!enumerator.Current.Width.HasValue && !enumerator.Current.Height.HasValue){
                        await channel.SendMessageAsync($"ID: {msg.Id}, type: binary\nname: {fName}\nsize: {enumerator.Current.Size} bytes");
                    }
                    // if attachment is image download it and resend it as new
                    else{
                        attachment = await client.GetStreamAsync(enumerator.Current.ProxyUrl);
                        await channel.SendFileAsync(attachment,fName,$"ID: {msg.Id}, type: image");
                    }
                    // wait 5 seconds between attachments
                    await Task.Delay(5000);
                }
            }


            // build and display embed signaling the end of all attachments
            var endBuilder = new EmbedBuilder();
            endBuilder.WithColor(Color.Red);
            endBuilder.WithCurrentTimestamp();
            endBuilder.WithDescription($"End of transmission with ID {msg.Id}");
            await channel.SendMessageAsync("",false,endBuilder.Build());
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
                builder.WithDescription($"Message edited in {(messageChannel as SocketTextChannel).Mention}");
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