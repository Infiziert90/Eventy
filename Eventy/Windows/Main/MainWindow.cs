using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;

namespace Eventy.Windows.Main;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin Plugin;

    private static readonly string[] DayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    private static readonly string[] MonthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    private static readonly int[] NumDaysPerMonth = [31,28,31,30,31,30,31,31,30,31,30,31];

    private static float LongestMonthWidth;
    private static readonly float[] MonthWidths = new float[12];
    private DateTime CurrentDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);

    private readonly uint DarkGrey;
    public readonly Queue<uint> Colors;

    public MainWindow(Plugin plugin) : base("Eventy##Eventy")
    {
        Plugin = plugin;

        Flags = ImGuiWindowFlags.AlwaysAutoResize;

        Colors = new Queue<uint>(
        [
            Helper.Vec4ToUintColor(ImGuiColors.ParsedGreen), Helper.Vec4ToUintColor(ImGuiColors.ParsedBlue),
            Helper.Vec4ToUintColor(ImGuiColors.ParsedPurple), Helper.Vec4ToUintColor(ImGuiColors.ParsedOrange),
            Helper.Vec4ToUintColor(ImGuiColors.ParsedPink), Helper.Vec4ToUintColor(ImGuiColors.ParsedGold),
            Helper.Vec4ToUintColor(ImGuiColors.DPSRed), Helper.Vec4ToUintColor(ImGuiColors.DalamudYellow)
        ]);

        DarkGrey = Helper.Vec4ToUintColor(ImGuiColors.DalamudGrey3);
    }

    public void Dispose() { }

    private Vector2 FieldSize = new(60, 60);
    public override void Draw()
    {
        FieldSize = new Vector2(60, 60) * ImGuiHelpers.GlobalScale;
        var sampleWidth = ImGui.CalcTextSize("00").X;

        if (LongestMonthWidth == 0.0f)
        {
            for (var i = 0; i < 12; i++)
            {
                var mw = ImGui.CalcTextSize(MonthNames[i]).X;

                MonthWidths[i] = mw;
                LongestMonthWidth = Math.Max(LongestMonthWidth, mw);
            }
        }

        var style = ImGui.GetStyle();

        const string arrowLeft = "<";
        const string arrowRight = ">";
        var arrowLeftWidth = ImGui.CalcTextSize(arrowLeft).X;
        var arrowRightWidth = ImGui.CalcTextSize(arrowRight).X;

        var yearString = $"{CurrentDate.Year}";
        var yearPartWidth = arrowLeftWidth + arrowRightWidth + ImGui.CalcTextSize(yearString).X;

        using (ImRaii.PushId(1234))
        {
            if (ImGui.SmallButton(arrowLeft))
                CurrentDate = CurrentDate.AddMonths(-1);

            ImGui.SameLine();

            var color = ImGui.GetColorU32(style.Colors[(int)ImGuiCol.Text]);
            var monthWidth = MonthWidths[CurrentDate.Month - 1];
            var pos = ImGui.GetCursorScreenPos();
            pos = pos with { X = pos.X + ((LongestMonthWidth - monthWidth) * 0.5f) };

            ImGui.GetForegroundDrawList().AddText(pos, color, MonthNames[CurrentDate.Month - 1]);

            ImGui.SameLine(0, LongestMonthWidth + style.ItemSpacing.X * 2);

            if (ImGui.SmallButton(arrowRight))
                CurrentDate = CurrentDate.AddMonths(1);
        }

        ImGui.SameLine(ImGui.GetWindowWidth() - yearPartWidth - style.WindowPadding.X - style.ItemSpacing.X * 4.0f);

        using (ImRaii.PushId(1235))
        {
            if (ImGui.SmallButton(arrowLeft))
                CurrentDate = CurrentDate.AddYears(-1);

            ImGui.SameLine();
            ImGui.Text($"{CurrentDate.Year}");
            ImGui.SameLine();

            if (ImGui.SmallButton(arrowRight))
                CurrentDate = CurrentDate.AddYears(1);
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        var maxDayOfCurMonth = NumDaysPerMonth[CurrentDate.Month - 1];
        if (maxDayOfCurMonth == 28)
        {
            var year = CurrentDate.Year;
            var bis = ((year % 4) == 0) && ((year % 100) != 0 || (year % 400) == 0);
            if (bis)
                maxDayOfCurMonth = 29;
        }

        var drawlist = ImGui.GetWindowDrawList();
        var dayOfWeek = (int)new DateTime(CurrentDate.Year, CurrentDate.Month, 1).DayOfWeek;
        for (var dw = 0; dw < 7; dw++)
        {
            using (ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(0,0)))
            using (ImRaii.Group())
            {
                using var textColor = ImRaii.PushColor(ImGuiCol.Text, CalculateTextColor(), dw == 0);

                ImGui.Text($"{(dw == 0 ? "" : " ")}{DayNames[dw]}");
                if (dw == 0)
                    ImGui.Separator();
                else
                    ImGui.Spacing();

                // Use dayOfWeek for spacing
                var curDay = dw - dayOfWeek;
                for (var row = 0; row < 7; row++)
                {
                    var cday = curDay + (7 * row);
                    if (cday >= 0 && cday < maxDayOfCurMonth)
                    {
                        var day = new DateTime(CurrentDate.Year, CurrentDate.Month, cday + 1, 0, 0, 0);
                        var eventDay = Plugin.Events.TryGetValue(day.Ticks, out var array);

                        var pos = ImGui.GetCursorScreenPos();
                        CreateSquare(dw, row, drawlist, eventDay, array);

                        // using var rowId = ImRaii.PushId(row * 10 + dw);
                        var text = string.Format(cday < 9 ? " {0}" : "{0}", cday + 1);
                        var textWidth = ImGui.CalcTextSize(text).X;
                        var spacing = 5.0f * ImGuiHelpers.GlobalScale;

                        pos = pos with { X = pos.X + spacing + ((sampleWidth - textWidth) * 0.5f) };
                        var color = Helper.Vec4ToUintColor(dw != 0 ? ImGuiColors.DalamudGrey : CalculateTextColor());
                        drawlist.AddText(pos, color, string.Format(cday < 9 ? " {0}" : "{0}", cday + 1));
                    }
                    else
                    {
                        if (cday - dw < maxDayOfCurMonth)
                            CreateSquare(dw, row, drawlist);
                    }
                }

                if (dw == 0)
                    ImGui.Separator();
            }

            if (dw != 6)
                ImGui.SameLine((FieldSize.X * (dw + 1)) + ImGui.GetStyle().ItemSpacing.X);
        }
    }

    private static Vector4 CalculateTextColor()
    {
        var textColor = ImGuiColors.DalamudGrey;
        var l = (textColor.X + textColor.Y + textColor.Z) * 0.33334f;
        return new Vector4(l * 2.0f > 1 ? 1 : l * 2.0f, l * .5f, l * .5f, textColor.W);
    }

    private bool CreateSquare(int row, int col, ImDrawListPtr drawList, bool isEvent = false, ParsedEvent[]? events = null)
    {
        var min = ImGui.GetCursorScreenPos();
        var max = new Vector2(min.X + FieldSize.X, min.Y + FieldSize.Y);
        var size = max - min;

        using var pushedId = ImRaii.PushId($"{row}{col}");
        ImGui.Dummy(size);
        var clicked = ImGui.IsItemClicked(ImGuiMouseButton.Left) || ImGui.IsItemClicked(ImGuiMouseButton.Right);
        var hovered = ImGui.IsItemHovered();

        ImGui.SetCursorScreenPos(min);

        var specialDay = new ParsedEvent();
        if (events != null)
            specialDay = events.FirstOrDefault(ev => ev.Special);

        DrawRect(min, max, 0, !string.IsNullOrEmpty(specialDay.Name) ? specialDay.Color : DarkGrey, drawList);

        if (!string.IsNullOrEmpty(specialDay.Name) && hovered)
        {
            using var textColor = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
            ImGui.SetTooltip(specialDay.Name);
        }

        if (isEvent && events != null)
        {
            foreach (var ev in events.Where(ev => !ev.Special))
            {
                var spacing = ev.Spacing * ImGuiHelpers.GlobalScale;
                var lineMin = min with { Y = min.Y + spacing };
                var lineMax = max with { Y = min.Y + spacing + (5.0f * ImGuiHelpers.GlobalScale) };

                drawList.AddRectFilled(lineMin, lineMax, ev.Color);
                if (ImGui.IsMouseHoveringRect(lineMin, lineMax))
                {
                    using var textColor = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey);
                    ImGui.SetTooltip($"{ev.Name}\n{ev.Begin:f} - {ev.End:f}");
                }
            }
        }

        ImGui.SetCursorScreenPos(min);
        ImGui.Dummy(size);

        return clicked;
    }

    private static void DrawRect(Vector2 min, Vector2 max, uint fillColor, uint borderColor, ImDrawListPtr drawList)
    {
        drawList.AddRectFilled(min, max, fillColor);
        drawList.AddRect(min, max, borderColor);
    }
}
