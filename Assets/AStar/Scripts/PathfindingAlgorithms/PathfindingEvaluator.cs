
public class PathfindingEvaluator
{
    private Grid3D mGrid;
    private PathfindingManager mPathManager;
    public PathfindingEvaluator(Grid3D grid, UI ui, PathfindingManager pathManager)
    {
        mPathManager = pathManager;
        mGrid = grid;
    }
    
    public EvaluationResult Evaluate()
    {
        var nodes = mGrid.GetStartEndNodes();

        var aStar = EvaluationResult.FromPathResult(mPathManager.RunAStar(nodes.start, nodes.end));
        var ilsWithAStar = EvaluationResult.FromPathResult(mPathManager.RunILS(nodes.start, nodes.end, ILSAlgorithm.AStar));
        var gbfs = EvaluationResult.FromPathResult(mPathManager.RunGBFS(nodes.start, nodes.end));
        var ilsWithGBFS = EvaluationResult.FromPathResult(mPathManager.RunILS(nodes.start, nodes.end, ILSAlgorithm.GBFS));

        var result = new EvaluationResult
        {
            AStar = aStar,
            ILSWithAStar = ilsWithAStar,
            GBFS = gbfs,
            ILSWithGBFS = ilsWithGBFS,
        };
        return result;
    }
}
