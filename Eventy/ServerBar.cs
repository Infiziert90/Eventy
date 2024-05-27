using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;

namespace Eventy;

public class ServerBar
{
    private readonly Plugin Plugin;
    private readonly DtrBarEntry? DtrEntry;

    private long LastRefresh = Environment.TickCount64;

    public ServerBar(Plugin plugin)
    {
        Plugin = plugin;

        if (Plugin.DtrBar.Get("Eventy") is not { } entry)
            return;

        DtrEntry = entry;

        DtrEntry.Text = "We all like events...";
        DtrEntry.Shown = false;
        DtrEntry.OnClick += OnClick;

        Plugin.Framework.Update += UpdateDtrBar;
    }

    public void Dispose()
    {
        if (DtrEntry == null)
            return;

        Plugin.Framework.Update -= UpdateDtrBar;
        DtrEntry.OnClick -= OnClick;
        DtrEntry.Dispose();
    }

    public void UpdateDtrBar(IFramework framework)
    {
        if (!Plugin.Configuration.ShowDtrEntry)
        {
            UpdateVisibility(false);
            return;
        }

        // Only refresh every 5s
        if (Environment.TickCount64 + 5000 < LastRefresh)
            return;
        LastRefresh = Environment.TickCount64;

        UpdateVisibility(true);
        UpdateBarString();
    }

    private void UpdateBarString()
    {
        var text = $"{(!Plugin.Configuration.UseShortVersion ? "No Events" : $"{(char) SeIconChar.Clock} 0")}";
        DtrEntry!.Tooltip = null;

        var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        if (Plugin.Events.TryGetValue(date.Ticks, out var events))
        {
            text = $"{(!Plugin.Configuration.UseShortVersion ? "Ongoing Events: " : (char) SeIconChar.Clock)} {events.Length}";

            var tooltip = new SeStringBuilder();
            foreach (var ev in events)
            {
                tooltip.AddText($"{ev.Name}\n");
                tooltip.AddUiForeground($"{ev.Begin:D} - {ev.End:D}", 58);
                tooltip.AddText("\n");
            }

            DtrEntry!.Tooltip = tooltip.BuiltString;
        }

        DtrEntry!.Text = text;
    }

    private void UpdateVisibility(bool shown) => DtrEntry!.Shown = shown;

    private void OnClick()
    {
        Plugin.OpenMain();
    }
}
