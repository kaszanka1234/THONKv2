using System;
using System.IO;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace THONK.Services{
    public class CommandHandler{
        private readonly IServiceProvider _services;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly THONK.Configuration.IConfig _config;

        public CommandHandler(IServiceProvider services, DiscordSocketClient client, CommandService commands, THONK.Configuration.IConfig config){
            _client = client;
            _commands = commands;
            _config = config;
            _services = services;
            _client.MessageReceived += MessageRecievedAsync;
        }

        // Method to execute on recieveing a message
        private async Task MessageRecievedAsync(SocketMessage m){
            // return if it's not a message sent by valid user
            if (!(m is SocketUserMessage msg)) return;

            // Don't respond to other bots and webhooks
            if (msg.Author.IsBot)return;
            if(msg.Author.IsWebhook)return;

            // Also don't respond to self
            if(msg.Author.Id==_client.CurrentUser.Id)return;

            // message is private message
            if(msg.Channel is Discord.IPrivateChannel){
                var kas = _client.Guilds.FirstOrDefault().Users.Single(x => x.Id == 333769079569776642);
                var builder = new EmbedBuilder();
                builder.WithAuthor(msg.Author);
                builder.WithDescription(msg.Content);
                if(msg.Attachments.Count>0){
                    using(var client = new HttpClient()){
                        // get enumerator of attachments list
                        var enumerator = msg.Attachments.GetEnumerator();

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
                                await kas.SendMessageAsync($"ID: {msg.Id}, type: binary\nname: {fName}\nsize: {enumerator.Current.Size} bytes");
                            }
                            // if attachment is image download it and resend it as new
                            else{
                                attachment = await client.GetStreamAsync(enumerator.Current.ProxyUrl);
                                await kas.SendFileAsync(attachment,fName,$"ID: {msg.Id}, type: image");
                            }
                        }
                    }
                }
                await kas.SendMessageAsync("",false,builder.Build());
                return;
            }

            // Create new command context
            // it provides all relevant info about user that has sent the message and the channel it was sent in
            var context = new SocketCommandContext(_client, msg);

            // define at which position command prefix is at
            int argPos=0;

            // return if message doesn't have a command prefix (bot can also be mentioned instead of of using prefix)
            if(msg.HasStringPrefix(_config[context.Guild.Id].Prefix, ref argPos)||msg.HasMentionPrefix(_client.CurrentUser, ref argPos)){

                // find correct module and execute the command
                // get back a result of executed command
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                // if the result isn't a success respond to user
                if(!result.IsSuccess){
                    await context.Channel.SendMessageAsync(result.ToString());
                }
            }
        }
    }
}