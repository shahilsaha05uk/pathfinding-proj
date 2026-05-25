using System;
using UnityEngine;

[Serializable]
public class DensityRange
{
    [Range(0, 1)] public float Min = 0.1f;
    [Range(0, 1)] public float Max = 0.3f;

    public DensityRange()
    {
    }

    public DensityRange(float min, float max)
    {
        Min = Mathf.Clamp01(min);
        Max = Mathf.Clamp01(max);

        // Ensure min <= max
        if (Min > Max)
            Min = Max;
    }

    /// <summary>
    /// Get a random density value within this range
    /// </summary>
    public float GetRandomDensity()
    {
        return UnityEngine.Random.Range(Min, Max);
    }

    /// <summary>
    /// Inverts the density (1.0 - density) for use as Perlin noise threshold
    /// Lesser density value = lesser obstacles
    /// Greater inverted value = lesser obstacles in Perlin noise
    /// </summary>
    public static float InvertDensity(float density)
    {
        return 1.0f - Mathf.Clamp01(density);
    }
}
