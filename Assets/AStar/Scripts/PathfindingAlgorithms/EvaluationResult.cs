
using System;
using UnityEngine.Serialization;

[Serializable]
public class EvaluationData
{
    public float TimeTaken;
    public float SpaceTaken;
    public int PathLength;
    public float PathCost;
    public int CorridorIterations;
}


[Serializable]
public class EvaluationResult
{
    public EvaluationData AStar;
    public EvaluationData ILSWithAStar;
    public EvaluationData GBFS;
    public EvaluationData ILSWithGBFS;
    
    public static EvaluationData FromPathResult(PathResult result)
    {
        return new EvaluationData
        {
            TimeTaken = result.TimeTaken,
            SpaceTaken = result.SpaceTaken,
            PathLength = result.PathLength,
            PathCost = result.PathCost,
            CorridorIterations = result.CorridorIterations
        };
    }
}