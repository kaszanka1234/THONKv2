using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using THONK.Configuration;

namespace THONK.CommandModules{

    // Group of commands to change current guild config

    // require user administrator permission to run these commands
    [Group("config"),Alias("settings"),RequireUserPermission(Discord.GuildPermission.Administrator)]
    public class Configuration : ModuleBase<SocketCommandContext>{

        // ONLY COMMAND WHERE CONFIG IS MODIFABLE
        private IConfig _config {get;set;}
        private THONK.Services.ConfigLoader _loader {get;set;}

        // group of commands to get current confgurations
        [Group("get")]
        public class Get : Configuration{
            
            // get current command prefix
            // but how do you get command prefix if you don't know it in the first place :thonk:
            [Command("prefix")]
            public async Task Prefix(){
                await Context.Channel.SendMessageAsync($"current prefix is: \"{_config[Context.Guild.Id].Prefix}\"");
            }

            // group of commands to get channels
            [Group("channel")]
            public class Channel : Get{
                
                // show general channel
                [Command("general")]
                public async Task General(){
                    var channel = _config[Context.Guild.Id].GeneralChannel;
                    if(channel==null){
                        await Send(false,"General");
                    }else{
                        await Send(true,"General",channel.Mention);
                    }
                }

                // do i really have to document every single method...
                [Command("announcements")]
                public async Task Announcements(){
                    var channel = _config[Context.Guild.Id].AnnouncementsChannel;
                    if(channel==null){
                        await Send(false,"Announcements");
                    }else{
                        await Send(true,"Announcements",channel.Mention);
                    }
                }

                [Command("bot log"),Alias("botlog")]
                public async Task BotLog(){
                    var channel = _config[Context.Guild.Id].BotLogChannel;
                    if(channel==null){
                        await Send(false,"Bot log");
                    }else{
                        await Send(true,"Bot log",channel.Mention);
                    }
                }

                [Command("log")]
                public async Task Log(){
                    var channel = _config[Context.Guild.Id].LogChannel;
                    if(channel==null){
                        await Send(false,"Log");
                    }else{
                        await Send(true,"Log",channel.Mention);
                    }
                }

                // CODE REUSABILITY! YAY! send that channel given in parameter is set, or not
                public async Task Send(bool isSet, string channelName, string channelMention=""){
                    string msg = $"No channel set as {channelName}";
                    if(isSet){
                        msg = $"{channelName} channel is {channelMention}";
                    }
                    await Context.Channel.SendMessageAsync(msg);
                }

                // parameterless command to show usage of the command
                [Command("")]
                new public async Task Usage(){
                    var p = _config[Context.Guild.Id].Prefix;
                    string msg = $"usage:\n{p}config get channel general\n{p}config get channel announcements\n{p}config get channel bot log\n{p}config get channel log";
                    await Context.Channel.SendMessageAsync(msg);
                }

                public Channel(IConfig config, THONK.Services.ConfigLoader loader) : base(config,loader){
                    _config = config;
                }
            }

            // paramterless command to show the usage of the command
            // i guess thes should be at the top
            // but meh
            [Command("")]
            new public async Task Usage(){
                var p = _config[Context.Guild.Id].Prefix;
                string msg = $"usage:\n{p}config get prefix\n{p}config get channel";
                await Context.Channel.SendMessageAsync(msg);
            }
            // constructors too :thonk:
            public Get(IConfig config, THONK.Services.ConfigLoader loader) : base(config,loader){
                _config = config;
            }
        }

        // Group of commands to change configuration
        // these commands do not make any changes to database
        // they only change in memory configuration
        // if not saved all changes are lost on restart
        [Group("set")]
        public class Set : Configuration{
            
            // set command prefix
            [Command("prefix")]
            public async Task Prefix(string pref=""){
                string msg = $"Prefix changed to \"{pref}\" commands will look like this:\n{pref}ping";
                // if prefix is empty say that it cannot be empty and return
                if(pref==""){
                    msg = $"Sets command prefix for this server\neg. {_config[Context.Guild.Id].Prefix}";
                    await Context.Channel.SendMessageAsync(msg);
                    return;
                }

                // change the prefix and signal a success
                _config[Context.Guild.Id].Prefix = pref;
                await Context.Channel.SendMessageAsync(msg);
            }

            // group of commands for changing channels
            [Group("channel")]
            public class Channel : Set{
                
                // same as get channel but this sets it xD
                //
                [Command("general")]
                public async Task General(SocketTextChannel channel=null){
                    string msg;
                    // if no channel is passed as a parameter set it to channel this was executed in
                    if(channel==null){
                        _config[Context.Guild.Id].GeneralChannel = Context.Channel as SocketTextChannel;
                        msg = $"{(Context.Channel as SocketTextChannel).Mention} sucessfully set as General channel";
                    }
                    // or set it to channel passed as parameter
                    else{
                        _config[Context.Guild.Id].GeneralChannel = channel;
                        msg = $"{channel.Mention} sucessfully set as General channel";
                    }
                    // signal success
                    await Context.Channel.SendMessageAsync(msg);
                }

                // same applies to 3 commands below
                // but for their respective channels
                // and no fancy 'Send' method here
                [Command("announcements")]
                public async Task Announcements(SocketTextChannel channel=null){
                    string msg;
                    if(channel==null){
                        _config[Context.Guild.Id].AnnouncementsChannel = Context.Channel as SocketTextChannel;
                        msg = $"{(Context.Channel as SocketTextChannel).Mention} sucessfully set as Announcements channel";
                    }else{
                        _config[Context.Guild.Id].AnnouncementsChannel = channel;
                        msg = $"{channel.Mention} sucessfully set as Announcements channel";
                    }
                    await Context.Channel.SendMessageAsync(msg);
                }

                [Command("bot log"),Alias("botlog")]
                public async Task BotLog(SocketTextChannel channel=null){
                    string msg;
                    if(channel==null){
                        _config[Context.Guild.Id].BotLogChannel = Context.Channel as SocketTextChannel;
                        msg = $"{(Context.Channel as SocketTextChannel).Mention} sucessfully set as Bot log channel";
                    }else{
                        _config[Context.Guild.Id].BotLogChannel = channel;
                        msg = $"{channel.Mention} sucessfully set as Bot log channel";
                    }
                    await Context.Channel.SendMessageAsync(msg);
                }

                [Command("log")]
                public async Task Log(SocketTextChannel channel=null){
                    string msg;
                    if(channel==null){
                        _config[Context.Guild.Id].LogChannel = Context.Channel as SocketTextChannel;
                        msg = $"{(Context.Channel as SocketTextChannel).Mention} sucessfully set as Log channel";
                    }else{
                        _config[Context.Guild.Id].LogChannel = channel;
                        msg = $"{channel.Mention} sucessfully set as Log channel";
                    }
                    await Context.Channel.SendMessageAsync(msg);
                }

                // this should probably be finished
                new public async Task Usage(){
                    string msg = $"a";
                    await Context.Channel.SendMessageAsync(msg);
                }

                public Channel(IConfig config, THONK.Services.ConfigLoader loader) : base(config,loader){
                    _config = config;
                }
            }

            public Set(IConfig config, THONK.Services.ConfigLoader loader):base(config,loader){
                _config = config;
            }
        }

        // save the config to database
        [Command("save")]
        public async Task Save(){
            _loader.SaveGuildConfig(Context.Guild.Id);
            await Context.Channel.SendMessageAsync("success");
        }

        [Command("")]
        public async Task Usage(){
            var p = _config[Context.Guild.Id].Prefix;
            string msg = $"usage:\n{p}config get\n{p}config set\n{p}config save";
            await Context.Channel.SendMessageAsync(msg);
        }

        public Configuration(IConfig config, THONK.Services.ConfigLoader loader){
            _config = config;
            _loader = loader;
        }
    }
}