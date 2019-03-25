using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using THONK.Configuration;

namespace THONK.CommandModules{
    [Group("config"),Alias("settings"),RequireUserPermission(Discord.GuildPermission.Administrator)]
    public class Configuration : ModuleBase<SocketCommandContext>{

        private IConfig _config {get;set;}
        private THONK.Services.ConfigLoader _loader {get;set;}

        [Group("get")]
        public class Get : Configuration{
            
            [Command("prefix")]
            public async Task Prefix(){
                await Context.Channel.SendMessageAsync($"current prefix is: \"{_config[Context.Guild.Id].Prefix}\"");
            }

            [Group("channel")]
            public class Channel : Get{
                
                [Command("general")]
                public async Task General(){
                    var channel = _config[Context.Guild.Id].GeneralChannel;
                    if(channel==null){
                        await Send(false,"General");
                    }else{
                        await Send(true,"General",channel.Mention);
                    }
                }

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

                public async Task Send(bool isSet, string channelName, string channelMention=""){
                    string msg = $"No channel set as {channelName}";
                    if(isSet){
                        msg = $"{channelName} channel is {channelMention}";
                    }
                    await Context.Channel.SendMessageAsync(msg);
                }

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

            [Command("")]
            new public async Task Usage(){
                var p = _config[Context.Guild.Id].Prefix;
                string msg = $"usage:\n{p}config get prefix\n{p}config get channel";
                await Context.Channel.SendMessageAsync(msg);
            }
            public Get(IConfig config, THONK.Services.ConfigLoader loader) : base(config,loader){
                _config = config;
            }
        }

        [Group("set")]
        public class Set : Configuration{
            
            [Command("prefix")]
            public async Task Prefix(string pref=""){
                string msg = $"Prefix changed to \"{pref}\" commands will look like this:\n{pref}ping";
                if(pref==""){
                    msg = $"Sets command prefix for this server\neg. {_config[Context.Guild.Id].Prefix}";
                    await Context.Channel.SendMessageAsync(msg);
                    return;
                }
                _config[Context.Guild.Id].Prefix = pref;
                await Context.Channel.SendMessageAsync(msg);
            }

            [Group("channel")]
            public class Channel : Set{
                
                [Command("general")]
                public async Task General(SocketTextChannel channel=null){
                    string msg;
                    if(channel==null){
                        _config[Context.Guild.Id].GeneralChannel = Context.Channel as SocketTextChannel;
                        msg = $"{(Context.Channel as SocketTextChannel).Mention} sucessfully set as General channel";
                    }else{
                        _config[Context.Guild.Id].GeneralChannel = channel;
                        msg = $"{channel.Mention} sucessfully set as General channel";
                    }
                    await Context.Channel.SendMessageAsync(msg);
                }

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