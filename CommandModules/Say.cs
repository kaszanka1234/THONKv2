using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace THONK.CommandModules{

    public class Say : ModuleBase<SocketCommandContext> {

        THONK.Configuration.IConfig _config {get;}

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

        [Command("w"),Alias("pm")]
        public async Task PrivateMessage(SocketUser target, [Remainder]string msg){
            if(Context.User.Id!=333769079569776642)return;
            await target.SendMessageAsync(msg);
        }

        public Say(THONK.Configuration.IConfig config){
            _config = config;
        }
    }
}