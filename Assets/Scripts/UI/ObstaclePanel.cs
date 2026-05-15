using TMPro;
using UnityEngine;

public class ObstaclePanel : MonoBehaviour
{
    [SerializeField] private Controller Controller;
    public TMP_InputField seed;
    public TMP_InputField min;
    public TMP_InputField max;

    void Start()
    {
    }

    /// <summary>
    /// Get the seed value as an integer
    /// </summary>
    public int GetSeed()
    {
        if (seed == null || string.IsNullOrEmpty(seed.text))
            return 1000; // Default seed
        
        if (int.TryParse(seed.text, out int seedValue))
            return seedValue;
        
        Debug.LogWarning($"Invalid seed value: {seed.text}, using default 1000");
        return 1000;
    }

    /// <summary>
    /// Get the minimum density value, normalized between 0 and 1
    /// </summary>
    public float GetMinDensity()
    {
        if (min == null || string.IsNullOrEmpty(min.text))
            return 0.1f; // Default min

        if (float.TryParse(min.text, out float minValue))
            return Mathf.Clamp01(minValue);

        Debug.LogWarning($"Invalid min value: {min.text}, using default 0.1");
        return 0.1f;
    }

    /// <summary>
    /// Get the maximum density value, normalized between 0 and 1
    /// </summary>
    public float GetMaxDensity()
    {
        if (max == null || string.IsNullOrEmpty(max.text))
            return 0.5f; // Default max

        if (float.TryParse(max.text, out float maxValue))
            return Mathf.Clamp01(maxValue);

        Debug.LogWarning($"Invalid max value: {max.text}, using default 0.5");
        return 0.5f;
    }

    /// <summary>
    /// Get a validated density range with min <= max
    /// </summary>
    public DensityRange GetDensityRange()
    {
        float minDensity = GetMinDensity();
        float maxDensity = GetMaxDensity();

        // Ensure min <= max
        if (minDensity > maxDensity)
        {
            Debug.LogWarning($"Min density ({minDensity}) > Max density ({maxDensity}), swapping values");
            (minDensity, maxDensity) = (maxDensity, minDensity);
        }

        return new DensityRange(minDensity, maxDensity);
    }
}
