using System;
using UnityEngine;

[Serializable]
public class TerrainData
{
    public TerrainType Type;
    public Color Color;
    public string ColorHex;
    public bool IsBlocked;
    public float HeightMin;
    public float HeightMax;
    public float MovementCost = 1f;
}
