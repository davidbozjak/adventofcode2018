using SantasToolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _4_SleepyGuards
{
    class MostSleepyGuard
    {
        static void Main(string[] args)
        {
            var logs = GetLogs().OrderBy(log => log.Timestamp).ToList();
            var shifts = GetShifts(logs);

            var shiftsByGuard = shifts
                .GroupBy(w => w.GuardId);

            var guardSleepMinutes = shiftsByGuard
                .ToDictionary(w => w.Key, w => w.Sum(shift => shift.MinuteState.Count(minute => minute == GuardState.Asleep)))
                .OrderByDescending(w => w.Value);

            var mostSleepyGuard = guardSleepMinutes
                .First()
                .Key;

            Console.WriteLine($"Most sleepy guard: {mostSleepyGuard}");

            Dictionary<int, int> minuteSleepFrequency = new Dictionary<int, int>(Enumerable.Range(0, 60).Select(w => new KeyValuePair<int, int>(w, 0)));

            var shiftsForMostSleepyGuard = shiftsByGuard
                .Where(w => w.Key == mostSleepyGuard)
                .SelectMany(w => w);

            foreach (var shift in shiftsForMostSleepyGuard)
            {
                for (int i = 0; i < shift.MinuteState.Length; i++)
                {
                    if (shift.MinuteState[i] == GuardState.Asleep)
                    {
                        minuteSleepFrequency[i]++;
                    }
                }
            }

            var mostSleepyMinute = minuteSleepFrequency.OrderByDescending(w => w.Value).First().Key;
            Console.WriteLine($"Most often minute: {mostSleepyMinute}");
            Console.WriteLine($"Result: {mostSleepyGuard * mostSleepyMinute}");

            Console.WriteLine("Done, Press any key co close.");
            Console.ReadKey();
        }

        private static List<ShiftLog> GetShifts(List<SleepLogEntry> logs)
        {
            var shifts = new List<ShiftLog>();

            for (int i = 0, j = 0; i < logs.Count; i = j)
            {
                for (j = i + 1; j < logs.Count && logs[j].LogMessageType != LogMessageType.NewGuard; j++) ;

                shifts.Add(new ShiftLog(logs.GetRange(i, j - i).GetEnumerator()));
            }

            return shifts;
        }

        private static List<SleepLogEntry> GetLogs()
        {
            var logProvider = new InputProvider<SleepLogEntry>("Input.txt", GetLog);

            var logs = new List<SleepLogEntry>();

            while (logProvider.MoveNext())
            {
                logs.Add(logProvider.Current);
            }
            
            return logs.ToList();
        }

        private static bool GetLog(string value, out SleepLogEntry result)
        {
            try
            {
                result = new SleepLogEntry(value);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
