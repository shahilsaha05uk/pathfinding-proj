using UnityEngine.Profiling;

public partial class Controller
{
    public EvaluationData OnNavigate(AlgorithmType algorithmType, int corridorWidth = 1)
    {
        mGrid.ResetPath();

        var (start, end) = mGrid.GetStartEndNodes();
        PathResult result = null;

        Profiler.BeginSample("Pathfinding");
        switch (algorithmType)
        {
            case AlgorithmType.AStar:
                result = pathfindingManager.RunAStar(start, end);
                break;
            case AlgorithmType.GBFS:
                result = pathfindingManager.RunGBFS(start, end);
                break;
            case AlgorithmType.ILS_AStar:
                result = pathfindingManager.RunILS(start, end, ILSAlgorithm.AStar);
                break;
            case AlgorithmType.ILS_GBFS:
                result = pathfindingManager.RunILS(start, end, ILSAlgorithm.GBFS);
                break;
            default:
                result = pathfindingManager.RunAStar(start, end);
                break;
        }

        Profiler.EndSample();
        
        if (result != null)
            mGrid.HighlightPath(result.Path);
        return EvaluationResult.FromPathResult(result);
    }
}