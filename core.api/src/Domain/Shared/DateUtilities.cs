namespace Domain.Shared;

public static class DateUtilities
{
    /// <summary>
    /// Utility function for parsing date strings from Plaid
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTimeOffset ParseStringAsDate(string date)
    {
        var dateOnly = DateOnly.ParseExact(date, "yyyy-MM-dd");
        var utc = dateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        return new DateTimeOffset(utc);
    }
}