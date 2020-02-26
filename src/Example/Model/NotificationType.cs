using System;

namespace Example.Model
{
    [Flags]
    public enum NotificationType
    {
        Individual = 1 << 0,
        Contiguous = 1 << 1,
        Proximity = 1 << 2,
    }
}