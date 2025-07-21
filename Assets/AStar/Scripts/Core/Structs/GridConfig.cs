using System;
using UnityEngine;

[Serializable]
public struct GridConfig
{
    public int GridSize;
    public int MaxHeight;
    public float TileSize;
    public float TileSpacing;
    public Vector3 Offset;
    public float ObstacleDensity;
    public float OffsetRandomization;
    public float NoiseScale;
}