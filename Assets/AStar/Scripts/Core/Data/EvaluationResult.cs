using UnityEngine;

[SerializeField]
public class EvaluationResult
{
    public EvaluationData AStar;
    public EvaluationData GBFS;
    public EvaluationData JPS;
    public EvaluationData Dijkstra;
    public EvaluationData ILSWithAStar;
    public EvaluationData ILSWithGBFS;
    public EvaluationData ILSWithDijkstra;
    
    public static EvaluationData FromPathResult(PathResult result)
    {
        if(result == null)
        {
            Debug.LogError("Path result was null!!");
            return new EvaluationData();
        }

        return new EvaluationData
        {
            TimeTaken = result.TimeTaken,
            PathLength = result.PathLength,
            PathCost = result.PathCost,
            VisitedNodes = result.VisitedNodes,
            CorridorIterations = result.CorridorIterations
        };
    }
}