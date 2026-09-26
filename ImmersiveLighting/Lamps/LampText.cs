using System;
using Vintagestory.API.Config;

namespace ImmersiveLighting.Lamps;

public static class LampText
{
    /// <summary>"under a minute", "45 min", "1 h 10 min": rounded so the number does not jitter.</summary>
    public static string Duration(double seconds)
    {
        int minutes = (int)Math.Round(seconds / 60.0, MidpointRounding.AwayFromZero);
        if (seconds < 60) return Lang.Get("immersivelighting:duration-under");
        if (minutes >= 10) minutes = (int)(Math.Round(seconds / 300.0, MidpointRounding.AwayFromZero) * 5);
        if (minutes < 90) return Lang.Get("immersivelighting:duration-minutes", minutes);
        return Lang.Get("immersivelighting:duration-hours", minutes / 60, minutes % 60);
    }
}
