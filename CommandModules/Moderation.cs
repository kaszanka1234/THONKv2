using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using THONK.Extensions.SocketGuildUserExtension;
using THONK.Configuration;

namespace THONK.CommandModules{
    public class Moderation : ModuleBase<SocketCommandContext>{
        
        private readonly IConfig _config;

        public Moderation(IConfig config){
            _config = config;
        }

        [Command("kick"),Priority(2)]
        public async Task Kick(SocketGuildUser user, [Remainder]string reason = ""){
            SocketGuildUser issuer = Context.User as SocketGuildUser;
            if(!issuer.Authorized("General") || !issuer.HigherThan(user)){
                await Context.Channel.SendMessageAsync(":x: Insufficient permissions");
                return;
            }
            if(user.ClanRank()==null){
                await Context.Channel.SendMessageAsync("user is not in a clan");
                return;
            }
            await user.RemoveRoleAsync(user.ClanRank());
            await Context.Channel.SendMessageAsync("user was kicked from clan");
            await user.SendMessageAsync($"You were kicked from a clan for: {(reason==""?"*no reason specified*":reason)}\nif you think this was a mistake and want to rejoin the clan message any sergeant or higher");
            var channel = _config[Context.Guild.Id].BotLogChannel;
            if(channel == null)return;
            var builder = new EmbedBuilder();
            builder.WithColor(Color.Red);
            builder.WithCurrentTimestamp();
            builder.WithAuthor(issuer);
            builder.WithDescription($"{user.Mention} ({user.Id}) was kicked\nreason: {reason}");
            await channel.SendMessageAsync("",false,builder.Build());
        }
        
        [Command("kick"),Priority(1)]
        public async Task KickUsage([Remainder]string a = ""){
            string p = _config[Context.Guild.Id].Prefix;
            string msg = $"usage:\n{p}kick @user (optional reason)";
            await Context.Channel.SendMessageAsync(msg);
        }

        [Command("warn"),Priority(2)]
        public async Task Warn(SocketGuildUser user,string type, [Remainder]string custom = ""){
            SocketGuildUser issuer = Context.User as SocketGuildUser;
            if(!issuer.Authorized("Lieutenant")){
                await Context.Channel.SendMessageAsync(":x: Insufficient permissions");
                return;
            }
            switch(type){
                case "inactive":
                    await user.SendMessageAsync("You have been inactive for a long time, if this continues you will be kicked out of the clan");
                    break;
                case "custom":
                    if(custom == ""){
                        await Context.Channel.SendMessageAsync("You have to specify reason for a warning");
                        return;
                    }
                    await user.SendMessageAsync($"You have been given a warning,\nreason: {custom}");
                    break;
                default:
                    await Context.Channel.SendMessageAsync("You have to use one of warning types:\ninactive, custom");
                    return;
            }
            await Context.Channel.SendMessageAsync("user was warned");
            var channel = _config[Context.Guild.Id].BotLogChannel;
            if(channel == null)return;
            var builder = new EmbedBuilder();
            builder.WithAuthor(issuer);
            builder.WithCurrentTimestamp();
            builder.WithDescription($"{user.Mention} ({user.Id}) was given a warning of type: {type}{(custom==""?"":$" reason: {custom}")}");
            await channel.SendMessageAsync("",false,builder.Build());
        }

        [Command("warn"),Priority(1)]
        public async Task WarnUsage([Remainder]string a = ""){
            string p = _config[Context.Guild.Id].Prefix;
            string msg = $"usage:\n{p}warn @user type (custom reason)\ntypes includes inactive, custom\ncustom reason is only used for custom type warnings";
            await Context.Channel.SendMessageAsync(msg);
        }
    }
}