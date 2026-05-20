using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EvaluatorConfig", menuName = "EvaluatorConfig", order = 1)]
public class SO_EvaluatorConfig : ScriptableObject
{
    [Header("Export Options")]
    public EExportType ExportOption;
    public int NumberOfIterations;

    public List<AlgorithmType> Algorithms;

    public bool SetCustomGridSize;

    // If SetCustomGridSize is true, use this list
    // If SetCustomGridSize is false, auto-generate based on GridSizeCount
    public List<int> GridSizes;

    // Number of grid sizes to generate when SetCustomGridSize is false
    // Generates: 16, 32, 64, 128, etc. (powers of 2 starting from 2^4)
    [Range(1, 10)] public int GridSizeCount = 3;

    // Display only - shows the largest grid size
    [SerializeField] private int lastGridSize;

    [Header("Obstacle Density Ranges")]
    // List of density ranges to evaluate
    // Each range will be randomized and inverted for obstacle generation
    // Lesser value = lesser obstacles
    public List<DensityRange> ObstacleDensityRanges = new();
    [Range(10000, 100000)]public int ObstacleSeedScale;

    // total number of evaluations to run for each grid size and density range
    public int BatchSize = 10;
    public int BaseSeed = 200;

    // Range for noise scale, e.g. 0.1 to 1.0
    [Range(0, 1)] public float NoiseScaleMin;
    [Range(0, 1)] public float NoiseScaleMax;
    public float NoiseScaleMultiplier;

    [Range(0, 1)] public float OffsetXMinRange;
    [Range(0, 1)] public float OffsetXMaxRange;
    [Range(0, 1)] public float OffsetYMinRange;
    [Range(0, 1)] public float OffsetYMaxRange;
    public float OffsetMultiplier;

    [Range(0, 10)] public int HeightRange;
    public float MaxHeightDeviation;

    public bool AnimatePaths = false;
    public float PathAnimationDelay = 0.1f;

    private void OnValidate()
    {
        // Auto-generate grid sizes if not using custom
        if (!SetCustomGridSize)
        {
            GenerateGridSizes();
        }

        // Update the display field
        UpdateLastGridSize();

        // Initialize density ranges if empty
        if (ObstacleDensityRanges == null || ObstacleDensityRanges.Count == 0)
        {
            ObstacleDensityRanges = new List<DensityRange>
            {
                new DensityRange(0.1f, 0.3f),  // Low density range
                new DensityRange(0.4f, 0.6f),  // Medium density range
                new DensityRange(0.7f, 0.9f),  // High density range
            };
        }
    }

    private void GenerateGridSizes()
    {
        if (GridSizes == null)
            GridSizes = new List<int>();

        GridSizes.Clear();

        // Generate grid sizes: 2^4, 2^5, 2^6, etc.
        // For count = 3: 16, 32, 64
        for (int i = 0; i < GridSizeCount; i++)
        {
            int exponent = 4 + i; // Start from 2^4 = 16
            int gridSize = (int)Mathf.Pow(2, exponent);
            GridSizes.Add(gridSize);
        }
    }

    private void UpdateLastGridSize()
    {
        if (GridSizes != null && GridSizes.Count > 0)
        {
            lastGridSize = GridSizes[GridSizes.Count - 1];
        }
        else
        {
            lastGridSize = 0;
        }
    }

    public int GetLastGridSize() => lastGridSize;
}