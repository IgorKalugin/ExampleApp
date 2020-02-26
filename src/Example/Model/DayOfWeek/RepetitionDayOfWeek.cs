using System;

namespace Example.Model.DayOfWeek
{
    [Flags]
    public enum RepetitionDayOfWeek : byte
    {
        Empty = 0,
        Sunday = 1 << 0,
        Monday = 1 << 1,
        Tuesday = 1 << 2,
        Wednesday = 1 << 3,
        Thursday = 1 << 4,
        Friday = 1 << 5,
        Saturday = 1 << 6,
        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
        Weekend = Sunday | Saturday,
        EveryDay = Weekdays | Weekend
    }
}