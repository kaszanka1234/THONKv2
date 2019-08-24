using System;

namespace THONK.Resources {
    class PlainsTime {
        // total seconds that elapsed since the beginning of current cetus cycle
        //private readonly int _totalSeconds;
        
        // cetus cycle length in seconds
        // it's still not correct
        // investigation is in progress
        public readonly double cycleLength = 8998.8748;

        // variable representing time on cetus
        // total of 150(not exactly but it's within a margin of error here) mintues
        public TimeSpan Time { get; }

        public PlainsTime() {
            // any date that day on cetus started
            DateTime dayStart = new DateTime(2019, 8, 24, 20, 4, 20).ToUniversalTime();
            // get current time on cetus (150 minutes)
            TimeSpan cycle = DateTime.UtcNow - dayStart;

            // debug
            //DateTime dbg = new DateTime(2019,8,24,21,44,21).ToUniversalTime();
            //TimeSpan cycle = dbg - dayStart;

            // calculate correct values
            double tmp = cycle.TotalSeconds % cycleLength;
            //_totalSeconds = (int)tmp;
            int seconds = (int)tmp % 60;
            tmp = Math.Floor(tmp / 60);
            int minutes = (int)tmp % 60;
            tmp = Math.Floor(tmp / 60);
            int hours = (int)tmp;

            // assign values to public vaiable Time
            Time = new TimeSpan(hours, minutes, seconds);
        }
    }
}
