using System;

public class EvaluationLog
{
    public readonly int BatchSize;
    public readonly int GridSize;
    public readonly int ObstacleSeed;
    public readonly float NoiseScale;
    public readonly (float X, float Y) Offsets;
    public readonly int Height;
    public EvaluationLog(
        int batchSize,
        int gridSize,
        int obstacleSeed, 
        float noiseScale, 
        (float X, float Y) offsets, 
        int height)
    {
        BatchSize = batchSize;
        GridSize = gridSize;
        ObstacleSeed = obstacleSeed;
        NoiseScale = noiseScale;
        Offsets = offsets;
        Height = height;
    }
}
