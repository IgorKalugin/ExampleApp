using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;

namespace Example.Model.DayOfWeek
{
    // We use variable name "dayOfWeek" to indicate that it contains only individual days (like Sunday, Monday, etc)
    // And we use variable name "repetition" to indicate that it contains pattern of several days (like Empty, Weekdays, Sunday | Monday, etc)
    public static class RepetitionDayOfWeekEx
    {
        private static readonly List<RepetitionDayOfWeek> daysOfWeek = new List<RepetitionDayOfWeek> 
        { 
            RepetitionDayOfWeek.Monday, 
            RepetitionDayOfWeek.Tuesday, 
            RepetitionDayOfWeek.Wednesday, 
            RepetitionDayOfWeek.Thursday,
            RepetitionDayOfWeek.Friday, 
            RepetitionDayOfWeek.Saturday,
            RepetitionDayOfWeek.Sunday,
        };
        
        private static readonly Dictionary<RepetitionDayOfWeek, string> names = new Dictionary<RepetitionDayOfWeek, string>
        {
            { RepetitionDayOfWeek.Monday, "Monday" },
            { RepetitionDayOfWeek.Tuesday, "Tuesday" },
            { RepetitionDayOfWeek.Wednesday, "Wednesday" },
            { RepetitionDayOfWeek.Thursday, "Thursday" },
            { RepetitionDayOfWeek.Friday, "Friday" },
            { RepetitionDayOfWeek.Saturday, "Saturday" },
            { RepetitionDayOfWeek.Sunday, "Sunday" },
        };
        
        private static readonly Dictionary<RepetitionDayOfWeek, string> shortNames = new Dictionary<RepetitionDayOfWeek, string>
        {
            { RepetitionDayOfWeek.Empty, string.Empty },
            { RepetitionDayOfWeek.Monday, "MON" },
            { RepetitionDayOfWeek.Tuesday, "TUE" },
            { RepetitionDayOfWeek.Wednesday, "WED" },
            { RepetitionDayOfWeek.Thursday, "THU" },
            { RepetitionDayOfWeek.Friday, "FRI" },
            { RepetitionDayOfWeek.Saturday, "SAT" },
            { RepetitionDayOfWeek.Sunday, "SUN" },
            { RepetitionDayOfWeek.Weekdays, "WEEKDAYS" },
            { RepetitionDayOfWeek.Weekend, "WEEKEND" },
            { RepetitionDayOfWeek.EveryDay, "EVERY DAY" }
        };
        
        private static readonly IDictionary<System.DayOfWeek, RepetitionDayOfWeek> mapping = new Dictionary<System.DayOfWeek, RepetitionDayOfWeek>
        {   
            { System.DayOfWeek.Monday, RepetitionDayOfWeek.Monday },
            { System.DayOfWeek.Tuesday, RepetitionDayOfWeek.Tuesday },
            { System.DayOfWeek.Wednesday, RepetitionDayOfWeek.Wednesday },
            { System.DayOfWeek.Thursday, RepetitionDayOfWeek.Thursday },
            { System.DayOfWeek.Friday, RepetitionDayOfWeek.Friday },
            { System.DayOfWeek.Saturday, RepetitionDayOfWeek.Saturday },
            { System.DayOfWeek.Sunday, RepetitionDayOfWeek.Sunday },
        };
        
        public static string GetName(this RepetitionDayOfWeek dayOfWeek)
        {
            return names[dayOfWeek];
        }
        
        public static string GetShortName(this RepetitionDayOfWeek dayOfWeek)
        {
            return shortNames.TryGetValue(dayOfWeek, out var shortName)
                ? shortName
                : string.Join(", ", dayOfWeek.GetDaysOfWeek().Select(x => shortNames[x]));
        }

        public static IEnumerable<RepetitionDayOfWeek> GetDaysOfWeek(this RepetitionDayOfWeek repetition)
        {
            return daysOfWeek.Where(x => x.IsInRepetition(repetition));
        }

        public static RepetitionDayOfWeek GetCurrentDayOfWeek(this IScheduler scheduler)
        {
            return mapping[scheduler.Now.LocalDateTime.DayOfWeek];
        }
        
        public static RepetitionDayOfWeek GetDayOfWeek(this DateTime dateTime)
        {
            return mapping[dateTime.DayOfWeek];
        }

        private static RepetitionDayOfWeek GetNextDayOfWeek(this RepetitionDayOfWeek dayOfWeek)
        {
            var index = daysOfWeek.BinarySearch(dayOfWeek);
            return daysOfWeek[(index + 1) % 7];
        }
        
        public static bool IsRepetitionToday(this IScheduler scheduler, RepetitionDayOfWeek repetition)
        {
            return scheduler.GetCurrentDayOfWeek().IsInRepetition(repetition);
        }

        public static bool IsInRepetition(this RepetitionDayOfWeek dayOfWeek, RepetitionDayOfWeek repetition)
        {
            return repetition.HasFlag(dayOfWeek);
        }

        public static RepetitionDayOfWeek GetNextRepetitionDayOfWeek(this RepetitionDayOfWeek dayOfWeek, RepetitionDayOfWeek repetition)
        {
            if (repetition == RepetitionDayOfWeek.Empty)
            {
                return RepetitionDayOfWeek.Empty;
            }

            var nextRepetitionDayOfWeek = dayOfWeek.GetNextDayOfWeek();
            while (!IsInRepetition(nextRepetitionDayOfWeek, repetition))
            {
                nextRepetitionDayOfWeek = nextRepetitionDayOfWeek.GetNextDayOfWeek();
            }

            return nextRepetitionDayOfWeek;
        }

        /// <summary>
        /// Distance from one day of week to another in days
        /// </summary>
        public static int DistanceTo(this RepetitionDayOfWeek fromDayOfWeek, RepetitionDayOfWeek toDayOfWeek)
        {
            var fromIndex = daysOfWeek.BinarySearch(fromDayOfWeek);
            var toIndex = daysOfWeek.BinarySearch(toDayOfWeek);
            if (toIndex > fromIndex)
            {
                return toIndex - fromIndex;
            }

            return 7 - fromIndex + toIndex;
        }
    }
}