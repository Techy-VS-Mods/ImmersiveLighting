namespace ImmersiveLighting;

/// <summary>ModConfig/immersivelighting.json (client side): purely visual settings.</summary>
public class ImmersiveLightingConfig
{
    /// <summary>Multiplier on the smoke rising from lit lamps. 0 turns lamp smoke off.</summary>
    public float SmokeScale { get; set; } = 1f;

    public static ImmersiveLightingConfig Current { get; private set; } = new ImmersiveLightingConfig();

    public static void Load(Vintagestory.API.Common.ICoreAPI api)
    {
        try
        {
            Current = api.LoadModConfig<ImmersiveLightingConfig>("immersivelighting.json") ?? new ImmersiveLightingConfig();
            api.StoreModConfig(Current, "immersivelighting.json");
        }
        catch (System.Exception e)
        {
            api.Logger.Warning("[ImmersiveLighting] could not read immersivelighting.json, using defaults: " + e.Message);
        }
    }
}
