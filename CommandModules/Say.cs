using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace THONK.CommandModules{

    public class Say : ModuleBase<SocketCommandContext> {

        THONK.Configuration.Config _config {get;}

        [Command("say"),RequireUserPermission(Discord.GuildPermission.ManageMessages)]
        public async Task Echo([Remainder] string msg){
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(msg);
        }
        [Command("announce"),RequireUserPermission(Discord.GuildPermission.ManageMessages)]
        public async Task Announce([Remainder]string msg){
            var channel = _config[Context.Guild.Id].AnnouncementsChannel;
            if(channel==null) {
                await Context.Channel.SendMessageAsync("No announcements channel set!");
                return;
            }
            await channel.SendMessageAsync(msg);
        }

        public Say(THONK.Configuration.Config config){
            _config = config;
        }
    }
}