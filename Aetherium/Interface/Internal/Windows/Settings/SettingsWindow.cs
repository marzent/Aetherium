using System.Linq;
using System.Numerics;
using Aetherium.Configuration.Internal;
using Aetherium.Interface.Colors;
using Aetherium.Interface.FontAwesome;
using Aetherium.Interface.Internal.Windows.Settings.Tabs;
using Aetherium.Interface.Raii;
using Aetherium.Interface.Windowing;
using Aetherium.Utility;
using CheapLoc;
using ImGuiNET;

namespace Aetherium.Interface.Internal.Windows.Settings;

/// <summary>
/// The window that allows for general configuration of Aetherium itself.
/// </summary>
internal class SettingsWindow : Window
{
    private readonly SettingsTab[] tabs =
    {
        new SettingsTabGeneral(),
        new SettingsTabLook(),
    };

    private string searchInput = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
    /// </summary>
    public SettingsWindow()
        : base(Loc.Localize("AetheriumSettingsHeader", "Aetherium Settings") + "###XlSettings2", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar)
    {
        this.Size = new Vector2(740, 550);
        this.SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(752, 610),
            MaximumSize = new Vector2(1780, 940),
        };

        this.SizeCondition = ImGuiCond.FirstUseEver;
    }

    /// <inheritdoc/>
    public override void OnOpen()
    {
        foreach (var settingsTab in this.tabs)
        {
            settingsTab.Load();
        }

        this.searchInput = string.Empty;

        base.OnOpen();
    }

    /// <inheritdoc/>
    public override void OnClose()
    {
        var configuration = Service<AetheriumConfiguration>.Get();
        var interfaceManager = Service<InterfaceManager>.Get();

        var rebuildFont =
            ImGui.GetIO().FontGlobalScale != configuration.GlobalUiScale;

        ImGui.GetIO().FontGlobalScale = configuration.GlobalUiScale;

        foreach (var settingsTab in this.tabs)
        {
            if (settingsTab.IsOpen)
                settingsTab.OnClose();

            settingsTab.IsOpen = false;
        }
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        var windowSize = ImGui.GetWindowSize();

        if (ImGui.BeginTabBar("###settingsTabs"))
        {
            if (string.IsNullOrEmpty(this.searchInput))
            {
                foreach (var settingsTab in this.tabs.Where(x => x.IsVisible))
                {
                    if (ImGui.BeginTabItem(settingsTab.Title))
                    {
                        if (!settingsTab.IsOpen)
                        {
                            settingsTab.IsOpen = true;
                            settingsTab.OnOpen();
                        }

                        if (ImGui.BeginChild($"###settings_scrolling_{settingsTab.Title}", new Vector2(-1, -1), false))
                        {
                            settingsTab.Draw();
                        }

                        ImGui.EndChild();
                        ImGui.EndTabItem();
                    }
                    else if (settingsTab.IsOpen)
                    {
                        settingsTab.IsOpen = false;
                        settingsTab.OnClose();
                    }
                }
            }
            else
            {
                if (ImGui.BeginTabItem("Search Results"))
                {
                    var any = false;

                    foreach (var settingsTab in this.tabs.Where(x => x.IsVisible))
                    {
                        var eligible = settingsTab.Entries.Where(x => !x.Name.IsNullOrEmpty() && x.Name.ToLower().Contains(this.searchInput.ToLower())).ToArray();

                        if (!eligible.Any())
                            continue;

                        any = true;

                        ImGui.TextColored(ImGuiColors.AetheriumGrey, settingsTab.Title);
                        ImGui.Dummy(new Vector2(5));

                        foreach (var settingsTabEntry in eligible)
                        {
                            settingsTabEntry.Draw();
                            ImGuiHelpers.ScaledDummy(3);
                        }

                        ImGui.Separator();

                        ImGui.Dummy(new Vector2(10));
                    }

                    if (!any)
                        ImGui.TextColored(ImGuiColors.AetheriumGrey, "No results found...");

                    ImGui.EndTabItem();
                }
            }
        }

        return;

        ImGui.SetCursorPos(windowSize - ImGuiHelpers.ScaledVector2(70));

        if (ImGui.BeginChild("###settingsFinishButton"))
        {
            using var disabled = ImRaii.Disabled(this.tabs.Any(x => x.Entries.Any(y => !y.IsValid)));

            using (ImRaii.PushStyle(ImGuiStyleVar.FrameRounding, 100f))
            {
                if (ImGui.Button(FontAwesomeIcon.Save.ToIconString(), new Vector2(40)))
                {
                    this.Save();

                    if (!ImGui.IsKeyDown(ImGuiKey.ModShift))
                        this.IsOpen = false;
                }
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(!ImGui.IsKeyDown(ImGuiKey.ModShift)
                                     ? Loc.Localize("AetheriumSettingsSaveAndExit", "Save changes and close")
                                     : Loc.Localize("AetheriumSettingsSaveAndExit", "Save changes"));
            }
        }

        ImGui.EndChild();

        ImGui.SetCursorPos(new Vector2(windowSize.X - 250, ImGui.GetTextLineHeightWithSpacing() + (ImGui.GetStyle().FramePadding.Y * 2)));
        ImGui.SetNextItemWidth(240);
        ImGui.InputTextWithHint("###searchInput", "Search for settings...", ref this.searchInput, 100);
    }

    private void Save()
    {
        var configuration = Service<AetheriumConfiguration>.Get();

        foreach (var settingsTab in this.tabs)
        {
            settingsTab.Save();
        }

        // Apply docking flag
        if (!configuration.IsDocking)
        {
            ImGui.GetIO().ConfigFlags &= ~ImGuiConfigFlags.DockingEnable;
        }
        else
        {
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        }

        // NOTE (Chiv) Toggle gamepad navigation via setting
        if (!configuration.IsGamepadNavigationEnabled)
        {
            ImGui.GetIO().BackendFlags &= ~ImGuiBackendFlags.HasGamepad;
            ImGui.GetIO().ConfigFlags &= ~ImGuiConfigFlags.NavEnableSetMousePos;

            var di = Service<AetheriumInterface>.Get();
        }
        else
        {
            ImGui.GetIO().BackendFlags |= ImGuiBackendFlags.HasGamepad;
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NavEnableSetMousePos;
        }

        configuration.QueueSave();
    }
}
