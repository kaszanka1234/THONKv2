using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using THONK.Configuration;
using THONK.Extensions.SocketGuildUserExtension;

namespace THONK.CommandModules{
    [Group("user")]
    public class User : ModuleBase<SocketCommandContext>{
        
        private readonly IConfig _config;

        public User(IConfig config){
            _config = config;
        }

        // show possible completions for command
        [Command(""),Priority(-1)]
        public async Task Usage([Remainder]string s=""){
            string prefix = _config[Context.Guild.Id].Prefix;
            string msg = $"";
            await Context.Channel.SendMessageAsync(msg);
        }

        // command for changing user's rank
        [Command("rank")]
        public async Task Rank(SocketGuildUser user, string rank){
            var userRequesting = Context.User as SocketGuildUser;
            if(userRequesting.Authorized("Lieutenant")){
                await InsufficientPermissions();
                return;
            }
            if(!userRequesting.HigherThan(user)){await InsufficientPermissions();return;}
            
            // TODO set rank
        }

        // overload for previous command
        [Command("rank")]
        public async Task Rank(string rank,SocketGuildUser user)=>await Rank(user,rank);
        
        // show insufficient permissions
        public async Task InsufficientPermissions(){
            await Context.Channel.SendMessageAsync(":x: Insufficient Permissions");
        }
    }
}