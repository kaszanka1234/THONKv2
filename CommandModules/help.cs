using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using THONK.Configuration;
using THONK.utils;

namespace THONK.Core.CommandModules {
    [Group("help")]
    public class Help : ModuleBase<SocketCommandContext>{
        
        private readonly IConfig _config;

        public Help(IConfig config)
        {
            _config = config;
        }
        [Command(""),Priority(11),RequireITWTGuild()]
        public async Task Default(){
            string p = _config[Context.Guild.Id].Prefix;
            string[] cmds = {
                "user",
                "plains time",
                "warn",
                "kick",
                "say",
                "announce"
            };
            string msg = "here are the commands you can use:\n";
            foreach(var s in cmds){
                msg += $"{p}{s}\n";
            }
            msg += "type in one of the commands for further usage help";
            await Context.Channel.SendMessageAsync(msg);
        }
        [Command(""),Priority(1)]
        public async Task DefaultNoITWT(){
            string p = _config[Context.Guild.Id].Prefix;
            string[] cmds = {
                "plains time",
                "warn",
                "say",
                "announce"
            };
            string msg = "here are the commands you can use:\n";
            foreach(var s in cmds){
                msg += $"{p}{s}\n";
            }
            msg += "type in one of the commands for further usage help";
            await Context.Channel.SendMessageAsync(msg);
        }
    }
}