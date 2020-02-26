namespace Example.Converters.Medicine
{
    public static class MedicineCountConverters
    {
        public static string Convert(int? value)
        {
            if (!value.HasValue)
            {
                return null;
            }
            
            return $"{value} pill" + (value > 1 ? "s" : string.Empty);
        }

        public static int? Convert(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            
            return int.Parse(value.Substring(0, 1));
        }
    }
}