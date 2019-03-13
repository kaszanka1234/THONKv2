using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using THONK.Resources;

namespace THONK.Services {
    public class RPUpdater {
        private readonly DiscordSocketClient _client;

        public RPUpdater(DiscordSocketClient client) {
            _client = client;
            Task.Run(() => CheckTime());
        }

        private async Task CheckTime() {
            TimeSpan timeChecked = new TimeSpan();
            while (true) {
                var plainsTime = new PlainsTime();
                if (timeChecked.Minutes != plainsTime.Time.Minutes) {
                    timeChecked = plainsTime.Time;
                    bool isDay = timeChecked.TotalMinutes <= 100;
                    await SetPresenceAsync($"{(isDay?100-Math.Floor(timeChecked.TotalMinutes):150-Math.Floor(timeChecked.TotalMinutes))}m to {(!isDay?"day":"night")}");
                }
                await Task.Delay(2500);
            }
        }

        private async Task SetPresenceAsync(string text) {
            await _client.SetGameAsync(text);
        }
    }
}
