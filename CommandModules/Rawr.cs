using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using THONK.Services;

namespace THONK.CommandModules{
    public class Rawr : ModuleBase<SocketCommandContext>{

        private readonly Logger _logger;
        static Dictionary<ulong, DateTime> lastRwared = new Dictionary<ulong, DateTime>();

        static Random random = new Random((int)(DateTime.UtcNow.Ticks%int.MaxValue));
        int cooldownSec;


        public Rawr(Logger logger){
            cooldownSec = 10*60;
            _logger = logger;
        }

        [Command("rawr")]
        public async Task AtRawr([Remainder]string s=""){
            await Context.Channel.SendMessageAsync($"{Context.User.Mention} rawr");
        }

        [Command("random-rawr",true),RequireUserPermission(GuildPermission.MentionEveryone),Priority(11)]
        public async Task RandomRawr(string s=""){
            await DoRandRawr(s);
        }

        [Command("random-rawr",true),Priority(1)]
        public async Task RandomRawrUnprivileged(string s=""){
            ulong userId = Context.User.Id;
            if(lastRwared.ContainsKey(userId)){
                if(DateTime.UtcNow.CompareTo(lastRwared[userId].AddSeconds(cooldownSec)) > 0){
                    // allow
                    lastRwared[userId] = DateTime.UtcNow;
                    await DoRandRawr(s);
                    return;
                }
            }else{
                // allow
                lastRwared.Add(userId, DateTime.UtcNow);
                await DoRandRawr(s);
                return;
            }
            // dont allow
            
            await Context.Channel.SendMessageAsync("You are doing this too much!\nTry again later");
        }

        private async Task DoRandRawr(string s){
            List<SocketGuildUser> users;
            users = new List<SocketGuildUser>();
            foreach(var user in await Context.Channel.GetUsersAsync(CacheMode.AllowDownload).First()){
                if(user.Status != UserStatus.Offline){
                    users.Add(Context.Guild.GetUser(user.Id));
                }else if(user.Status == UserStatus.Offline && s.ToLower()=="offline"){
                    users.Add(Context.Guild.GetUser(user.Id));
                }
            }
            int rand = random.Next(users.Count);
            await _logger.LogAsync($"Counted {users.Count} members in {Context.Channel.Name}","rndRawr",LogSeverity.Verbose);
            await Context.Channel.SendMessageAsync($"{users[rand].Mention} rawr");
        }
    }
}