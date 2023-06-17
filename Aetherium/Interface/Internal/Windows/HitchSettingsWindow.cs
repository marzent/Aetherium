using Aetherium.Configuration.Internal;
using Aetherium.Interface.Windowing;
using ImGuiNET;

namespace Aetherium.Interface.Internal.Windows;

/// <summary>
/// Window responsible for hitch settings.
/// </summary>
public class HitchSettingsWindow : Window
{
    private const float MinHitch = 1;
    private const float MaxHitch = 500;

    /// <summary>
    /// Initializes a new instance of the <see cref="HitchSettingsWindow"/> class.
    /// </summary>
    public HitchSettingsWindow()
        : base("Hitch Settings", ImGuiWindowFlags.AlwaysAutoResize)
    {
        this.ShowCloseButton = true;
        this.RespectCloseHotkey = true;
    }
    
    /// <inheritdoc/>
    public override void Draw()
    {
        var config = Service<AetheriumConfiguration>.Get();

        var uiBuilderHitch = (float)config.UiBuilderHitch;
        if (ImGui.SliderFloat("UiBuilderHitch", ref uiBuilderHitch, MinHitch, MaxHitch))
        {
            config.UiBuilderHitch = uiBuilderHitch;
            config.QueueSave();
        }
    }
}
