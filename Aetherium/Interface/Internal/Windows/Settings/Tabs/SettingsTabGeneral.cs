using System.Diagnostics.CodeAnalysis;
using Aetherium.Interface.Internal.Windows.Settings.Widgets;
using CheapLoc;

namespace Aetherium.Interface.Internal.Windows.Settings.Tabs;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Internals")]
public class SettingsTabGeneral : SettingsTab
{
    public override SettingsEntry[] Entries { get; } =
    {
        new GapSettingsEntry(5),

        new GapSettingsEntry(5),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingsWaitForPluginsOnStartup", "Wait for plugins before game loads"),
            Loc.Localize("AetheriumSettingsWaitForPluginsOnStartupHint", "Do not let the game load, until plugins are loaded."),
            c => c.IsResumeGameAfterPluginLoad,
            (v, c) => c.IsResumeGameAfterPluginLoad = v),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingsAutoUpdatePlugins", "Auto-update plugins"),
            Loc.Localize("AetheriumSettingsAutoUpdatePluginsMsgHint", "Automatically update plugins when logging in with a character."),
            c => c.AutoUpdatePlugins,
            (v, c) => c.AutoUpdatePlugins = v),
    };

    public override string Title => Loc.Localize("AetheriumSettingsGeneral", "General");
}
