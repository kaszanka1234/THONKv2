using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace THONK.CommandModules{
    public class Rawr : ModuleBase<SocketCommandContext>{

        static Dictionary<ulong, DateTime> lastRwared = new Dictionary<ulong, DateTime>();

        static Random random = new Random((int)(DateTime.UtcNow.Ticks%int.MaxValue));
        int cooldownSec;


        public Rawr(){
            cooldownSec = 10*60;
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
            SocketGuildUser[] users;
            if(s=="offline"){
                users = Context.Guild.Users.ToArray();
            }else{
                users = Context.Guild.Users.Where(x => x.Status != UserStatus.Offline).ToArray();
            }
            int rand = random.Next(users.Length);

            await Context.Channel.SendMessageAsync($"{users[rand].Mention} rawr");
        }
    }
}