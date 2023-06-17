using Aetherium.Interface.FontAwesome;
using ImGuiNET;

namespace Aetherium.Interface.Components;

/// <summary>
/// Class containing various methods providing ImGui components.
/// </summary>
public static partial class ImGuiComponents
{
    /// <summary>
    /// HelpMarker component to add a help icon with text on hover.
    /// </summary>
    /// <param name="helpText">The text to display on hover.</param>
    public static void HelpMarker(string helpText)
    {
        ImGui.SameLine();
        ImGui.TextDisabled(FontAwesomeIcon.InfoCircle.ToIconString());
        if (!ImGui.IsItemHovered()) return;
        ImGui.BeginTooltip();
        ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
        ImGui.TextUnformatted(helpText);
        ImGui.PopTextWrapPos();
        ImGui.EndTooltip();
    }
}
