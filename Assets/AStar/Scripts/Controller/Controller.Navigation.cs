using UnityEngine.Profiling;

public partial class Controller
{
    public EvaluationData OnNavigate(AlgorithmType algorithmType, int corridorWidth = 1)
    {
        mGrid.ResetPath();

        var (start, end) = mGrid.GetStartEndNodes();
        PathResult result = null;

        switch (algorithmType)
        {
            case AlgorithmType.AStar:
                result = pathfindingManager.RunAStar(start, end);
                break;
            case AlgorithmType.GBFS:
                result = pathfindingManager.RunGBFS(start, end);
                break;
            case AlgorithmType.ILS_AStar:
                result = pathfindingManager.RunILSWithAStar(start, end);
                break;
            case AlgorithmType.ILS_GBFS:
                result = pathfindingManager.RunILSWithGBFS(start, end);
                break;
            case AlgorithmType.JPS:
                result = pathfindingManager.RunJPS(start, end);
                break;
            case AlgorithmType.ILS_Dijkstra:
                result = pathfindingManager.RunILSWithDijkstra(start, end);
                break;
            case AlgorithmType.Dijkstra:
                result = pathfindingManager.RunDijkstra(start, end);
                break;
            default:
                result = pathfindingManager.RunAStar(start, end);
                break;
        }
        
        if (result != null)
            mGrid.HighlightPath(result.Path);
        return EvaluationResult.FromPathResult(result);
    }
}