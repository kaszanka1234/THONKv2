using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using THONK.Resources;

namespace THONK.Services {
    public class RPUpdater {
        private readonly DiscordSocketClient _client;

        public RPUpdater(DiscordSocketClient client) {
            _client = client;
            Task.Run(() => CheckTimeAsync());
        }

        // Method to check time on plains
        private async Task CheckTimeAsync() {
            // variable that saves when status was last updated
            TimeSpan timeChecked = new TimeSpan();
            // run indefinetly
            while (true) {
                // create new PlainsTime objects with all values calculated
                var plainsTime = new PlainsTime();

                // if time changed by more than a minute update the status
                //
                // TODO
                // - make it >1 minute instead of 0 minutes
                // - add check for >30 seconds
                if (timeChecked.Minutes != plainsTime.Time.Minutes) {
                    timeChecked = plainsTime.Time;
                    // parse the data and set correct time as status
                    bool isDay = timeChecked.TotalMinutes <= 100;
                    int timeToShow = (int)(isDay?100-Math.Floor(timeChecked.TotalMinutes):150-Math.Floor(timeChecked.TotalMinutes));
                    if(timeToShow==0){
                        await SetPresenceAsync($"<1m to {(!isDay?"day":"night")}");
                    }else{
                        await SetPresenceAsync($"{timeToShow}m to {(!isDay?"day":"night")}");
                    }
                }
                // wait 2.5 seconds before rechecking
                await Task.Delay(2500);
            }
        }

        // Method that sets given text as status
        private async Task SetPresenceAsync(string text) {
            await _client.SetGameAsync(text);
        }
    }
}
