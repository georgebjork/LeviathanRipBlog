namespace LeviathanRipBlog.Helpers;

public static class DateHelpers {
    
    public static DateTime ConvertUTCToLocalTime(this DateTime utcDate, string? timeZone = null) {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDate, TimeZoneInfo.FindSystemTimeZoneById(timeZone ?? "Pacific Standard Time"));
    }
    
    public static string ConvertUtcToFormattedDate(this DateTime utcDateTime)
    {
        // Convert UTC DateTime to local time
        var localDateTime = utcDateTime.ToLocalTime();

        // Format the DateTime as "MM/dd/yyyy"
        return localDateTime.ToString("MM/dd/yyyy");
    }
}
