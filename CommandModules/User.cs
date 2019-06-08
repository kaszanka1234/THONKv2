using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using THONK.Configuration;
using THONK.Extensions.SocketGuildUserExtension;
using THONK.utils;

namespace THONK.CommandModules{
    [Group("user")]
    public class User : ModuleBase<SocketCommandContext>{
        
        private readonly IConfig _config;

        public User(IConfig config){
            _config = config;
        }

        // show possible completions for command
        [Command(""),Priority(0)]
        public async Task Usage([Remainder]string s=""){
            string p = _config[Context.Guild.Id].Prefix;
            string msg = $"correct commands:\n{p}user approve @user\n{p}user rank @user (rank)\n{p}user mr (mr)\n{p}user inactive @user true/false";
            await Context.Channel.SendMessageAsync(msg);
        }

        // approve user as clan member
        [Command("approve"),Alias("accept","a"),Priority(2)]
        public async Task Approve(SocketGuildUser user){
            // get visitor role
            SocketRole visitor = Context.Guild.Roles.Where(x=>x.Name=="Visitor").First();
            // check if user has visitor role (if can be approved)
            if(user.Roles.Contains(visitor)){
                // add initiate role and remove visitor role
                await user.AddRoleAsync(Context.Guild.Roles.Where(x=>x.Name=="Initiate").First());
                await user.RemoveRoleAsync(visitor);

                // send a message signalig success
                string msg = $"**{HelperFunctions.NicknameOrUsername(user)}** is now member of the clan, welcome and have fun!";
                await Context.Channel.SendMessageAsync(msg);

                // send a message to botlog channel
                var channel = _config[Context.Guild.Id].BotLogChannel;
                if(channel!=null){
                    var builder = new EmbedBuilder();
                    builder.WithColor(Color.LightGrey);
                    builder.WithCurrentTimestamp();
                    builder.WithAuthor(Context.User);
                    builder.WithDescription($"User {HelperFunctions.UserIdentity(user)} was approved");
                    await channel.SendMessageAsync("",false,builder.Build());
                }
            }else{
                await Context.Channel.SendMessageAsync("User is already approved");
            }
        }

        // show usage for command
        [Command("approve"),Alias("accept","a"),Priority(1)]
        public async Task Approve(){
            string msg = $"Correct syntax:\n{_config[Context.Guild.Id].Prefix}user approve (@user)";
            await Context.Channel.SendMessageAsync(msg);
        }

        // command for changing user's rank
        [Command("rank"),Priority(2)]
        public async Task Rank(SocketGuildUser user, string rank){
            var userRequesting = Context.User as SocketGuildUser;
            // disallow users with rank lower than Lieutenant
            if(!userRequesting.IsAtLeast("Lieutenant")){await InsufficientPermissionsAsync();return;}

            // disallow changing roles of users with higher rank
            if(!userRequesting.HigherThan(user)){await InsufficientPermissionsAsync();return;}

            SocketRole role;
            string name;
            
            // match clan ranks with entered text and change any
            // combination of big and small letters to just
            // the first letter as big
            switch(rank.ToLower()){
                case "warlord":     name = "Warlord";break;
                case "general":     name = "General";break;
                case "lieutenant":  name = "Lieutenant";break;
                case "sergeant":    name = "Sergeant";break;
                case "soldier":     name = "Soldier";break;
                case "guest":       name = "Guest";break;
                case "initiate":    name = "Initiate";break;
                default:            name = "notfound";break;
            }

            // disallow setting roles higher than own
            if(!userRequesting.IsAtLeast(name)){await InsufficientPermissionsAsync();return;}
            
            // if passed role is correct clan rank set it as user role
            // and delete any other clan roles
            if(name != "notfound"){
                role = Context.Guild.Roles.Where(x=>x.Name==name).First();
                SocketRole bef = user.ClanRank();
                await user.AddRoleAsync(role);
                await user.DeleteClanRanksExceptAsync(name);
                string msg = $"Rank of {HelperFunctions.NicknameOrUsername(user)} was set to {role.Name}";
                await Context.Channel.SendMessageAsync(msg);
                var channel = _config[Context.Guild.Id].BotLogChannel;
                if(channel!=null){
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithAuthor(Context.User);
                    builder.WithDescription($"Rank of user {HelperFunctions.UserIdentity(user)} was changed");
                    builder.AddField("From",bef.Mention);
                    builder.AddField("To",role.Mention);
                    await channel.SendMessageAsync("",false,builder.Build());
                }
            }
            // if role isn't clan rank try to match it with any other assignable role
            else{
                await RankOther(user,rank);
            }
        }

        // overload for previous command
        [Command("rank"),Priority(2)]
        public async Task Rank(string rank,SocketGuildUser user)=>await Rank(user,rank);

        // show usage for command
        [Command("rank"),Priority(1)]
        public async Task Rank([Remainder]string s=""){
            string msg = $"Correct syntax:\n{_config[Context.Guild.Id].Prefix}user rank (@user) (rank)";
            await Context.Channel.SendMessageAsync(msg);
        }
        
        // method for setting roles not releated to clan ranks
        private async Task RankOther(SocketGuildUser user, string rank){
            if(false){
                // placeholder for setting other roles
            }else{
                string msg = "Unknown role";
                await Context.Channel.SendMessageAsync(msg);
            }
        }

        // set mastery rank for user
        [Command(""),Priority(1)]
        public async Task Mr(string one, string two, SocketGuildUser user=null) => await Mr(one+two, user);

        [Command(""),Priority(1)]
        public async Task Mr(string rank, SocketGuildUser user=null){
            int r;
            bool succ;
            string msg;

            // check if rank starts with "mr" and parse number of the rank
            if(rank.StartsWith("mr ",true,null)){
                succ = int.TryParse(rank.Substring(3),out r);
            }else if(rank.StartsWith("mr",true,null)){
                succ = int.TryParse(rank.Substring(2),out r);
            }else{
                await Usage();
                return;
            }
            if(succ&&r>=0&&r<=30){
                var target = Context.User;
                if(user!=null && (Context.User as SocketGuildUser).IsAtLeast("Lieutenant")){
                    target = user;
                }
                await SetMR(target, r);
                msg = $"{HelperFunctions.NicknameOrUsername(target as SocketGuildUser)} has been assigned MR{r}";
                await Context.Channel.SendMessageAsync(msg);
            }else{
                string p = _config[Context.Guild.Id].Prefix;
                msg = $"Something went wrong, try something like {p}user mr 15";
                await Context.Channel.SendMessageAsync(msg);
            }
        }

        // set mastery rank for user
        private async Task SetMR(SocketUser user, int mr){
            if(mr<0 || mr>30){
                throw new Exception("Mastery rank out of range!");
            }
            var u = user as SocketGuildUser;
            await u.RemoveRolesAsync(u.Roles.Where(x=>x.Name.Substring(0,2)=="MR"));
            await u.AddRoleAsync(Context.Guild.Roles.Where(x=>x.Name==$"MR{mr}").First());
        }

        // toggle user inactivity status
        [Command("inactive"),Priority(1)]
        public async Task SetInactive(SocketGuildUser user, bool? status=null){
            if(!(Context.User as SocketGuildUser).IsAtLeast("Lieutenant")){
                await InsufficientPermissionsAsync();
                return;
            }
            var inactiveRole = Context.Guild.Roles.Where(x=>x.Name=="Inactive").First();
            bool hasRole = user.Roles.Contains(inactiveRole);
            if(status==null && hasRole){
                status = false;
            }else if(status==null && !hasRole){
                status = true;
            }else if(hasRole == status){
                // nothing changed
                return;
            }
            
            var botLogChannel = _config[Context.Guild.Id].LogChannel;

            string msg = "";

            var builder = new EmbedBuilder();
            builder.WithAuthor(Context.User);
            builder.WithColor(Color.LightGrey);
            builder.WithCurrentTimestamp();

            if(status==false){
                await user.RemoveRoleAsync(inactiveRole);
                msg = $"User was set as active";
                builder.WithDescription($"User {HelperFunctions.UserIdentity(user)} was set as active");
            }else if(status==true){
                await user.AddRoleAsync(inactiveRole);
                msg = $"User was set as inactive";
                builder.WithDescription($"User {HelperFunctions.UserIdentity(user)} was set as {inactiveRole.Mention}");
            }
            await Context.Channel.SendMessageAsync(msg);
            await botLogChannel.SendMessageAsync("",false,builder.Build());
        }

        // show insufficient permissions
        private async Task InsufficientPermissionsAsync(){
            await Context.Channel.SendMessageAsync(":x: Insufficient Permissions");
        }
    }
    public static class UserExtensions{
        // SocketGuildUser extension to allow deleting all clan related roles except
        // the one given in argument
        public static async Task DeleteClanRanksExceptAsync(this SocketGuildUser user, string role){
            List<string> roles = new List<string>();
            roles.Add("Initiate");
            roles.Add("Guest");
            roles.Add("Soldier");
            roles.Add("Sergeant");
            roles.Add("Lieutenant");
            roles.Add("General");
            roles.Add("Warlord");
            roles.Remove(role);
            await user.RemoveRolesAsync(user.Roles.Where(x=>roles.Contains(x.Name)));
        }
    }
}
