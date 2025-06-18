using System;

namespace VARLab.CCSIF
{
    public class Timer
    {
        private TimeSpan startTime;
        private TimeSpan endTime;
        private bool isRunning = false;

        public TimeSpan TimeTaken { 
            get {
                if (isRunning) { return DateTime.Now.TimeOfDay - startTime; }
                else { return endTime - startTime; }
            } 
        }

        public Timer()
        {
            isRunning = false;
        }

        public void StartTimer()
        {
            isRunning = true;
            startTime = DateTime.Now.TimeOfDay;
        }

        public void StopTimer()
        {
            isRunning = false;
            endTime = DateTime.Now.TimeOfDay;
        }

        public void RestartTimer()
        {
            isRunning = true;
            startTime = DateTime.Now.TimeOfDay;
        }
    }
}
