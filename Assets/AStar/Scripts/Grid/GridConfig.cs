using System;
using UnityEngine;

[Serializable]
public struct GridConfig
{
    public int GridSize;
    public float TileSize;
    public float TileSpacing;
    public Vector3 Offset;
    public float OffsetRandomization;
    public float NoiseScale;
}