using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Space(5)][Header("Grid Data")]
    public int GridSize;
    public int MaxHeight;
    public float NoiseScale;
    public int ObstacleSeed;

    [Space(5)][Header("Evaluation Data")]
    public int BatchSize;

    [Space(5)][Header("Pathfinding Data")]
    public List<EvaluationResult> EvaluationResult;
}