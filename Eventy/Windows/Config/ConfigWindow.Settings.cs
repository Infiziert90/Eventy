using Dalamud.Interface.Utility;

namespace Eventy.Windows.Config;

public partial class ConfigWindow
{
    private void Settings()
    {
        using var tabItem = ImRaii.TabItem("Settings");
        if (!tabItem.Success)
            return;

        var changed = false;

        changed |= ImGui.Checkbox("Show Server Bar", ref Plugin.Configuration.ShowDtrEntry);
        changed |= ImGui.Checkbox("Use Short Version", ref Plugin.Configuration.UseShortVersion);
        changed |= ImGui.Checkbox("Hide If No Events", ref Plugin.Configuration.HideForZeroEvents);
        changed |= ImGui.Checkbox("Show PvP Seasons", ref Plugin.Configuration.ShowPvP);

        ImGuiHelpers.ScaledDummy(20.0f);

        ImGui.SetNextItemWidth(ImGui.GetWindowWidth() / 5.0f);
        using var combo = ImRaii.Combo("Subdomain To Use", Plugin.Configuration.Subdomain.ToName());
        if (combo.Success)
        {
            foreach (var sub in Enum.GetValues<Subdomain>())
            {
                if (ImGui.Selectable(sub.ToName(), sub == Plugin.Configuration.Subdomain))
                {
                    changed = true;
                    Plugin.Configuration.Subdomain = sub;
                }
            }
        }

        if (changed)
        {
            Plugin.Configuration.Save();
            Plugin.ServerBar.Refresh();
        }
    }
}
