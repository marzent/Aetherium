using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using Aetherium;
using Aetherium.Configuration.Internal;
using Aetherium.Interface;
using Aetherium.Interface.Animation.EasingFunctions;
using Aetherium.Interface.Colors;
using Aetherium.Interface.ImGuiFileDialog;
using Aetherium.Interface.Internal;
using Aetherium.Interface.Internal.ManagedAsserts;
using Aetherium.Interface.Internal.Windows;
using Aetherium.Interface.Internal.Windows.Settings;
using Aetherium.Interface.Internal.Windows.StyleEditor;
using Aetherium.Interface.Raii;
using Aetherium.Interface.Style;
using Aetherium.Interface.Windowing;
using Aetherium.Logging.Internal;
using ImGuiNET;
using Serilog;
using Serilog.Events;
using Util = Aetherium.Utility.Util;

namespace Aetherium.Interface.Internal;

/// <summary>
/// This plugin implements all of the Aetherium interface separately, to allow for reloading of the interface and rapid prototyping.
/// </summary>
[ServiceManager.EarlyLoadedService]
internal class AetheriumInterface : IDisposable, IServiceType
{
    private const float CreditsDarkeningMaxAlpha = 0.8f;

    private static readonly ModuleLog Log = new("AUI");
    
    private readonly ColorDemoWindow colorDemoWindow;
    private readonly ComponentDemoWindow componentDemoWindow;
    private readonly ConsoleWindow consoleWindow;
    private readonly SettingsWindow settingsWindow;
    private readonly StyleEditorWindow styleEditorWindow;
    private readonly HitchSettingsWindow hitchSettingsWindow;

    private bool isCreditsDarkening = false;
    private OutCubic creditsDarkeningAnimation = new(TimeSpan.FromSeconds(10));

#if DEBUG
    private bool isImGuiDrawDevMenu = true;
#else
    private bool isImGuiDrawDevMenu = false;
#endif

#if BOOT_AGING
        private bool signaledBoot = false;
#endif

    private bool isImGuiDrawDemoWindow = false;
    private bool isImPlotDrawDemoWindow = false;
    private bool isImGuiTestWindowsInMonospace = false;
    private bool isImGuiDrawMetricsWindow = false;

    [ServiceManager.ServiceConstructor]
    private AetheriumInterface(
        Aetherium aetherium,
        AetheriumConfiguration configuration,
        InterfaceManager.InterfaceManagerWithScene interfaceManagerWithScene)
    {
        var interfaceManager = interfaceManagerWithScene.Manager;
        this.WindowSystem = new WindowSystem("AetheriumCore");
        
        this.colorDemoWindow = new ColorDemoWindow() { IsOpen = false };
        this.componentDemoWindow = new ComponentDemoWindow() { IsOpen = false };
        this.consoleWindow = new ConsoleWindow() { IsOpen = configuration.LogOpenAtStartup };
        this.settingsWindow = new SettingsWindow() { IsOpen = false };
        this.styleEditorWindow = new StyleEditorWindow() { IsOpen = false };
        this.hitchSettingsWindow = new HitchSettingsWindow() { IsOpen = false };

        this.WindowSystem.AddWindow(this.colorDemoWindow);
        this.WindowSystem.AddWindow(this.componentDemoWindow);
        this.WindowSystem.AddWindow(this.consoleWindow);
        this.WindowSystem.AddWindow(this.settingsWindow);
        this.WindowSystem.AddWindow(this.styleEditorWindow);
        this.WindowSystem.AddWindow(this.hitchSettingsWindow);

        ImGuiManagedAsserts.AssertsEnabled = configuration.AssertsEnabledAtStartup;
        this.isImGuiDrawDevMenu = this.isImGuiDrawDevMenu || configuration.DevBarOpenAtStartup;

        interfaceManager.Draw += this.OnDraw;

        this.creditsDarkeningAnimation.Point1 = Vector2.Zero;
        this.creditsDarkeningAnimation.Point2 = new Vector2(CreditsDarkeningMaxAlpha);
    }

    /// <summary>
    /// Gets the number of frames since Aetherium has loaded.
    /// </summary>
    public ulong FrameCount { get; private set; }

    /// <summary>
    /// Gets the <see cref="WindowSystem"/> controlling all Aetherium-internal windows.
    /// </summary>
    public WindowSystem WindowSystem { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the /xldev menu is open.
    /// </summary>
    public bool IsDevMenuOpen
    {
        get => this.isImGuiDrawDevMenu;
        set => this.isImGuiDrawDevMenu = value;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Service<InterfaceManager>.Get().Draw -= this.OnDraw;

        this.WindowSystem.RemoveAllWindows();
        
        this.consoleWindow.Dispose();
    }

    #region Open
    

    /// <summary>
    /// Opens the <see cref="ColorDemoWindow"/>.
    /// </summary>
    public void OpenColorsDemoWindow() => this.colorDemoWindow.IsOpen = true;

    /// <summary>
    /// Opens the <see cref="ComponentDemoWindow"/>.
    /// </summary>
    public void OpenComponentDemoWindow() => this.componentDemoWindow.IsOpen = true;

    /// <summary>
    /// Opens the dev menu bar.
    /// </summary>
    public void OpenDevMenu() => this.isImGuiDrawDevMenu = true;

    /// <summary>
    /// Opens the <see cref="ConsoleWindow"/>.
    /// </summary>
    public void OpenLogWindow()
    {
        this.consoleWindow.IsOpen = true;
        this.consoleWindow.BringToFront();
    }

    /// <summary>
    /// Opens the <see cref="SettingsWindow"/>.
    /// </summary>
    public void OpenSettings()
    {
        this.settingsWindow.IsOpen = true;
        this.settingsWindow.BringToFront();
    }

    /// <summary>
    /// Opens the <see cref="StyleEditorWindow"/>.
    /// </summary>
    public void OpenStyleEditor()
    {
        this.styleEditorWindow.IsOpen = true;
        this.styleEditorWindow.BringToFront();
    }

    /// <summary>
    /// Opens the <see cref="HitchSettingsWindow"/>.
    /// </summary>
    public void OpenHitchSettings()
    {
        this.hitchSettingsWindow.IsOpen = true;
        this.hitchSettingsWindow.BringToFront();
    }

    #endregion

    #region Toggle

    /// <summary>
    /// Toggles the <see cref="ColorDemoWindow"/>.
    /// </summary>
    public void ToggleColorsDemoWindow() => this.colorDemoWindow.Toggle();

    /// <summary>
    /// Toggles the <see cref="ComponentDemoWindow"/>.
    /// </summary>
    public void ToggleComponentDemoWindow() => this.componentDemoWindow.Toggle();

    /// <summary>
    /// Toggles the dev menu bar.
    /// </summary>
    public void ToggleDevMenu() => this.isImGuiDrawDevMenu ^= true;

    /// <summary>
    /// Toggles the <see cref="ConsoleWindow"/>.
    /// </summary>
    public void ToggleLogWindow() => this.consoleWindow.Toggle();

    /// <summary>
    /// Toggles the <see cref="SettingsWindow"/>.
    /// </summary>
    public void ToggleSettingsWindow() => this.settingsWindow.Toggle();

    #endregion

    /// <summary>
    /// Toggle the screen darkening effect used for the credits.
    /// </summary>
    /// <param name="status">Whether or not to turn the effect on.</param>
    public void SetCreditsDarkeningAnimation(bool status)
    {
        this.isCreditsDarkening = status;

        if (status)
            this.creditsDarkeningAnimation.Restart();
    }

    private void OnDraw()
    {
        this.FrameCount++;

#if BOOT_AGING
            if (this.frameCount > 500 && !this.signaledBoot)
            {
                this.signaledBoot = true;

                System.Threading.Tasks.Task.Run(async () =>
                {
                    using var client = new System.Net.Http.HttpClient();
                    await client.PostAsync("http://localhost:1415/aging/success", new System.Net.Http.StringContent(string.Empty));
                });
            }
#endif

        try
        {
            this.DrawHiddenDevMenuOpener();
            this.DrawDevMenu();

            this.WindowSystem.Draw();
            
            if (this.isImGuiTestWindowsInMonospace)
                ImGui.PushFont(InterfaceManager.MonoFont);

            if (this.isImGuiDrawDemoWindow)
                ImGui.ShowDemoWindow(ref this.isImGuiDrawDemoWindow);

            if (this.isImGuiDrawMetricsWindow)
                ImGui.ShowMetricsWindow(ref this.isImGuiDrawMetricsWindow);
            
            if (this.isImGuiTestWindowsInMonospace)
                ImGui.PopFont();

            if (this.isCreditsDarkening)
                this.DrawCreditsDarkeningAnimation();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during OnDraw");
        }
    }

    private void DrawCreditsDarkeningAnimation()
    {
        using var style = ImRaii.PushStyle(ImGuiStyleVar.WindowRounding, 0f);

        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(ImGuiHelpers.MainViewport.Size);
        ImGuiHelpers.ForceNextWindowMainViewport();

        this.creditsDarkeningAnimation.Update();
        ImGui.SetNextWindowBgAlpha(Math.Min(this.creditsDarkeningAnimation.EasedPoint.X, CreditsDarkeningMaxAlpha));

        ImGui.Begin(
            "###CreditsDarkenWindow",
            ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoBringToFrontOnFocus |
            ImGuiWindowFlags.NoNav);
        ImGui.End();
    }

    private void DrawHiddenDevMenuOpener()
    {
        if (!this.isImGuiDrawDevMenu)
        {
            using var color = ImRaii.PushColor(ImGuiCol.Button, Vector4.Zero);
            color.Push(ImGuiCol.ButtonActive, Vector4.Zero);
            color.Push(ImGuiCol.ButtonHovered, Vector4.Zero);
            color.Push(ImGuiCol.TextSelectedBg, new Vector4(0, 0, 0, 1));
            color.Push(ImGuiCol.Border, new Vector4(0, 0, 0, 1));
            color.Push(ImGuiCol.BorderShadow, new Vector4(0, 0, 0, 1));
            color.Push(ImGuiCol.WindowBg, new Vector4(0, 0, 0, 1));

            using var style = ImRaii.PushStyle(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            style.Push(ImGuiStyleVar.WindowBorderSize, 0);
            style.Push(ImGuiStyleVar.FrameBorderSize, 0);

            var windowPos = ImGui.GetMainViewport().Pos + new Vector2(20);
            ImGui.SetNextWindowPos(windowPos, ImGuiCond.Always);
            ImGui.SetNextWindowBgAlpha(1);

            if (ImGui.Begin("DevMenu Opener", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings))
            {
                ImGui.SetNextItemWidth(40);
                if (ImGui.Button("###devMenuOpener", new Vector2(20, 20)))
                    this.isImGuiDrawDevMenu = true;

                ImGui.End();
            }

            if (EnvironmentConfiguration.AetheriumForceFishHook)
            {
                ImGui.SetNextWindowPos(windowPos, ImGuiCond.Always);
                ImGui.SetNextWindowBgAlpha(1);

                if (ImGui.Begin(
                        "Disclaimer",
                        ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoBackground |
                        ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove |
                        ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMouseInputs |
                        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings))
                {
                    ImGui.TextColored(ImGuiColors.AetheriumRed, "Is force FishHook!");
                }

                ImGui.End();
            }
        }
    }

    private void DrawDevMenu()
    {
        if (this.isImGuiDrawDevMenu)
        {
            if (ImGui.BeginMainMenuBar())
            {
                var aetherium = Service<Aetherium>.Get();
                var configuration = Service<AetheriumConfiguration>.Get();

                if (ImGui.BeginMenu("Aetherium"))
                {
                    ImGui.MenuItem("Draw dev menu", string.Empty, ref this.isImGuiDrawDevMenu);
                    var devBarAtStartup = configuration.DevBarOpenAtStartup;
                    if (ImGui.MenuItem("Draw dev menu at startup", string.Empty, ref devBarAtStartup))
                    {
                        configuration.DevBarOpenAtStartup ^= true;
                        configuration.QueueSave();
                    }

                    ImGui.Separator();

                    if (ImGui.MenuItem("Open Log window"))
                    {
                        this.OpenLogWindow();
                    }

                    if (ImGui.BeginMenu("Set log level..."))
                    {
                        foreach (var logLevel in Enum.GetValues(typeof(LogEventLevel)).Cast<LogEventLevel>())
                        {
                            if (ImGui.MenuItem(logLevel + "##logLevelSwitch", string.Empty, EntryPoint.LogLevelSwitch.MinimumLevel == logLevel))
                            {
                                EntryPoint.LogLevelSwitch.MinimumLevel = logLevel;
                                configuration.LogLevel = logLevel;
                                configuration.QueueSave();
                            }
                        }

                        ImGui.EndMenu();
                    }

                    var startInfo = Service<AetheriumStartInfo>.Get();

                    var logSynchronously = configuration.LogSynchronously;
                    if (ImGui.MenuItem("Log Synchronously", null, ref logSynchronously))
                    {
                        configuration.LogSynchronously = logSynchronously;
                        configuration.QueueSave();

                        EntryPoint.InitLogging(
                            startInfo.WorkingDirectory!,
                            startInfo.BootShowConsole,
                            configuration.LogSynchronously,
                            startInfo.LogName);
                    }

                    ImGui.Separator();

                    if (ImGui.MenuItem("Open Settings window"))
                    {
                        this.OpenSettings();
                    }

                    if (ImGui.MenuItem("Open Components Demo"))
                    {
                        this.OpenComponentDemoWindow();
                    }

                    if (ImGui.MenuItem("Open Colors Demo"))
                    {
                        this.OpenColorsDemoWindow();
                    }

                    if (ImGui.MenuItem("Open Style Editor"))
                    {
                        this.OpenStyleEditor();
                    }

                    if (ImGui.MenuItem("Open Hitch Settings"))
                    {
                        this.OpenHitchSettings();
                    }

                    ImGui.Separator();

                    if (ImGui.MenuItem("Unload Aetherium"))
                    {
                        Service<Aetherium>.Get().Unload();
                    }

                    if (ImGui.MenuItem("Kill game"))
                    {
                        Process.GetCurrentProcess().Kill();
                    }

                    ImGui.Separator();

                    if (ImGui.MenuItem("Access Violation"))
                    {
                        Marshal.ReadByte(IntPtr.Zero);
                    }

                    if (ImGui.MenuItem("Report crashes at shutdown", null, configuration.ReportShutdownCrashes))
                    {
                        configuration.ReportShutdownCrashes = !configuration.ReportShutdownCrashes;
                        configuration.QueueSave();
                    }

                    ImGui.Separator();

                    ImGui.MenuItem(Util.AssemblyVersion, false);
                    ImGui.MenuItem($"A: {Util.GetGitHash()}[{Util.GetGitCommitCount()}]]", false);
                    ImGui.MenuItem($"CLR: {Environment.Version}", false);

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("GUI"))
                {
                    ImGui.MenuItem("Use Monospace font for following windows", string.Empty, ref this.isImGuiTestWindowsInMonospace);
                    ImGui.MenuItem("Draw ImGui demo", string.Empty, ref this.isImGuiDrawDemoWindow);
                    ImGui.MenuItem("Draw ImPlot demo", string.Empty, ref this.isImPlotDrawDemoWindow);
                    ImGui.MenuItem("Draw metrics", string.Empty, ref this.isImGuiDrawMetricsWindow);

                    ImGui.Separator();

                    var val = ImGuiManagedAsserts.AssertsEnabled;
                    if (ImGui.MenuItem("Enable Asserts", string.Empty, ref val))
                    {
                        ImGuiManagedAsserts.AssertsEnabled = val;
                    }

                    if (ImGui.MenuItem("Enable asserts at startup", null, configuration.AssertsEnabledAtStartup))
                    {
                        configuration.AssertsEnabledAtStartup = !configuration.AssertsEnabledAtStartup;
                        configuration.QueueSave();
                    }

                    if (ImGui.MenuItem("Clear focus"))
                    {
                        ImGui.SetWindowFocus(null);
                    }

                    if (ImGui.MenuItem("Dump style"))
                    {
                        var info = string.Empty;
                        var style = StyleModelV1.Get();
                        var enCulture = new CultureInfo("en-US");

                        foreach (var propertyInfo in typeof(StyleModel).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (propertyInfo.PropertyType == typeof(Vector2))
                            {
                                var vec2 = (Vector2)propertyInfo.GetValue(style);
                                info += $"{propertyInfo.Name} = new Vector2({vec2.X.ToString(enCulture)}f, {vec2.Y.ToString(enCulture)}f),\n";
                            }
                            else
                            {
                                info += $"{propertyInfo.Name} = {propertyInfo.GetValue(style)},\n";
                            }
                        }

                        info += "Colors = new Dictionary<string, Vector4>()\n";
                        info += "{\n";

                        foreach (var color in style.Colors)
                        {
                            info +=
                                $"{{\"{color.Key}\", new Vector4({color.Value.X.ToString(enCulture)}f, {color.Value.Y.ToString(enCulture)}f, {color.Value.Z.ToString(enCulture)}f, {color.Value.W.ToString(enCulture)}f)}},\n";
                        }

                        info += "},";

                        Log.Information(info);
                    }

                    if (ImGui.MenuItem("Show dev bar info", null, configuration.ShowDevBarInfo))
                    {
                        configuration.ShowDevBarInfo = !configuration.ShowDevBarInfo;
                    }

                    ImGui.EndMenu();
                }

                if (configuration.ShowDevBarInfo)
                {
                    ImGui.PushFont(InterfaceManager.MonoFont);
                    
                    ImGui.BeginMenu($"{Util.GetGitHash()}({Util.GetGitCommitCount()})", false);
                    ImGui.BeginMenu(this.FrameCount.ToString("000000"), false);
                    ImGui.BeginMenu(ImGui.GetIO().Framerate.ToString("000"), false);
                    ImGui.BeginMenu($"W:{Util.FormatBytes(GC.GetTotalMemory(false))}", false);
/*
                    var videoMem = Service<InterfaceManager>.Get().GetD3dMemoryInfo();
                    ImGui.BeginMenu(
                        !videoMem.HasValue ? $"V:???" : $"V:{Util.FormatBytes(videoMem.Value.Used)}",
                        false);*/

                    ImGui.PopFont();
                }

                ImGui.EndMainMenuBar();
            }
        }
    }
}
