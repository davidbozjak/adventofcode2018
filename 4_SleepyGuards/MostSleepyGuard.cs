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

            PrintStrategy1Result(shiftsByGuard);

            Console.WriteLine("");

            PrintSrategy2Result(shiftsByGuard);

            Console.WriteLine("");
            Console.WriteLine("Done, Press any key co close.");
            Console.ReadKey();
        }
        
        private static void PrintStrategy1Result(IEnumerable<IGrouping<int, ShiftLog>> shiftsByGuard)
        {
            Console.WriteLine("Strategy 1:");

            var guardSleepMinutes = shiftsByGuard
                .ToDictionary(w => w.Key, w => w.Sum(shift => shift.MinuteState.Count(minute => minute == GuardState.Asleep)))
                .OrderByDescending(w => w.Value);

            var mostSleepyGuard = guardSleepMinutes
                .First()
                .Key;

            Console.WriteLine($"Most sleepy guard: {mostSleepyGuard}");
            var minuteSleepFrequencyForMostSleepyGuard = GetGuardSleepFrequency(shiftsByGuard, mostSleepyGuard);

            var mostSleepyMinute = minuteSleepFrequencyForMostSleepyGuard.OrderByDescending(w => w.Value).First().Key;
            Console.WriteLine($"Most often minute: {mostSleepyMinute}");
            Console.WriteLine($"Result Strategy 1: {mostSleepyGuard * mostSleepyMinute}");
        }

        private static Dictionary<int, int> GetGuardSleepFrequency(IEnumerable<IGrouping<int, ShiftLog>> shiftsByGuard, int mostSleepyGuard)
        {
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

            return minuteSleepFrequency;
        }

        private static void PrintSrategy2Result(IEnumerable<IGrouping<int, ShiftLog>> shiftsByGuard)
        {
            Console.WriteLine("Strategy 2:");

            int mostConsistentlyAsleepGuardID = -1;
            int mostFrequentlyAsleepMinute = -1;
            int mostFrequentlyAsleepMinuteValue = -1;

            foreach (var guardId in shiftsByGuard.Select(w => w.Key))
            {
                var minuteSleepFrequency = GetGuardSleepFrequency(shiftsByGuard, guardId);

                var mostFrequentlyAsleepMinuteForGuard = minuteSleepFrequency.OrderByDescending(w => w.Value).First();

                if (mostFrequentlyAsleepMinuteForGuard.Value > mostFrequentlyAsleepMinuteValue)
                {
                    mostConsistentlyAsleepGuardID = guardId;
                    mostFrequentlyAsleepMinute = mostFrequentlyAsleepMinuteForGuard.Key;
                    mostFrequentlyAsleepMinuteValue = mostFrequentlyAsleepMinuteForGuard.Value;
                }
            }

            Console.WriteLine($"Most Consistently Asleep GuardID: {mostConsistentlyAsleepGuardID}");
            Console.WriteLine($"Most Consistently Asleep Minute: {mostFrequentlyAsleepMinute}");
            Console.WriteLine($"Result Strategy 2: {mostConsistentlyAsleepGuardID * mostFrequentlyAsleepMinute}");
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
