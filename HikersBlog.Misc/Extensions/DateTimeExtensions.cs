namespace HikersBlog.Misc.Extensions;

public static class DateTimeExtensions
{
    const int SECOND = 1;
    const int MINUTE = 60 * SECOND;
    const int HOUR = 60 * MINUTE;
    const int DAY = 24 * HOUR;
    const int MONTH = 30 * DAY;

    public static string ToTimeAgo(this DateTime dt)
    {
        var ts = new TimeSpan(DateTime.Now.Ticks - dt.Ticks);
        double delta = Math.Abs(ts.TotalSeconds);

        if (delta < 1 * MINUTE)
            return ts.Seconds == 1 ? "En sekund sedan" : ts.Seconds + " sekunder sedan";

        if (delta < 2 * MINUTE)
            return "En minut sedan";

        if (delta < 45 * MINUTE)
            return ts.Minutes + " minuter sedan";

        if (delta < 90 * MINUTE)
            return "En timme sedan";

        if (delta < 24 * HOUR)
            return ts.Hours + " timmar sedan";

        if (delta < 48 * HOUR)
            return "Igår";

        if (delta < 30 * DAY)
            return ts.Days + " dagar sedan";

        if (delta < 12 * MONTH)
        {
            int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
            return months <= 1 ? "En månad sedan" : months + " månader sedan";
        }
        else
        {
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "Ett år sedan" : years + " år sedan";
        }
    }
}
