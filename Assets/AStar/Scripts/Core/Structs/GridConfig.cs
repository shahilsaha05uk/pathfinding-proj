using JetBrains.Annotations;
using System;

[Serializable]
public class GridData
{
    public int GridSize;
    public int MaxHeight;
    public float NoiseScale;
    public float ObstacleDensity;
}

[Serializable]
public class GridConfig
{
    public int GridSize;
    public int MaxHeight;
    public float NoiseScale;
    public float ObstacleDensity;
    public (float min, float max) OffsetX;
    public (float min, float max) OffsetY;
}