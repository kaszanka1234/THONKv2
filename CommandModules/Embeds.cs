using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace THONK.Core.CommandModules {
    [Group("embeds"),RequireUserPermission(Discord.GuildPermission.Administrator)]
    public class Embeds : ModuleBase<SocketCommandContext>{
        [Command("rules")]
        public async Task Rules(){
            var builder = new EmbedBuilder();
            //
            await Context.Channel.SendMessageAsync("",false,builder.Build());
        }

        [Command("ranks")]
        public async Task Ranks(){
            //
        }

        private async Task RanksTask(){
            var[] builders = {new EmbedBuilder()};
            //
            
            for(int i=0;i<builders.Length();++i){
                await Context.Channel.SendMessageAsync("",false,builders[i].Build());
                await Task.Delay(2500);
            }
        }

        [Command("sguide")]
        public async Task Sguide(){
            var builder = new EmbedBuilder();
            //
            await Context.Channel.SendMessageAsync("",false,builder.Build());
        }
        
    }
}