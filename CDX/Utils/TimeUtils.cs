using System;
using System.Diagnostics;

namespace CDX.Utils
{
    public class StopwatchOffset
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan  _offset;
        
        public TimeSpan elapsed => _stopwatch.Elapsed + _offset;

        public TimeSpan offset
        {
            get => _offset;
            set => _offset = value;
        }
        
        public int millis()
        {
            return (int) (_stopwatch.Elapsed + _offset).TotalMilliseconds;
        }

        public void start()
        {
            _stopwatch.Start();
        }

        public void restart()
        {
            _stopwatch.Restart();
        }
    }
    
    public static class TimeUtils
    {
        private const long NS_PER_MS = 1_000_000L;

        private static StopwatchOffset _stopwatch = new StopwatchOffset();

        static TimeUtils()
        {
            _stopwatch.start();
        }
        
        public static long nanoTime()
        {
            return millisToNanos(millis());
        }
        
        public static int millis()
        {
            return _stopwatch.millis();
        }

        public static float seconds()
        {
            return millis() / 1000f;
        }

        private static long millisToNanos(long millis)
        {
            return millis * NS_PER_MS;
        }

        public static void restart()
        {
            _stopwatch.restart();
        }

        public static void setOffset(TimeSpan offset)
        {
            _stopwatch.offset = offset;
        }
    }
}