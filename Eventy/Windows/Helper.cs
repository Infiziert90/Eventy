using Dalamud.Interface;
using Dalamud.Interface.Components;

namespace Eventy.Windows;

public static class Helper
{
    /// <summary>
    /// An unformatted version for ImGui.TextColored
    /// </summary>
    /// <param name="color">color to be used</param>
    /// <param name="text">text to display</param>
    public static void TextColored(Vector4 color, string text)
    {
        using (ImRaii.PushColor(ImGuiCol.Text, color))
            ImGui.TextUnformatted(text);
    }
    
    /// <summary>
    /// An unformatted version for ImGui.TextWrapped
    /// </summary>
    /// <param name="text">text to display</param>
    public static void TextWrapped(string text)
    {
        using (ImRaii.TextWrapPos(0.0f))
            ImGui.TextUnformatted(text);
    }
    
    /// <summary>
    /// An unformatted version for ImGui.TextWrapped with color
    /// </summary>
    /// <param name="color">color to be used</param>
    /// <param name="text">text to display</param>
    public static void WrappedTextWithColor(Vector4 color, string text)
    {
        using (ImRaii.PushColor(ImGuiCol.Text, color))
            TextWrapped(text);
    }
    
    public static bool Button(string id, FontAwesomeIcon icon, bool disabled)
    {
        var clicked = false;

        using var pushedDisabled = ImRaii.Disabled(disabled);
        if (ImGuiComponents.IconButton(id, icon))
            clicked = true;

        return clicked;
    }

    private static float Saturate(float f) => f < 0.0f ? 0.0f : f > 1.0f ? 1.0f : f;
    private static uint FloatToUintSat(float val) => (uint) ((Saturate(val) * 255.0f) + 0.5f);

    public static uint Vec4ToUintColor(Vector4 i)
    {
        var o = FloatToUintSat(i.X) << 0;
        o |= FloatToUintSat(i.Y) << 8;
        o |= FloatToUintSat(i.Z) << 16;
        o |= FloatToUintSat(i.W) << 24;

        return o;
    }
}
