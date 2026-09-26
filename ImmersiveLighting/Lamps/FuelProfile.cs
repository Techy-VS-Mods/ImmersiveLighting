using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ImmersiveLighting.Lamps;

/// <summary>
/// How a fuel burns, read from the fuel item's attributes (patched in by this mod for known fuels):
/// "immersivelighting": { "flameHue": 0-63, "flameSat": 0-7, "luminosity": 0-1.5, "smoke": 0-1 }.
/// Hue and saturation are the game's own light units (hue = 64 steps around the colour wheel, saturation 0-7). Luminosity is how
/// bright the flame is relative to a reference flame (soot glows, so clean spirit flames are dim), smoke is how much soot escapes.
/// A fuel with burn properties but no profile (for example from a mod we do not patch) gets a warm, mildly smoky default.
/// </summary>
public readonly struct FuelProfile
{
    public byte Hue { get; }
    public byte Sat { get; }
    public float Luminosity { get; }
    public float Smoke { get; }

    public FuelProfile(byte hue, byte sat, float luminosity, float smoke)
    {
        Hue = hue; Sat = sat; Luminosity = luminosity; Smoke = smoke;
    }

    public static FuelProfile Default => new FuelProfile(5, 5, 0.9f, 0.2f);

    public static FuelProfile For(ItemStack fuel)
    {
        var d = Default;
        var a = fuel?.ItemAttributes?["immersivelighting"];
        if (a == null || !a.Exists) return d;
        return new FuelProfile(
            (byte)GameMath.Clamp(a["flameHue"].AsInt(d.Hue), 0, 63),
            (byte)GameMath.Clamp(a["flameSat"].AsInt(d.Sat), 0, 7),
            GameMath.Clamp(a["luminosity"].AsFloat(d.Luminosity), 0.05f, 1.5f),
            GameMath.Clamp(a["smoke"].AsFloat(d.Smoke), 0f, 1f));
    }

    /// <summary>Smoke actually produced at a wick height (1-3): turning the wick up burns dirtier, as in a real lamp.</summary>
    public float EffectiveSmoke(int wick) => Smoke * (0.6f + 0.4f * (GameMath.Clamp(wick, 1, 3) - 1) / 2f);
}
