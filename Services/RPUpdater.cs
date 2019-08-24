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
                if (timeChecked.Minutes != plainsTime.Time.Minutes || ((timeChecked.TotalMinutes>99 && timeChecked.TotalMinutes<100) || (timeChecked.TotalMinutes>149 && timeChecked.TotalMinutes<150))) {
                    timeChecked = plainsTime.Time;
                    bool setSubMiunte = false;
                    // parse the data and set correct time as status
                    bool isDay = timeChecked.TotalMinutes <= 100;
                    int timeToShow = (int)(isDay?100-Math.Ceiling(timeChecked.TotalMinutes):150-Math.Ceiling(timeChecked.TotalMinutes));
                    if(timeToShow==0){
                        if(timeChecked.Seconds>30 && !setSubMiunte){
                            setSubMiunte = true;
                            await SetPresenceAsync($"<30s to {(!isDay?"day":"night")}");
                        }else{
                            await SetPresenceAsync($"<1m to {(!isDay?"day":"night")}");
                        }
                    }else{
                        await SetPresenceAsync($"{timeToShow}m to {(!isDay?"day":"night")}");
                        setSubMiunte = false;
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
