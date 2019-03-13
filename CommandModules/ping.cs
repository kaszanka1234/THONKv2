using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace THONK.Core.CommandModules {
    /* ping class extending module of type socket command */
    [Group("ping")]
    public class Ping : ModuleBase<SocketCommandContext>{
        /* set command's properties
         * [command's name, aliases of command, description of command] */
        [Command(""), Summary("Ping command")]
        public async Task Default(){
            await Context.Channel.SendMessageAsync("pong!");
        }
    }
}