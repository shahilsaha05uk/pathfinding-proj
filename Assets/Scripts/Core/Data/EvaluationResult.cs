using System.Collections.Generic;
using UnityEngine;

public class EvaluationResult
{
    public Dictionary<AlgorithmType, EvaluationData> Results = new();
    public int ResultCount => Results.Count;

    public void AddResult(AlgorithmType type, EvaluationData data)
    {
        Results[type] = data;
    }

    public static EvaluationData FromPathResult(PathResult result, StatData stats = null)
    {
        if (result == null)
        {
            Debug.LogError("Path result was null!!");
            return new EvaluationData
            {
                Message = "Path result was null!!"
            };
        }

        return new EvaluationData
        {
            PathLength = result.PathLength,
            PathCost = result.PathCost,
            VisitedNodes = result.VisitedNodes,
            CorridorIterations = result.CorridorIterations,
            MaxCorridorWidth = result.MaxCorridorWidth,
            CorridorSize = result.CorridorSize,

            Success = result.Success,
            PeakMemoryBytes = result.PeakedMemoryBytes > 0 ? result.PeakedMemoryBytes : (long)(stats?.PeekedBytes ?? 0),
            MaxOpenListSize = result.MaxOpenListSize,
            MaxClosedListSize = result.MaxClosedListSize,

            TimeTaken = stats?.TimeTaken ?? 0,
            MeasuredTimeMs = stats?.MeasuredTimeMs ?? 0,
            MemoryUsedBytes = stats?.MemoryUsedBytes ?? 0,

            Message = result.Message,
        };
    }
}