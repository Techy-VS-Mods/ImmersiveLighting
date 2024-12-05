using System;

public class FlameColorCalculatorMaterial
{
    public static byte[] GetFlameColor(string material, CombustionContext context)
    {
        var fuel = GetFuelMaterial(material);
        if (fuel == null)
            throw new ArgumentException("Material not found.");

        // Estimate temperature based on combustion context
        double temperature = EstimateTemperature(fuel, context);

        // Calculate blackbody-based color
        const double wienConstant = 2.897e6; // nm·K
        double peakWavelength = wienConstant / (temperature + 273.15);
        var (rBase, gBase, bBase) = WavelengthToRGB(peakWavelength);

        // Add emission contributions
        var (rEmission, gEmission, bEmission) = SimulateEmissionLines(fuel);

        // Blend blackbody and emission colors
        double rFinal = context.HighOxygen ? (0.6 * rBase + 0.4 * rEmission) : (0.4 * rBase + 0.6 * rEmission);
        double gFinal = context.HighOxygen ? (0.6 * gBase + 0.4 * gEmission) : (0.4 * gBase + 0.6 * gEmission);
        double bFinal = context.HighOxygen ? (0.6 * bBase + 0.4 * bEmission) : (0.4 * bBase + 0.6 * bEmission);

        // Convert to HSV and scale to byte array
        var (hue, saturation, value) = RGBToHSV(rFinal, gFinal, bFinal);
        return HSVToByteArray(hue, saturation, value);
    }

    private static FuelMaterial GetFuelMaterial(string material)
    {
        var materials = MaterialRepository.MaterialDatabase();
        return materials.Find(m => m.Name.Equals(material, StringComparison.OrdinalIgnoreCase));
    }

    private static double EstimateTemperature(FuelMaterial fuel, CombustionContext context)
    {
        return context.HighOxygen ? fuel.MaxTemperature : fuel.MinTemperature;
    }

    private static (double R, double G, double B) SimulateEmissionLines(FuelMaterial fuel)
    {
        double r = 0, g = 0, b = 0;

        for (int i = 0; i < fuel.EmissionWavelengths.Length; i++)
        {
            var (rLine, gLine, bLine) = WavelengthToRGB(fuel.EmissionWavelengths[i]);
            r += rLine * fuel.EmissionWeights[i];
            g += gLine * fuel.EmissionWeights[i];
            b += bLine * fuel.EmissionWeights[i];
        }

        double max = Math.Max(r, Math.Max(g, b));
        if (max > 0)
        {
            r /= max;
            g /= max;
            b /= max;
        }

        return (r, g, b);
    }

    private static (double R, double G, double B) WavelengthToRGB(double wavelength)
    {
        double r = 0, g = 0, b = 0;

        if (wavelength >= 380 && wavelength < 440)
        {
            r = -(wavelength - 440) / (440 - 380);
            g = 0.0;
            b = 1.0;
        }
        else if (wavelength >= 440 && wavelength < 490)
        {
            r = 0.0;
            g = (wavelength - 440) / (490 - 440);
            b = 1.0;
        }
        else if (wavelength >= 490 && wavelength < 510)
        {
            r = 0.0;
            g = 1.0;
            b = -(wavelength - 510) / (510 - 490);
        }
        else if (wavelength >= 510 && wavelength < 580)
        {
            r = (wavelength - 510) / (580 - 510);
            g = 1.0;
            b = 0.0;
        }
        else if (wavelength >= 580 && wavelength < 645)
        {
            r = 1.0;
            g = -(wavelength - 645) / (645 - 580);
            b = 0.0;
        }
        else if (wavelength >= 645 && wavelength <= 750)
        {
            r = 1.0;
            g = 0.0;
            b = 0.0;
        }

        double factor = 0.0;
        if (wavelength >= 380 && wavelength < 420)
        {
            factor = 0.3 + 0.7 * (wavelength - 380) / (420 - 380);
        }
        else if (wavelength >= 420 && wavelength < 645)
        {
            factor = 1.0;
        }
        else if (wavelength >= 645 && wavelength <= 750)
        {
            factor = 0.3 + 0.7 * (750 - wavelength) / (750 - 645);
        }
        else
        {
            factor = 0.3;
        }

        r = Math.Pow(r * factor, 0.8);
        g = Math.Pow(g * factor, 0.8);
        b = Math.Pow(b * factor, 0.8);

        return (r, g, b);
    }

    private static (double Hue, double Saturation, double Value) RGBToHSV(double r, double g, double b)
    {
        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double delta = max - min;

        double hue = 0.0;
        if (delta > 0)
        {
            if (max == r)
            {
                hue = (g - b) / delta;
            }
            else if (max == g)
            {
                hue = 2 + (b - r) / delta;
            }
            else if (max == b)
            {
                hue = 4 + (r - g) / delta;
            }
            hue *= 60;
            if (hue < 0) hue += 360;
        }

        double saturation = max == 0 ? 0 : (delta / max);
        double value = max;

        return (hue, saturation, value);
    }

    private static byte[] HSVToByteArray(double hue, double saturation, double value)
    {
        byte hueByte = (byte)(hue / 360.0 * 255);
        byte saturationByte = (byte)(saturation * 255);
        byte valueByte = (byte)(value * 255);
        return new byte[] { hueByte, saturationByte, valueByte };
    }

}
