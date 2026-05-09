using UnityEngine;

public class EvaluationResult
{
    public EvaluationData AStar;
    public EvaluationData GBFS;
    public EvaluationData JPS;
    public EvaluationData Dijkstra;
    public EvaluationData ILSWithAStar;
    public EvaluationData ILSWithGBFS;
    public EvaluationData ILSWithDijkstra;
    
    public static EvaluationData FromPathResult(
        PathResult result, 
        StatData stats = null)
    {
        if(result == null)
        {
            Debug.LogError("Path result was null!!");
            return new EvaluationData();
        }

        return new EvaluationData
        {
            PathLength = result.PathLength,
            PathCost = result.PathCost,
            VisitedNodes = result.VisitedNodes,
            CorridorIterations = result.CorridorIterations,

            // Stats
            TimeTaken = stats?.TimeTaken ?? 0,
            MeasuredTimeMs = stats?.MeasuredTimeMs ?? 0,
            MemoryUsedBytes = stats?.MemoryUsedBytes ?? 0,

            Message = stats != null 
                ? "Stats collected successfully." 
                : "Stats wasnt recorded!!"
        };
    }
}