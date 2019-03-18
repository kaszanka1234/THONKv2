using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using THONK.Configuration;

namespace THONK.CommandModules{
    [Group("config"),Alias("settings"),RequireUserPermission(Discord.GuildPermission.Administrator)]
    public class Configuration : ModuleBase<SocketCommandContext>{

        private IConfig _config {get;set;}
        private THONK.Services.ConfigLoader _services {get;set;}

        [Group("get")]
        public class Get : Configuration{
            //new private IConfig _config;
            
            [Command("prefix")]
            public async Task Prefix(){
                await Context.Channel.SendMessageAsync($"current prefix is: \"{_config[Context.Guild.Id].Prefix}\"");
            }

            [Command("")]
            new public async Task Usage(){
                var p = this._config[Context.Guild.Id].Prefix;
                string msg = $"usage:\n{p}config get prefix";
                await Context.Channel.SendMessageAsync(msg);
            }
            public Get(Config config, THONK.Services.ConfigLoader services) : base(config,services){
                this._config = config;
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

            public Set(Config config, THONK.Services.ConfigLoader services):base(config,services){
                _config = config;
            }
        }

        [Command("save")]
        public async Task Save(){
            _services.SaveGuildConfig(Context.Guild.Id);
            await Context.Channel.SendMessageAsync("success");
        }

        [Command("")]
        public async Task Usage(){
            var p = _config[Context.Guild.Id].Prefix;
            string msg = $"usage:\n{p}config get\n{p}config set\n{p}config save";
            await Context.Channel.SendMessageAsync(msg);
        }

        public Configuration(Config config, THONK.Services.ConfigLoader services){
            _config = config;
            _services = services;
        }
    }
}