using System.Collections.Generic;
using System.Linq;

public static class AutoEvaluatorExtensions
{
    public static AutoEvaluationConfig ToConfig(this SO_EvaluatorConfig config)
    {
        return new AutoEvaluationConfig(
            batchSize: config.BatchSize,
            algorithms: new HashSet<AlgorithmType>(config.Algorithms),
            gridSizes: new HashSet<int>(config.GridSizes),
            obstacleDensities: new HashSet<float>(config.ObstacleDensities),
            obstacleDensityDeviation: config.ObstacleDensityDeviation,
            noiseRange: (config.NoiseScaleMin, config.NoiseScaleMax),
            noiseMultiplier: config.NoiseScaleMultiplier,
            offsetXRange: (config.OffsetXMinRange, config.OffsetXMaxRange),
            offsetYRange: (config.OffsetYMinRange, config.OffsetYMaxRange),
            offsetMultiplier: config.OffsetMultiplier,
            heightRange: config.HeightRange,
            heightDeviation: config.MaxHeightDeviation,
            animate: config.AnimatePaths,
            animationDelay: config.PathAnimationDelay);
    }
}