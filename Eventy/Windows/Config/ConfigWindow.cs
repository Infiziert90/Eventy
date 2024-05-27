using Dalamud.Interface.Windowing;

namespace Eventy.Windows.Config;

public partial class ConfigWindow : Window, IDisposable
{
    public ConfigWindow() : base("Configuration##Eventy")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 200),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public void Dispose() { }

    public override void Draw()
    {
        using var tabBar = ImRaii.TabBar("##ConfigTabBar");
        if (!tabBar.Success)
            return;

        Settings();

        About();
    }
}
