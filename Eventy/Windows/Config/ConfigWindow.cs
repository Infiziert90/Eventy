using Dalamud.Interface.Windowing;

namespace Eventy.Windows.Config;

public partial class ConfigWindow : Window, IDisposable
{
    private readonly Plugin Plugin;

    public ConfigWindow(Plugin plugin) : base("Configuration##Eventy")
    {
        Plugin = plugin;

        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 230),
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
