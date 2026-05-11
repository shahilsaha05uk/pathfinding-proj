using System;

public class EvaluationLog
{
    public readonly int BatchSize;
    public readonly int GridSize;
    public readonly float ObstacleDensity;
    public readonly float NoiseScale;
    public readonly (float X, float Y) Offsets;
    public readonly int Height;
    public EvaluationLog(
        int batchSize,
        int gridSize, 
        float obstacleDensity, 
        float noiseScale, 
        (float X, float Y) offsets, 
        int height)
    {
        BatchSize = batchSize;
        GridSize = gridSize;
        ObstacleDensity = obstacleDensity;
        NoiseScale = noiseScale;
        Offsets = offsets;
        Height = height;
    }
}
