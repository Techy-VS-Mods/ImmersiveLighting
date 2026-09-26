using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.MathTools;

namespace ImmersiveLighting;

/// <summary>
/// Server-wide tuning, read from ModConfig/immersivelighting-server.json on the server. The values are published into the world
/// config, which the game sends to every client on join, so the server and the clients always agree (brightness in particular
/// must match on both sides because each computes the lamp's light).
/// </summary>
public class ImmersiveLightingServerConfig
{
    /// <summary>Multiplier on how fast lamps burn fuel. 2 = twice as fast, 0.5 = half as fast.</summary>
    public float BurnRateScale { get; set; } = 1f;
    /// <summary>Multiplier on every lamp's brightness.</summary>
    public float BrightnessScale { get; set; } = 1f;
    /// <summary>Multiplier on lamp smoke (clients can also scale it down with their own SmokeScale). 0 = no smoke for anyone.</summary>
    public float SmokeScale { get; set; } = 1f;
    /// <summary>If true, a lamp must be lit with an ignition source (firestarter, lit torch, ...). Douse and wick changes stay bare-handed.</summary>
    public bool RequireIgnitionSource { get; set; } = true;
}

public static class LampSettings
{
    const string Prefix = "immersivelighting_";

    public static void LoadAndPublish(ICoreServerAPI api)
    {
        var cfg = new ImmersiveLightingServerConfig();
        try
        {
            cfg = api.LoadModConfig<ImmersiveLightingServerConfig>("immersivelighting-server.json") ?? cfg;
            api.StoreModConfig(cfg, "immersivelighting-server.json");
        }
        catch (System.Exception e)
        {
            api.Logger.Warning("[ImmersiveLighting] could not read immersivelighting-server.json, using defaults: " + e.Message);
        }
        var w = api.World.Config;
        w.SetFloat(Prefix + "burnRate", GameMath.Clamp(cfg.BurnRateScale, 0.1f, 10f));
        w.SetFloat(Prefix + "brightness", GameMath.Clamp(cfg.BrightnessScale, 0f, 3f));
        w.SetFloat(Prefix + "smoke", GameMath.Clamp(cfg.SmokeScale, 0f, 3f));
        w.SetBool(Prefix + "requireIgnition", cfg.RequireIgnitionSource);
        api.Logger.Notification("[ImmersiveLighting] burn rate x{0}, brightness x{1}, smoke x{2}, ignition source required: {3}",
            w.GetFloat(Prefix + "burnRate"), w.GetFloat(Prefix + "brightness"), w.GetFloat(Prefix + "smoke"), cfg.RequireIgnitionSource);
    }

    public static float BurnRate(IWorldAccessor world) => world?.Config?.GetFloat(Prefix + "burnRate", 1f) ?? 1f;
    public static float Brightness(IWorldAccessor world) => world?.Config?.GetFloat(Prefix + "brightness", 1f) ?? 1f;
    public static float Smoke(IWorldAccessor world) => world?.Config?.GetFloat(Prefix + "smoke", 1f) ?? 1f;
    public static bool RequireIgnition(IWorldAccessor world) => world?.Config?.GetBool(Prefix + "requireIgnition", true) ?? true;
}
