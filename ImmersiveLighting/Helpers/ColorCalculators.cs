using System;

namespace ImmersiveLighting.Helpers;

public static class ColorCalculators
{
    public const double WIEN_CONSTANT = 0.0029; // meters Kelvin

    public static double GetWavelengthFromCelsius(double celsius)
    {
        return GetWavelengthFromKelvin(celsius + -273.15d);
    }

    public static double GetWavelengthFromKelvin(double temperatureKelvin)
    {
        var waveLengthMeters = (temperatureKelvin / WIEN_CONSTANT ); // Meters
        return Math.Pow(waveLengthMeters, -9); // Nanometers
    }

    // Reference https://gist.github.com/friendly/67a7df339aa999e2bcfcfec88311abfc
    // Note: Only calculates Wavelengths between 380 and 750 due to human visual range
    public static byte[] GetRGBFromWavelength(double wavelength, double gamma = 0.8)
    {
        double R;
        double G;
        double B;
        switch  (wavelength)
        {
            case >= 380 and <= 440:
            {
                var attenuation = 0.3 + 0.7 * (wavelength - 380) / (440 - 380);
                R = Math.Pow(((-(wavelength - 440) / (440 - 380)) * attenuation), gamma);
                G = 0.0;
                B = Math.Pow((1.0 * attenuation), gamma);
                break;
            }
            case >= 440 and <= 490:
            {
                R = 0.0;
                G = Math.Pow((wavelength - 440) / (490-440), gamma);
                B = 1.0;
                break;
            }
            case >= 490 and <= 510:
            {
                R = 0.0;
                G = 1.0;
                B = Math.Pow((-(wavelength - 510) / (510 - 490)), gamma);
                break;
            }
            case >= 510 and <= 580:
            {
                R = Math.Pow(((wavelength - 510) / (580 - 510)), gamma);
                G = 1.0;
                B = 0.0;
                break;
            }
            case >= 580 and <= 645:
            {
                R = 1.0;
                G = Math.Pow((-(wavelength - 645) / (645 - 580)) , gamma);
                B = 0.0;
                break;
            }
            case >= 645 and <= 750:
            {
                var attenuation = 0.3 + 0.7 * (750 - wavelength) / (750 - 645);
                R = Math.Pow((1.0 * attenuation), gamma);
                G = 0.0;
                B = 0.0;
                break;
            }
            default:
            {
                R = 0.0;
                G = 0.0;
                B = 0.0;
                break;
            }
        }
        R *= 255;
        G *= 255;
        B *= 255;
        return new [] { (byte)(int) Math.Floor(R), (byte)(int) Math.Floor(G), (byte)(int) Math.Floor(B) };
    }

    public static byte[] GetRGBFromCelsius(double celsius)
    {
        return GetRGBFromWavelength(GetWavelengthFromCelsius(celsius));
    }
    
}