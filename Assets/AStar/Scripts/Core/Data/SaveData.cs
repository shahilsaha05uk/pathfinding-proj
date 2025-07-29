using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Space(5)][Header("Grid Data")]
    public int GridSize;
    public float ObstacleDensity;

    [Space(5)][Header("Pathfinding Data")]
    public List<EvaluationResult> EvaluationResult;
}