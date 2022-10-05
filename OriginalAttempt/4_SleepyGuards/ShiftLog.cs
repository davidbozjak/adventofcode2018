using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace _4_SleepyGuards
{
    public enum GuardState { Awake, Asleep };

    class ShiftLog
    {
        public ShiftLog(IEnumerator<SleepLogEntry> shiftEvents)
        {
            shiftEvents.MoveNext();

            GuardId = GetGuardId(shiftEvents.Current.Message);

            this.MinuteState = new GuardState[60];
            int minute = 0;

            bool isAsleep = false;
            
            while (shiftEvents.MoveNext())
            {
                isAsleep = shiftEvents.Current.LogMessageType == LogMessageType.WakesUp;
                int eventMinute = shiftEvents.Current.Timestamp.Minute;

                AssignStateUntil(eventMinute);
                minute = eventMinute;
                isAsleep = !isAsleep;
            }

            AssignStateUntil(60);

            void AssignStateUntil(int stopMinute)
            {
                for (; minute < stopMinute; minute++)
                {
                    this.MinuteState[minute] = isAsleep ? GuardState.Asleep : GuardState.Awake;
                }
            }
        }

        public int GuardId { get; }

        public GuardState[] MinuteState { get; }

        private int GetGuardId(string shiftStartMessage)
        {
            var numberRegex = new Regex(@"\d+");
            return int.Parse(numberRegex.Match(shiftStartMessage).Value);
        }
    }
}
