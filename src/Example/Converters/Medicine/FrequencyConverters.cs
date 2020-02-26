namespace Example.Converters.Medicine
{
    public static class FrequencyConverters
    {
        public static string Convert(int? value)
        {
            return !value.HasValue ? null : $"every {value}";
        }

        public static int? Convert(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            
            return int.Parse(value.Substring(value.IndexOf(' ') + 1));
        }
    }
}