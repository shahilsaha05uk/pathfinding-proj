using System;

[Serializable]
public class GridConfig
{
    public int GridSize;
    public int MaxHeight;
    public float NoiseScale;
    public int ObstacleSeed;
    public float DensityThreshold; // Inverted density for Perlin noise threshold
    public (float min, float max) OffsetX;
    public (float min, float max) OffsetY;
}
