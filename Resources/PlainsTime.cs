using System;

namespace THONK.Resources {
    class PlainsTime {
        private readonly int _totalSeconds;

        public readonly double cycleLength = 8998.8748;
        public TimeSpan Time { get; }

        public PlainsTime() {
            DateTime dayStart = new DateTime(2019, 2, 23, 21, 36, 21);
            TimeSpan cycle = DateTime.UtcNow - dayStart;
            double tmp = cycle.TotalSeconds % cycleLength;
            _totalSeconds = (int)tmp;
            int seconds = (int)tmp % 60;
            tmp = Math.Floor(tmp / 60);
            int minutes = (int)tmp % 60;
            tmp = Math.Floor(tmp / 60);
            int hours = (int)tmp;
            Time = new TimeSpan(hours, minutes, seconds);
        }
    }
}
