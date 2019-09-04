using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using THONK.Services;
using THONK.utils;

namespace THONK.CommandModules{
    public class Rawr : ModuleBase<SocketCommandContext>{

        private readonly Logger _logger;
        static Dictionary<ulong, DateTime> untilNextRawr = new Dictionary<ulong, DateTime>();

        static Random random = new Random((int)(DateTime.UtcNow.Ticks%int.MaxValue));
        int cooldownSec;
        int maxCooldownOffset;


        public Rawr(Logger logger){
            cooldownSec = 10*60;
            maxCooldownOffset = cooldownSec;
            _logger = logger;
        }

        [Command("rawr")]
        public async Task AtRawr([Remainder]string s=""){
            await Context.Channel.SendMessageAsync($"{Context.User.Mention} rawr");
        }

        [Command("random-rawr",true),RequireUserPermission(GuildPermission.MentionEveryone),Priority(11)]
        public async Task RandomRawr(string s=""){
            StatisticData data;
            data.addedCooldown = 0;
            data.channel = Context.Channel.Name;
            data.count = -1;
            data.rand = -1;
            data.server = Context.Guild.Name;
            data.username = "";
            await DoRandRawr(s, data);
        }

        [Command("random-rawr",true),Priority(1)]
        public async Task RandomRawrUnprivileged(string s=""){
            StatisticData data;
            data.addedCooldown = 0;
            data.channel = Context.Channel.Name;
            data.count = -1;
            data.rand = -1;
            data.server = Context.Guild.Name;
            data.username = "";
            ulong userId = Context.User.Id;
            if(untilNextRawr.ContainsKey(userId)){
                if(DateTime.UtcNow.CompareTo(untilNextRawr[userId]) > 0){
                    // allow
                    int cooldown = cooldownSec+random.Next(maxCooldownOffset);
                    data.addedCooldown = cooldown;
                    untilNextRawr[userId] = DateTime.UtcNow.AddSeconds(cooldown);
                    await DoRandRawr(s,data);
                    return;
                }
            }else{
                // allow
                int cooldown = cooldownSec+random.Next(maxCooldownOffset);
                    data.addedCooldown = cooldown;
                    untilNextRawr[userId] = DateTime.UtcNow.AddSeconds(cooldown);
                await DoRandRawr(s,data);
                return;
            }
            // dont allow
            
            await Context.Channel.SendMessageAsync("You are doing this too much!\nTry again later");
        }

        struct StatisticData{
            public int count;
            public int rand;
            public int addedCooldown;
            public string channel;
            public string server;
            public string username;
        }

        private async Task DoRandRawr(string s, StatisticData data){
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
            var userToRawr = users[rand];
            data.rand = rand;
            data.count = users.Count;
            data.username = HelperFunctions.NicknameOrUsername(userToRawr);
            //if(data==null){
            //    await _logger.LogAsync($"Counted {users.Count} members in {Context.Channel.Name}","rndRawr",LogSeverity.Verbose);
            //}else{
                await _logger.LogAsync($"selected {data.rand} from {data.count} users ({data.username}) in {data.channel} on {data.server}","rndRawr",LogSeverity.Verbose);
            //}
            
            await Context.Channel.SendMessageAsync($"{userToRawr.Mention} rawr");
        }
    }
}