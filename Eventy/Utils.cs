namespace Eventy;

public static class Utils
{
    public static IEnumerable<DateTime> EachDay(DateTime start, DateTime end)
    {
        for(var day = start.Date; day.Date <= end.Date; day = day.AddDays(1))
            yield return new DateTime(day.Year, day.Month, day.Day);
    }
}

public static class Extensions
{
    public static IEnumerable<(T Val, int Idx)> WithIndex<T>(this IEnumerable<T> list)
    {
        return list.Select((val, idx) => (val, idx));
    }
}

