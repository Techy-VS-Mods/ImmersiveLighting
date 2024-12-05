using System;
using System.Collections.Generic;
using System.Linq;

public class FlameColorCalculatorTemp
{
    public static Dictionary<string, string> CodeToMaterialMap { get; set; } = new();
    private static double AdjustTemperature(double baseTemperature, FuelMaterial material)
    {
        // Calculate the midpoint of the material's temperature range
        double midpoint = (material.MinTemperature + material.MaxTemperature) / 2.0;

        // Determine the variance as the difference between the midpoint and base temperature
        double variance = baseTemperature - midpoint;

        // Weight the variance by emission weights (if provided)
        double weightedVariance = variance * material.EmissionWeights?.Average() ?? variance;

        // Adjust the flame temperature by adding the weighted variance
        return baseTemperature + weightedVariance;
    }
    
    private static double EstimateTemperature(double baseTemperature, FuelMaterial material, bool isHighOxygen = false)
    {
        // High oxygen scenarios increase temperature by a percentage
        if (isHighOxygen)
        {
            baseTemperature *= 1.2; // 20% increase for high oxygen
        }

        // Adjust the temperature using material properties
        double adjustedTemperature = AdjustTemperature(baseTemperature, material);

        // Apply a material-specific factor for clean or sooty combustion
        if (material.CleanCombustion)
        {
            adjustedTemperature *= 1.05; // Clean combustion increases temperature slightly
        }
        else
        {
            adjustedTemperature *= 0.95; // Sooty combustion decreases temperature slightly
        }

        return adjustedTemperature;
    }
    
    private static double CalculateWavelength(double temperature)
    {
        // Wien's displacement law (approximate): λ_max = b / T
        const double wienConstant = 2898; // μm * K (approximate)
        return wienConstant / temperature; // Result in nanometers (nm)
    }

    private static byte[] MapWavelengthToHSV(double wavelength)
    {
        // Map wavelength to a visible light hue (approximation)
        double hue = (wavelength < 380 || wavelength > 780) ? 0 : 780 - wavelength;

        // Flames are fully saturated and bright
        double saturation = 1.0;
        double value = 1.0;

        // Convert HSV to byte array
        return new byte[]
        {
            (byte)(hue / 360.0 * 255),
            (byte)(saturation * 255),
            (byte)(value * 255)
        };
    }
    
    public static byte[] CalculateFlameHSVWithMaterial(double baseTemperature, FuelMaterial material, bool isHighOxygen = false)
    {
        // Estimate the adjusted flame temperature
        double adjustedTemperature = EstimateTemperature(baseTemperature, material, isHighOxygen);

        // Calculate the dominant wavelength
        double wavelength = CalculateWavelength(adjustedTemperature);

        // Map the wavelength to HSV
        return MapWavelengthToHSV(wavelength);
    }
    
    private static FuelMaterial? GetFuelMaterial(string input)
     {
         // Check if the input matches an object code in the preloaded dictionary
         if (CodeToMaterialMap.TryGetValue(input, out var materialName))
         {
             // Match based on the material name retrieved from the mapping
             return MaterialRepository.MaterialDatabase()
                 .FirstOrDefault(m => string.Equals(m.Name, materialName, StringComparison.OrdinalIgnoreCase));
         }

         // If no code match, attempt to match the input directly to a material name
         return MaterialRepository.MaterialDatabase().FirstOrDefault(m => string.Equals(m.Name, input, StringComparison.OrdinalIgnoreCase));
     }

    private const double CtoKConversionConstant_AddToC = 273.15;
    public static byte[] CalculateFlameHSV(double baseTemperatureCelsius, string materialNameOrItemCode,
        bool isHighOxygen = false)
    {
        var material = GetFuelMaterial(materialNameOrItemCode);
        return CalculateFlameHSVWithMaterial(baseTemperatureCelsius + CtoKConversionConstant_AddToC, material, isHighOxygen);
    }

}












// using System;
// using System.Collections.Generic;
// using System.Linq;
//
// public class FlameColorCalculator
// {
//     public static Dictionary<string, string> CodeToMaterialMap { get; set; } = new();
//
//     public static byte[] GetFlameColor(string material, CombustionContext context)
//     {
//         var fuel = GetFuelMaterial(material);
//         if (fuel == null)
//             throw new ArgumentException("Material not found.");
//
//         // Estimate temperature based on combustion context
//         double temperature = EstimateTemperature(fuel, context);
//
//         // Calculate blackbody-based color
//         const double wienConstant = 2.897e6; // nm·K
//         double peakWavelength = wienConstant / (temperature + 273.15);
//         var (rBase, gBase, bBase) = WavelengthToRGB(peakWavelength);
//
//         // Add emission contributions
//         var (rEmission, gEmission, bEmission) = SimulateEmissionLines(fuel);
//
//         // Blend blackbody and emission colors
//         double rFinal = context.HighOxygen ? (0.6 * rBase + 0.4 * rEmission) : (0.4 * rBase + 0.6 * rEmission);
//         double gFinal = context.HighOxygen ? (0.6 * gBase + 0.4 * gEmission) : (0.4 * gBase + 0.6 * gEmission);
//         double bFinal = context.HighOxygen ? (0.6 * bBase + 0.4 * bEmission) : (0.4 * bBase + 0.6 * bEmission);
//
//         // Convert to HSV and scale to byte array
//         var (hue, saturation, value) = RGBToHSV(rFinal, gFinal, bFinal);
//         return HSVToByteArray(hue, saturation, value);
//     }
//
//     private static (double R, double G, double B) WavelengthToRGB(double wavelength)
//     {
//         double r = 0, g = 0, b = 0;
//
//         if (wavelength >= 380 && wavelength < 440)
//         {
//             r = -(wavelength - 440) / (440 - 380);
//             g = 0.0;
//             b = 1.0;
//         }
//         else if (wavelength >= 440 && wavelength < 490)
//         {
//             r = 0.0;
//             g = (wavelength - 440) / (490 - 440);
//             b = 1.0;
//         }
//         else if (wavelength >= 490 && wavelength < 510)
//         {
//             r = 0.0;
//             g = 1.0;
//             b = -(wavelength - 510) / (510 - 490);
//         }
//         else if (wavelength >= 510 && wavelength < 580)
//         {
//             r = (wavelength - 510) / (580 - 510);
//             g = 1.0;
//             b = 0.0;
//         }
//         else if (wavelength >= 580 && wavelength < 645)
//         {
//             r = 1.0;
//             g = -(wavelength - 645) / (645 - 580);
//             b = 0.0;
//         }
//         else if (wavelength >= 645 && wavelength <= 750)
//         {
//             r = 1.0;
//             g = 0.0;
//             b = 0.0;
//         }
//
//         // Intensity adjustment for the visible spectrum
//         double factor = 0.0;
//         if (wavelength >= 380 && wavelength < 420)
//         {
//             factor = 0.3 + 0.7 * (wavelength - 380) / (420 - 380);
//         }
//         else if (wavelength >= 420 && wavelength < 645)
//         {
//             factor = 1.0;
//         }
//         else if (wavelength >= 645 && wavelength <= 750)
//         {
//             factor = 0.3 + 0.7 * (750 - wavelength) / (750 - 645);
//         }
//
//         r = Math.Pow(r * factor, 0.8);
//         g = Math.Pow(g * factor, 0.8);
//         b = Math.Pow(b * factor, 0.8);
//
//         return (r, g, b);
//     }
//
//     private static (double Hue, double Saturation, double Value) RGBToHSV(double r, double g, double b)
//     {
//         double max = Math.Max(r, Math.Max(g, b));
//         double min = Math.Min(r, Math.Min(g, b));
//         double delta = max - min;
//
//         double hue = 0.0;
//         if (delta > 0)
//         {
//             if (max == r)
//             {
//                 hue = (g - b) / delta;
//             }
//             else if (max == g)
//             {
//                 hue = 2 + (b - r) / delta;
//             }
//             else if (max == b)
//             {
//                 hue = 4 + (r - g) / delta;
//             }
//
//             hue *= 60;
//             if (hue < 0) hue += 360;
//         }
//
//         double saturation = max == 0 ? 0 : (delta / max);
//         double value = max;
//
//         return (hue, saturation, value);
//     }
//     
//     private static double AdjustTemperature(double flameTemperature, FuelMaterial material)
//     {
//         // Calculate the midpoint of the material's temperature range
//         double midpoint = (material.MinTemperature + material.MaxTemperature) / 2.0;
//
//         // Determine the variance as the difference between the midpoint and flame temperature
//         double variance = flameTemperature - midpoint;
//
//         // Adjust the flame temperature by adding the variance
//         return flameTemperature + variance;
//     }
//     
//     private static double EstimateTemperatureWithMaterial(string input, double baseTemperature, bool isHighOxygen = false)
//     {
//         // Get the material from the database using the input code or name
//         var material = GetFuelMaterial(input);
//
//         if (material == null)
//         {
//             Console.WriteLine("Material not found.");
//             return baseTemperature; // Return base temperature if no material is found
//         }
//
//         // Estimate the adjusted temperature based on material properties
//         double estimatedTemperature = EstimateTemperature(baseTemperature, material, isHighOxygen);
//
//         Console.WriteLine($"Material: {material.Name}, Base Temperature: {baseTemperature}°C, Estimated Temperature: {estimatedTemperature}°C");
//         return estimatedTemperature;
//     }
//
//
//     private static double EstimateTemperature(double baseTemperature, FuelMaterial material, bool isHighOxygen = false)
//     {
//         // High oxygen scenarios increase temperature by a percentage
//         if (isHighOxygen)
//         {
//             baseTemperature *= 1.2; // 20% increase for high oxygen
//         }
//
//         // Adjust the temperature using material properties
//         double adjustedTemperature = AdjustTemperature(baseTemperature, material);
//
//         // Apply a material-specific factor for clean or sooty combustion
//         if (material.CleanCombustion)
//         {
//             adjustedTemperature *= 1.05; // Clean combustion increases temperature slightly
//         }
//         else
//         {
//             adjustedTemperature *= 0.95; // Sooty combustion decreases temperature slightly
//         }
//
//         return adjustedTemperature;
//     }
//
//     private static (double R, double G, double B) SimulateEmissionLines(FuelMaterial fuel)
//     {
//         double r = 0, g = 0, b = 0;
//
//         for (int i = 0; i < fuel.EmissionWavelengths.Length; i++)
//         {
//             var (rLine, gLine, bLine) = WavelengthToRGB(fuel.EmissionWavelengths[i]);
//             r += rLine * fuel.EmissionWeights[i];
//             g += gLine * fuel.EmissionWeights[i];
//             b += bLine * fuel.EmissionWeights[i];
//         }
//
//         double max = Math.Max(r, Math.Max(g, b));
//         if (max > 0)
//         {
//             r /= max;
//             g /= max;
//             b /= max;
//         }
//
//         return (r, g, b);
//     }
//
//
//     private static byte[] HSVToByteArray(double hue, double saturation, double value)
//     {
//         byte hueByte = (byte)(hue / 360.0 * 255);
//         byte saturationByte = (byte)(saturation * 255);
//         byte valueByte = (byte)(value * 255);
//         return new byte[] { hueByte, saturationByte, valueByte };
//     }
//
//     
// }
//
//
