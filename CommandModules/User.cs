using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
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
        [Command(""),Priority(0)]
        public async Task Usage([Remainder]string s=""){
            string p = _config[Context.Guild.Id].Prefix;
            string msg = $"correct commands:\n{p}user approve\n{p}user rank\n{p}user mr";
            await Context.Channel.SendMessageAsync(msg);
        }

        // approve user as clan member
        [Command("approve"),Alias("accept","a")]
        public async Task Approve(SocketGuildUser user){
            // get visitor role
            SocketRole visitor = Context.Guild.Roles.Where(x=>x.Name=="Visitor").First();
            // check if user has visitor role (if can be approved)
            if(user.Roles.Contains(visitor)){
                // add initiate role and remove visitor role
                await user.AddRoleAsync(Context.Guild.Roles.Where(x=>x.Name=="Initiate").First());
                await user.RemoveRoleAsync(visitor);

                // send a message signalig success
                string msg = $"**{(string.IsNullOrEmpty(user.Nickname)?user.Username:user.Nickname)}** is now member of the clan, welcome and have fun!";
                await Context.Channel.SendMessageAsync(msg);

                // send a message to botlog channel
                var channel = _config[Context.Guild.Id].BotLogChannel;
                if(channel!=null){
                    var builder = new EmbedBuilder();
                    builder.WithColor(Color.LightGrey);
                    builder.WithCurrentTimestamp();
                    builder.WithAuthor(Context.User);
                    builder.WithDescription($"User {user.Mention} ({user.Id}) was approved");
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
            if(!userRequesting.Authorized("Lieutenant")){await InsufficientPermissionsAsync();return;}

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
            if(!userRequesting.Authorized(name)){await InsufficientPermissionsAsync();return;}
            
            // if passed role is correct clan rank set it as user role
            // and delete any other clan roles
            if(name != "notfound"){
                role = Context.Guild.Roles.Where(x=>x.Name==name).First();
                SocketRole bef = user.ClanRank();
                await user.AddRoleAsync(role);
                await user.DeleteClanRanksExceptAsync(name);
                string msg = $"Rank of {(string.IsNullOrEmpty(user.Nickname)?user.Username:user.Nickname)} was set to {role.Name}";
                await Context.Channel.SendMessageAsync(msg);
                var channel = _config[Context.Guild.Id].BotLogChannel;
                if(channel!=null){
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithAuthor(Context.User);
                    builder.WithDescription($"Rank of user {user.Mention} ({user.Id}) was changed");
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
        [Command("")]
        public async Task Mr([Remainder]string rank){
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
                await (Context.User as SocketGuildUser).RemoveRolesAsync((Context.User as SocketGuildUser).Roles.Where(x=>x.Name.Substring(0,2)=="MR"));
                await (Context.User as SocketGuildUser).AddRoleAsync(Context.Guild.Roles.Where(x=>x.Name==$"MR{r}").First());
                msg = $"{(string.IsNullOrEmpty((Context.User as SocketGuildUser).Nickname)?Context.User.Username:(Context.User as SocketGuildUser).Nickname)} has been assigned MR{r}";
                await Context.Channel.SendMessageAsync(msg);
            }else{
                string p = _config[Context.Guild.Id].Prefix;
                msg = $"Something went wrong, try something like {p}user mr 15";
                await Context.Channel.SendMessageAsync(msg);
            }
        }

        // force mastery rank for user
        // [Command("setmr")]
        // public async Task SetMR(){
        //    // TODO 
        // }

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