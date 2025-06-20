namespace JournIQ.UI
{
    public static class DateExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek start)
        {
            int diff = (7 + (dt.DayOfWeek - start)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
