using System;

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
