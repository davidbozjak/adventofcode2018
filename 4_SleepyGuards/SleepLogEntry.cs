using System;

namespace _4_SleepyGuards
{
    public enum LogMessageType { NewGuard, FallsAsleep, WakesUp };

    class SleepLogEntry
    {
        public SleepLogEntry(string input)
        {
            var indexOfSeperator = input.IndexOf(']');
            this.Timestamp = DateTime.Parse(input.Substring(1, indexOfSeperator - 1));
            this.Message = input.Substring(indexOfSeperator + 1).Trim().ToLower();
            this.LogMessageType = Message.Contains("wake") ? LogMessageType.WakesUp : Message.Contains("asleep") ? LogMessageType.FallsAsleep : LogMessageType.NewGuard;
        }
        
        public DateTime Timestamp { get; }

        public string Message { get; }

        public LogMessageType LogMessageType { get; }
    }
}
