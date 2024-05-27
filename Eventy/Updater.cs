using System.Net.Http;
using System.Threading.Tasks;

namespace Eventy;

public static class Updater
{
    private const string BaseUrl = "https://xzwnvwjxgmaqtrxewngh.supabase.co/rest/v1/";
    private const string SupabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inh6d252d2p4Z21hcXRyeGV3bmdoIiwicm9sZSI6ImFub24iLCJpYXQiOjE2ODk3NzcwMDIsImV4cCI6MjAwNTM1MzAwMn0.aNYTnhY_Sagi9DyH5Q9tCz9lwaRCYzMC12SZ7q7jZBc";
    private static readonly HttpClient Client = new();

    static Updater()
    {
        Client.DefaultRequestHeaders.Add("apikey", SupabaseAnonKey);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseAnonKey}");
        Client.DefaultRequestHeaders.Add("Prefer", "return=representation");

        Client.Timeout = TimeSpan.FromSeconds(120);
    }

    public static async Task<string> GetEvents(long lastId)
    {
        try
        {
            var response = await Client.GetAsync($"{BaseUrl}Events?id=gt.{lastId}&select=*");
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Plugin.Log.Error(e, "Get events failed");
            return string.Empty;
        }
    }
}
