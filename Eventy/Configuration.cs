using Dalamud.Configuration;

namespace Eventy;

public class Configuration : IPluginConfiguration
{
    public int Version { get; set; }

    public bool ShowDtrEntry = true;
    public bool UseShortVersion = false;
    public bool HideForZeroEvents = false;
    public bool ShowPvP = false;

    public Subdomain Subdomain = Subdomain.Eu;

    public bool ShowCompletedEvents = false;
    public HashSet<long> CompletedEvents = [];

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
