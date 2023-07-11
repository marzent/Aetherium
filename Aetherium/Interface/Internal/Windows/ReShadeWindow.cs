using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Aetherium.Interface.Animation;
using Aetherium.Interface.Animation.EasingFunctions;
using Aetherium.Interface.Colors;
using Aetherium.Interface.Components;
using Aetherium.Interface.Windowing;
using ImGuiNET;
using ReShadeRuntime;

namespace Aetherium.Interface.Internal.Windows;

/// <summary>
/// Component Demo Window to view custom ImGui components.
/// </summary>
internal sealed class ReShadeWindow : Window
{
    private Vector4 defaultColor = ImGuiColors.AetheriumOrange;
    private readonly InterfaceManager interfaceManager = Service<InterfaceManager>.Get();
    private Runtime ReShadeRuntime => interfaceManager.ReShadeRuntime;
    private string shaderPath = "/Users/marc-aurel/Downloads/FXShaders-master/Shaders/SMAA.fx";

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDemoWindow"/> class.
    /// </summary>
    public ReShadeWindow()
        : base("ReShade Runtime")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(700, 300),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.RespectCloseHotkey = false;
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        ImGui.InputText("Shader path", ref shaderPath, 1000);
        if (ImGui.Button("Add"))
        {
            ReShadeRuntime.AddEffect(new FileInfo(shaderPath));
        }
        ImGui.Text("Techniques");
        for (var i = 0; i < ReShadeRuntime.Techniques.Count; i++)
        {
            var technique = ReShadeRuntime.Techniques[i];
            var enabled = technique.Enabled;
            var mouseDragDeltaY = ImGui.GetMouseDragDelta(0).Y;

            ImGui.PushID(i);
            if (ImGui.Checkbox(technique.Name, ref enabled)) 
                technique.Enabled = enabled;
            ImGui.PopID();
            
            if (!ImGui.IsItemActive() || ImGui.IsItemHovered())
                continue;
            
            switch (mouseDragDeltaY)
            {
                case < 0 when i > 0:
                    technique.Priority = i - 1;
                    ReShadeRuntime.Techniques[i - 1].Priority = i;
                    ImGui.ResetMouseDragDelta();
                    break;
                case > 0 when i < ReShadeRuntime.Techniques.Count - 1:
                    technique.Priority = i + 1;
                    ReShadeRuntime.Techniques[i + 1].Priority = i;
                    ImGui.ResetMouseDragDelta();
                    break;
            }
        }
        ReShadeRuntime.OrderTechniques();
    }

    private void ColorPickerWithPaletteDemo()
    {
        ImGui.Text("Click on the color button to use the picker.");
        ImGui.SameLine();
        this.defaultColor = ImGuiComponents.ColorPickerWithPalette(1, "ColorPickerWithPalette Demo", this.defaultColor);
    }
}
