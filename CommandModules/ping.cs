using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace THONK.Core.CommandModules {
    /* ping class extending module of type socket command */
    public class Ping : ModuleBase<SocketCommandContext>{
        /* set command's properties
         * [command's name, aliases of command, description of command] */
        [Command("ping"), Summary("Ping command")]
        public async Task Default(){
            await Context.Channel.SendMessageAsync("pong!");
        }
        [Command("rawr")]
        public async Task Rawr([Remainder]string s = ""){
            await Context.Channel.SendMessageAsync($"{Context.User.Mention} rawr");
        }
    }
}