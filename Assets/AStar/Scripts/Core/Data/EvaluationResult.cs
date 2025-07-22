using System;


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
            PathLength = result.PathLength,
            PathCost = result.PathCost,
            CorridorIterations = result.CorridorIterations
        };
    }
}