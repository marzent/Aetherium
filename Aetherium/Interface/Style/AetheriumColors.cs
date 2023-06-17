using System.Numerics;
using System.Text.Json.Serialization;
using Aetherium.Interface.Colors;

namespace Aetherium.Interface.Style;
#pragma warning disable SA1600

public class AetheriumColors
{
    [JsonPropertyName("a")]
    public Vector4? AetheriumBlue { get; set; }

    [JsonPropertyName("b")]
    public Vector4? AetheriumGrey { get; set; }

    [JsonPropertyName("c")]
    public Vector4? AetheriumGrey2 { get; set; }

    [JsonPropertyName("d")]
    public Vector4? AetheriumGrey3 { get; set; }

    [JsonPropertyName("e")]
    public Vector4? AetheriumWhite { get; set; }

    [JsonPropertyName("f")]
    public Vector4? AetheriumWhite2 { get; set; }

    [JsonPropertyName("g")]
    public Vector4? AetheriumOrange { get; set; }

    [JsonPropertyName("h")]
    public Vector4? TankBlue { get; set; }

    [JsonPropertyName("i")]
    public Vector4? HealerGreen { get; set; }

    [JsonPropertyName("j")]
    public Vector4? DPSRed { get; set; }

    [JsonPropertyName("k")]
    public Vector4? AetheriumYellow { get; set; }

    [JsonPropertyName("l")]
    public Vector4? AetheriumViolet { get; set; }

    [JsonPropertyName("m")]
    public Vector4? ParsedGrey { get; set; }

    [JsonPropertyName("n")]
    public Vector4? ParsedGreen { get; set; }

    [JsonPropertyName("o")]
    public Vector4? ParsedBlue { get; set; }

    [JsonPropertyName("p")]
    public Vector4? ParsedPurple { get; set; }

    [JsonPropertyName("q")]
    public Vector4? ParsedOrange { get; set; }

    [JsonPropertyName("r")]
    public Vector4? ParsedPink { get; set; }

    [JsonPropertyName("s")]
    public Vector4? ParsedGold { get; set; }

    public void Apply()
    {
        if (AetheriumBlue.HasValue)
        {
            ImGuiColors.AetheriumBlue = AetheriumBlue.Value;
        }

        if (AetheriumGrey.HasValue)
        {
            ImGuiColors.AetheriumGrey = AetheriumGrey.Value;
        }

        if (AetheriumGrey2.HasValue)
        {
            ImGuiColors.AetheriumGrey2 = AetheriumGrey2.Value;
        }

        if (AetheriumGrey3.HasValue)
        {
            ImGuiColors.AetheriumGrey3 = AetheriumGrey3.Value;
        }

        if (AetheriumWhite.HasValue)
        {
            ImGuiColors.AetheriumWhite = AetheriumWhite.Value;
        }

        if (AetheriumWhite2.HasValue)
        {
            ImGuiColors.AetheriumWhite2 = AetheriumWhite2.Value;
        }

        if (AetheriumOrange.HasValue)
        {
            ImGuiColors.AetheriumOrange = AetheriumOrange.Value;
        }

        if (TankBlue.HasValue)
        {
            ImGuiColors.TankBlue = TankBlue.Value;
        }

        if (HealerGreen.HasValue)
        {
            ImGuiColors.HealerGreen = HealerGreen.Value;
        }

        if (DPSRed.HasValue)
        {
            ImGuiColors.DPSRed = DPSRed.Value;
        }

        if (AetheriumYellow.HasValue)
        {
            ImGuiColors.AetheriumYellow = AetheriumYellow.Value;
        }

        if (AetheriumViolet.HasValue)
        {
            ImGuiColors.AetheriumViolet = AetheriumViolet.Value;
        }

        if (ParsedGrey.HasValue)
        {
            ImGuiColors.ParsedGrey = ParsedGrey.Value;
        }

        if (ParsedGreen.HasValue)
        {
            ImGuiColors.ParsedGreen = ParsedGreen.Value;
        }

        if (ParsedBlue.HasValue)
        {
            ImGuiColors.ParsedBlue = ParsedBlue.Value;
        }

        if (ParsedPurple.HasValue)
        {
            ImGuiColors.ParsedPurple = ParsedPurple.Value;
        }

        if (ParsedOrange.HasValue)
        {
            ImGuiColors.ParsedOrange = ParsedOrange.Value;
        }

        if (ParsedPink.HasValue)
        {
            ImGuiColors.ParsedPink = ParsedPink.Value;
        }

        if (ParsedGold.HasValue)
        {
            ImGuiColors.ParsedGold = ParsedGold.Value;
        }
    }
}

#pragma warning restore SA1600
