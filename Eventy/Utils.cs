using Dalamud.Interface.ImGuiNotification;

namespace Eventy;

public enum Subdomain
{
    Eu = 0,
    Na = 1,
    De = 2,
    Fr = 3,
    Jp = 4,
}

public static class Utils
{
    public static IEnumerable<DateTime> EachDay(DateTime start, DateTime end)
    {
        for(var day = start.Date; day.Date <= end.Date; day = day.AddDays(1))
            yield return new DateTime(day.Year, day.Month, day.Day);
    }

    public static void OpenUrl(string url)
    {
        try
        {
            Dalamud.Utility.Util.OpenLink(url);
        }
        catch
        {
            Plugin.Notification.AddNotification(new Notification { Content = "Unable to open the url", Type = NotificationType.Error });
        }
    }
}

public static class Extensions
{
    public static IEnumerable<(T Val, int Idx)> WithIndex<T>(this IEnumerable<T> list)
    {
        return list.Select((val, idx) => (val, idx));
    }

    public static string ToName(this Subdomain subdomain)
    {
        return subdomain switch
        {
            Subdomain.Eu => "EU",
            Subdomain.Na => "NA",
            Subdomain.De => "DE",
            Subdomain.Fr => "FR",
            Subdomain.Jp => "JP",
            _ => "EU",
        };
    }

    public static string ToValue(this Subdomain subdomain)
    {
        return subdomain switch
        {
            Subdomain.Eu => "eu",
            Subdomain.Na => "na",
            Subdomain.De => "de",
            Subdomain.Fr => "fr",
            Subdomain.Jp => "jp",
            _ => "eu",
        };
    }
}

