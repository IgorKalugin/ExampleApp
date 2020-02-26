using System;
using System.Linq;

namespace Example.Converters.Medicine
{
    public static class DosageConverters
    {
        public static string Convert(int? value)
        {
            int multiplier = 1000;
            return !value.HasValue ? null : (value < multiplier ? $"{value} mg" : $"{value / multiplier} g");
        }

        public static int? Convert(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var unitString = value.Remove(0, value.IndexOf(' ') + 1);

            var strength = int.Parse(value.Substring(0, value.IndexOf(' ')));
            return string.Compare(unitString, "g", StringComparison.OrdinalIgnoreCase) == 0 ? strength * 1000 : strength;
        }
    }
}