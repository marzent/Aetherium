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
            Loc.Localize("AetheriumSettingsFlash", "Flash FFXIV window on duty pop"),
            Loc.Localize("AetheriumSettingsFlashHint", "Flash the FFXIV window in your task bar when a duty is ready."),
            c => c.DutyFinderTaskbarFlash,
            (v, c) => c.DutyFinderTaskbarFlash = v),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingsDutyFinderMessage", "Chatlog message on duty pop"),
            Loc.Localize("AetheriumSettingsDutyFinderMessageHint", "Send a message in FFXIV chat when a duty is ready."),
            c => c.DutyFinderChatMessage,
            (v, c) => c.DutyFinderChatMessage = v),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingsPrintPluginsWelcomeMsg", "Display loaded plugins in the welcome message"),
            Loc.Localize("AetheriumSettingsPrintPluginsWelcomeMsgHint", "Display loaded plugins in FFXIV chat when logging in with a character."),
            c => c.PrintPluginsWelcomeMsg,
            (v, c) => c.PrintPluginsWelcomeMsg = v),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingsAutoUpdatePlugins", "Auto-update plugins"),
            Loc.Localize("AetheriumSettingsAutoUpdatePluginsMsgHint", "Automatically update plugins when logging in with a character."),
            c => c.AutoUpdatePlugins,
            (v, c) => c.AutoUpdatePlugins = v),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingsSystemMenu", "Aetherium buttons in system menu"),
            Loc.Localize("AetheriumSettingsSystemMenuMsgHint", "Add buttons for Aetherium plugins and settings to the system menu."),
            c => c.DoButtonsSystemMenu,
            (v, c) => c.DoButtonsSystemMenu = v),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingsEnableRmtFiltering", "Enable RMT Filtering"),
            Loc.Localize("AetheriumSettingsEnableRmtFilteringMsgHint", "Enable Aetherium's built-in RMT ad filtering."),
            c => !c.DisableRmtFiltering,
            (v, c) => c.DisableRmtFiltering = !v),

        new GapSettingsEntry(5),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingDoMbCollect", "Anonymously upload market board data"),
            Loc.Localize("AetheriumSettingDoMbCollectHint", "Anonymously provide data about in-game economics to Universalis when browsing the market board. This data can't be tied to you in any way and everyone benefits!"),
            c => c.IsMbCollect,
            (v, c) => c.IsMbCollect = v),
    };

    public override string Title => Loc.Localize("AetheriumSettingsGeneral", "General");
}
