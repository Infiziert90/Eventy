namespace Eventy.Windows.Config;

public partial class ConfigWindow
{
    private void Settings()
    {
        if (ImGui.BeginTabItem("Settings"))
        {
            var changed = false;

            changed |= ImGui.Checkbox("Show Server Bar", ref Plugin.Configuration.ShowDtrEntry);
            changed |= ImGui.Checkbox("Use Short Version", ref Plugin.Configuration.UseShortVersion);

            if (changed)
                Plugin.Configuration.Save();

            ImGui.EndTabItem();
        }
    }
}
