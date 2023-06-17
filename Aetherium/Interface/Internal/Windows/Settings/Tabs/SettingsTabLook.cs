using System;
using System.Diagnostics.CodeAnalysis;
using Aetherium.Configuration.Internal;
using Aetherium.Interface.Colors;
using Aetherium.Interface.Internal.Windows.Settings.Widgets;
using CheapLoc;
using ImGuiNET;
using Serilog;

namespace Aetherium.Interface.Internal.Windows.Settings.Tabs;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Internals")]
public class SettingsTabLook : SettingsTab
{
    private float globalUiScale;
    private float fontGamma;

    public override SettingsEntry[] Entries { get; } =
    {
        new GapSettingsEntry(5),

        new GapSettingsEntry(5, true),

        new ButtonSettingsEntry(
            Loc.Localize("AetheriumSettingsOpenStyleEditor", "Open Style Editor"),
            Loc.Localize("AetheriumSettingsStyleEditorHint", "Modify the look & feel of Aetherium windows."),
            () => Service<AetheriumInterface>.Get().OpenStyleEditor()),

        new GapSettingsEntry(5, true),

        new HintSettingsEntry(Loc.Localize("AetheriumSettingToggleUiHideOptOutNote", "Plugins may independently opt out of the settings below.")),
        new GapSettingsEntry(3),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingToggleUiHide", "Hide plugin UI when the game UI is toggled off"),
            Loc.Localize("AetheriumSettingToggleUiHideHint", "Hide any open windows by plugins when toggling the game overlay."),
            c => c.ToggleUiHide,
            (v, c) => c.ToggleUiHide = v),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingToggleUiHideDuringCutscenes", "Hide plugin UI during cutscenes"),
            Loc.Localize("AetheriumSettingToggleUiHideDuringCutscenesHint", "Hide any open windows by plugins during cutscenes."),
            c => c.ToggleUiHideDuringCutscenes,
            (v, c) => c.ToggleUiHideDuringCutscenes = v),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingToggleUiHideDuringGpose", "Hide plugin UI while gpose is active"),
            Loc.Localize("AetheriumSettingToggleUiHideDuringGposeHint", "Hide any open windows by plugins while gpose is active."),
            c => c.ToggleUiHideDuringGpose,
            (v, c) => c.ToggleUiHideDuringGpose = v),

        new GapSettingsEntry(5, true),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingToggleFocusManagement", "Use escape to close Aetherium windows"),
            Loc.Localize("AetheriumSettingToggleFocusManagementHint", "This will cause Aetherium windows to behave like in-game windows when pressing escape.\nThey will close one after another until all are closed. May not work for all plugins."),
            c => c.IsFocusManagementEnabled,
            (v, c) => c.IsFocusManagementEnabled = v),

        // This is applied every frame in InterfaceManager::CheckViewportState()
        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingToggleViewports", "Enable multi-monitor windows"),
            Loc.Localize("AetheriumSettingToggleViewportsHint", "This will allow you move plugin windows onto other monitors.\nWill only work in Borderless Window or Windowed mode."),
            c => !c.IsDisableViewport,
            (v, c) => c.IsDisableViewport = !v),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingToggleDocking", "Enable window docking"),
            Loc.Localize("AetheriumSettingToggleDockingHint", "This will allow you to fuse and tab plugin windows."),
            c => c.IsDocking,
            (v, c) => c.IsDocking = v),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingToggleGamepadNavigation", "Control plugins via gamepad"),
            Loc.Localize("AetheriumSettingToggleGamepadNavigationHint", "This will allow you to toggle between game and plugin navigation via L1+L3.\nToggle the PluginInstaller window via R3 if ImGui navigation is enabled."),
            c => c.IsGamepadNavigationEnabled,
            (v, c) => c.IsGamepadNavigationEnabled = v),

        new SettingsEntry<bool>(
            Loc.Localize("AetheriumSettingToggleTsm", "Show title screen menu"),
            Loc.Localize("AetheriumSettingToggleTsmHint", "This will allow you to access certain Aetherium and Plugin functionality from the title screen."),
            c => c.ShowTsm,
            (v, c) => c.ShowTsm = v),
    };

    public override string Title => Loc.Localize("AetheriumSettingsVisual", "Look & Feel");

    public override void Draw()
    {
        var interfaceManager = Service<InterfaceManager>.Get();

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3);
        ImGui.Text(Loc.Localize("AetheriumSettingsGlobalUiScale", "Global Font Scale"));
        ImGui.SameLine();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3);
        if (ImGui.Button("9.6pt##AetheriumSettingsGlobalUiScaleReset96"))
        {
            this.globalUiScale = 9.6f / 12.0f;
            ImGui.GetIO().FontGlobalScale = this.globalUiScale;
        }

        ImGui.SameLine();
        if (ImGui.Button("12pt##AetheriumSettingsGlobalUiScaleReset12"))
        {
            this.globalUiScale = 1.0f;
            ImGui.GetIO().FontGlobalScale = this.globalUiScale;
        }

        ImGui.SameLine();
        if (ImGui.Button("14pt##AetheriumSettingsGlobalUiScaleReset14"))
        {
            this.globalUiScale = 14.0f / 12.0f;
            ImGui.GetIO().FontGlobalScale = this.globalUiScale;
        }

        ImGui.SameLine();
        if (ImGui.Button("18pt##AetheriumSettingsGlobalUiScaleReset18"))
        {
            this.globalUiScale = 18.0f / 12.0f;
            ImGui.GetIO().FontGlobalScale = this.globalUiScale;
        }

        ImGui.SameLine();
        if (ImGui.Button("24pt##AetheriumSettingsGlobalUiScaleReset24"))
        {
            this.globalUiScale = 24.0f / 12.0f;
            ImGui.GetIO().FontGlobalScale = this.globalUiScale;
        }

        ImGui.SameLine();
        if (ImGui.Button("36pt##AetheriumSettingsGlobalUiScaleReset36"))
        {
            this.globalUiScale = 36.0f / 12.0f;
            ImGui.GetIO().FontGlobalScale = this.globalUiScale;
        }

        var globalUiScaleInPt = 12f * this.globalUiScale;
        if (ImGui.DragFloat("##AetheriumSettingsGlobalUiScaleDrag", ref globalUiScaleInPt, 0.1f, 9.6f, 36f, "%.1fpt", ImGuiSliderFlags.AlwaysClamp))
        {
            this.globalUiScale = globalUiScaleInPt / 12f;
            ImGui.GetIO().FontGlobalScale = this.globalUiScale;
        }

        ImGuiHelpers.SafeTextColoredWrapped(ImGuiColors.AetheriumGrey, Loc.Localize("AetheriumSettingsGlobalUiScaleHint", "Scale text in all XIVLauncher UI elements - this is useful for 4K displays."));

        ImGuiHelpers.ScaledDummy(5);

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3);
        ImGui.Text(Loc.Localize("AetheriumSettingsFontGamma", "Font Gamma"));
        ImGui.SameLine();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3);
        if (ImGui.Button(Loc.Localize("AetheriumSettingsIndividualConfigResetToDefaultValue", "Reset") + "##AetheriumSettingsFontGammaReset"))
        {
            this.fontGamma = 1.4f;
            interfaceManager.FontGammaOverride = this.fontGamma;
        }

        if (ImGui.DragFloat("##AetheriumSettingsFontGammaDrag", ref this.fontGamma, 0.005f, 0.3f, 3f, "%.2f", ImGuiSliderFlags.AlwaysClamp))
        {
            interfaceManager.FontGammaOverride = this.fontGamma;
        }

        ImGuiHelpers.SafeTextColoredWrapped(ImGuiColors.AetheriumGrey, Loc.Localize("AetheriumSettingsFontGammaHint", "Changes the thickness of text."));

        base.Draw();
    }

    public override void Load()
    {
        this.globalUiScale = Service<AetheriumConfiguration>.Get().GlobalUiScale;
        this.fontGamma = Service<AetheriumConfiguration>.Get().FontGammaLevel;

        base.Load();
    }

    public override void Save()
    {
        Service<AetheriumConfiguration>.Get().GlobalUiScale = this.globalUiScale;

        base.Save();
    }
}
