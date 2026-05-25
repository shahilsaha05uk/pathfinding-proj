using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum EExportType
{
    EveryGridSize,
    EveryAlgorithm,
    EveryIteration,
    EveryBatch,
}

[Serializable]
public class AutoEvaluationConfig
{
    public readonly int BatchSize;
    public readonly HashSet<AlgorithmType> Algorithms;
    public readonly HashSet<int> GridSizes;
    public readonly List<DensityRange> DensityRanges;
    public readonly (float Min, float Max) NoiseRange;
    public readonly float NoiseMultiplier;
    public readonly (float Min, float Max) OffsetXRange;
    public readonly (float Min, float Max) OffsetYRange;
    public readonly int HeightRange;
    public readonly float HeightDeviation;
    public readonly float OffsetMultiplier;
    public readonly bool Animate;
    public readonly float AnimationDelay;

    public readonly float ObstacleSeedScale; // Scale factor for seed generation (default: 1000)

    // Evaluator configs
    public readonly EExportType ExportOption;
    public readonly int NumberOfIterations;

    public AutoEvaluationConfig(
        int batchSize,
        HashSet<int> gridSizes,
        List<DensityRange> densityRanges,
        float seedScale,
        (float Min, float Max) noiseRange,
        float noiseMultiplier,
        (float Min, float Max) offsetXRange,
        (float Min, float Max) offsetYRange,
        int heightRange,
        float heightDeviation,
        float offsetMultiplier,
        bool animate,
        float animationDelay,
        HashSet<AlgorithmType> algorithms,
        EExportType exportOption,
        int numberOfIterations)
    {
        BatchSize = batchSize;
        GridSizes = gridSizes;
        DensityRanges = densityRanges;
        Algorithms = algorithms;
        NoiseRange = noiseRange;
        NoiseMultiplier = noiseMultiplier;
        OffsetXRange = offsetXRange;
        OffsetYRange = offsetYRange;
        OffsetMultiplier = offsetMultiplier;
        HeightRange = heightRange;
        HeightDeviation = heightDeviation;
        Animate = animate;
        AnimationDelay = animationDelay;

        ObstacleSeedScale = seedScale; 
        ExportOption = exportOption;
        NumberOfIterations = numberOfIterations;
    }
}
