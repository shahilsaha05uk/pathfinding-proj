using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Space(5)][Header("Grid Data")]
    public int GridSize;
    public int MaxHeight;
    public Vector3 Offset;
    public float OffsetRandomization;
    public float NoiseScale;
    public float ObstacleDensity;

    [Space(5)][Header("Pathfinding Data")]
    public List<EvaluationResult> EvaluationResult;
}