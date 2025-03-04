namespace Eventy.Windows.Config;

public partial class ConfigWindow
{
    private void Completed()
    {
        using var tabItem = ImRaii.TabItem("Completed");
        if (!tabItem.Success)
            return;

        var changed = false;

        changed |= ImGui.Checkbox("Show Completed Events", ref Plugin.Configuration.ShowCompletedEvents);
        
        var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        if (!Plugin.Events.TryGetValue(date.Ticks, out var events))
        {
            Helper.WrappedTextWithColor(ImGuiColors.DalamudViolet, "No active event at this time.");
            return;
        }

        using var table = ImRaii.Table("EventList", 2, ImGuiTableFlags.BordersInner);
        if (!table.Success)
            return;
        
        ImGui.TableSetupColumn("Event");
        var rowHeaderText = "Completed?";
        var width = ImGui.CalcTextSize(rowHeaderText).X + (ImGui.GetStyle().ItemInnerSpacing.X * 2);
        ImGui.TableSetupColumn(rowHeaderText, ImGuiTableColumnFlags.WidthFixed, width);
        
        ImGui.TableHeadersRow();
        foreach (var ev in events)
        {
            ImGui.TableNextColumn();
            Helper.TextWrapped(ev.Name);
            
            ImGui.TableNextColumn();
            var pos = ImGui.GetCursorPos();
            ImGui.SetCursorPos(pos with { X = pos.X + (ImGui.GetContentRegionAvail().X - ImGui.GetFrameHeight()) * 0.5f });
            
            var isCompleted = Plugin.Configuration.CompletedEvents.Contains(ev.Id);
            if (ImGui.Checkbox($"##{ev.Id}", ref isCompleted))
            {
                changed = true;
                if (isCompleted)
                    Plugin.Configuration.CompletedEvents.Add(ev.Id);
                else
                    Plugin.Configuration.CompletedEvents.Remove(ev.Id);
            }
            
            ImGui.TableNextRow();
        }

        if (changed)
        {
            Plugin.Configuration.Save();
            Plugin.ServerBar.Refresh();
        }
    }
}
