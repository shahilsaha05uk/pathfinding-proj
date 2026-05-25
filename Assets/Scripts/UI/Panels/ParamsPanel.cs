using System;
using UnityEngine;

public class ParamsPanel : MonoBehaviour
{
    private int gridSize = 16; // Default grid size
    [Space(5)]
    [SerializeField] private PanelInputField inputGridSize;
    [SerializeField] private PanelInputField inputMaxHeight;
    [SerializeField] private PanelInputField inputMaxHeightDeviation;

    [Space(5)]
    [SerializeField] private PanelMinMax inputOffsetX;
    [SerializeField] private PanelMinMax inputOffsetY;

    [Space(5)]
    public PanelMinMax inputNoise;
    [SerializeField] private PanelInputField noiseMultiplier;

    public ObstaclePanel obstaclePanel;
    public Action<int, bool> OnGridSizeChangedSignature;

    void Start()
    {
        inputGridSize.onValueChanged += OnGridSizeChanged;

        OnGridSizeChanged(inputGridSize.GetValue());
    }

    public GridConfig GetConfig()
    {
        // Validate grid size is set
        if (gridSize <= 0)
        {
            Debug.LogWarning($"Invalid grid size: {gridSize}, using default 16");
            gridSize = 16;
        }

        // Get density range and seed from obstacle panel
        var densityRange = obstaclePanel.GetDensityRange();
        int seedValue = obstaclePanel.GetSeed();
        
        // Calculate 5% deviation of the seed value
        // 5% deviation = seedValue * 0.05
        int seedDeviation = Mathf.Max(1, (int)(seedValue * 0.05f));
        int seedValueRand = UnityEngine.Random.Range(seedValue - seedDeviation, seedValue + seedDeviation);
        
        Debug.Log($"Seed calculation: Base={seedValue}, Deviation={seedDeviation}, Random={seedValueRand}");
        
        // Get random density within the range
        float randomDensity = densityRange.GetRandomDensity();
        
        // Invert the density for threshold
        float invertedDensity = DensityRange.InvertDensity(randomDensity);
        
        // Get validated height
        var height = UIHelper.ValidateInputAsInt(
            inputMaxHeight.GetValue(), out var maxHeight) 
            ? maxHeight : 7;

        // Get validated height deviation from the correct field
        var heightDeviation = UIHelper.ValidateInputAsFloat(
            inputMaxHeightDeviation.GetValue(), out var maxHeightDev) 
            ? maxHeightDev : 0.1f;

        // Get validated noise multiplier
        var noiseScaler = UIHelper.ValidateInputAsFloat(
            noiseMultiplier.GetValue(), out var noiseMult) 
            ? noiseMult : 0.1f;

        // Get noise range (min and max)
        var noiseRange = inputNoise.GetValue();
        float noiseValue = (noiseRange.min + noiseRange.max) / 2f; // Use average of range

        // Validate all critical values
        if (height <= 0)
        {
            Debug.LogWarning($"Invalid height: {height}, using default 7");
            height = 7;
        }

        if (noiseValue <= 0)
        {
            Debug.LogWarning($"Invalid noise value: {noiseValue}, using default 0.1f");
            noiseValue = 0.1f;
        }

        var config = new GridConfig
        {
            GridSize = gridSize,
            MaxHeight = (int)(GridConfigHelper.DeviatedValue(
                height,
                heightDeviation)),
            ObstacleSeed = seedValueRand, // Use seed directly, not multiplied
            DensityThreshold = invertedDensity,
            OffsetX = inputOffsetX.GetValue(),
            OffsetY = inputOffsetY.GetValue(),
            NoiseScale = GridConfigHelper.DeviatedValue(
                noiseValue,
                noiseScaler)
        };

        // Log config for debugging
        Debug.Log($"Grid Config Created: GridSize={config.GridSize}, MaxHeight={config.MaxHeight}, " +
            $"Seed={config.ObstacleSeed}, Threshold={config.DensityThreshold}, NoiseScale={config.NoiseScale}");

        return config;
    }

    private void OnGridSizeChanged(string value)
    {
        if (UIHelper.ValidateInputAsInt(
            inputGridSize.GetValue(),
            out int newGridSize))
        {
            if (newGridSize > 0 && newGridSize % 2 == 0) // Ensure power of 2
            {
                gridSize = newGridSize;
                Debug.Log($"Grid size changed to: {gridSize}");
                OnGridSizeChangedSignature?.Invoke(gridSize, true);
            }
            else
            {
                Debug.LogWarning($"Invalid grid size: {newGridSize}. Must be a positive power of 2.");
            }
        }
        else
        {
            Debug.LogWarning($"Failed to parse grid size from input: {value}");
        }
    }
}
