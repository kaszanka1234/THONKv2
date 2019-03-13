using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace THONK.CommandModules {
    [Group("plainstime"), Alias("cetus time","cetustime","plains time","pt","time")]
    public class PlainsTime : ModuleBase<SocketCommandContext> {
        [Command("")]
        public async Task Default() {
            var plainsTimeLeft = new Resources.PlainsTime().Time.Negate().Add(new TimeSpan(2,30,0));
            bool isDay = plainsTimeLeft.TotalMinutes > 50;
            if (isDay) plainsTimeLeft = plainsTimeLeft.Subtract(new TimeSpan(0, 50, 0));
            string message = 
                $"**{(isDay?"DAY":"NIGHT")}**\n" +
                $"{(plainsTimeLeft.Hours!=0?$"{plainsTimeLeft.Hours}h ":"")}{plainsTimeLeft.Minutes}m {plainsTimeLeft.Seconds}s left";
            await Context.Channel.SendMessageAsync(message);
        }
    }
}
