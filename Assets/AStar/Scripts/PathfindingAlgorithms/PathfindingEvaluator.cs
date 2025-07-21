
public class PathfindingEvaluator
{
    private Grid3D mGrid;
    private UI mUI;
    private PathfindingManager mPathManager;
    public PathfindingEvaluator(Grid3D grid, UI ui, PathfindingManager pathManager)
    {
        mPathManager = pathManager;
        mGrid = grid;
        mUI = ui;
    }
    
    public EvaluationResult Evaluate()
    {
        UIHelper.ValidateInputAsInt(mUI.controlPanel.inputMaxCorridorWidth.GetValue(), out int corridorWidth);
        var nodes = mGrid.GetStartEndNodes();

        var aStar = EvaluationResult.FromPathResult(mPathManager.RunAStar(nodes.start, nodes.end));
        var ilsWithAStar = EvaluationResult.FromPathResult(mPathManager.RunILS(nodes.start, nodes.end, corridorWidth, PathfindingAlgorithm.AStar));
        var gbfs = EvaluationResult.FromPathResult(mPathManager.RunGBFS(nodes.start, nodes.end));
        var ilsWithGBFS = EvaluationResult.FromPathResult(mPathManager.RunILS(nodes.start, nodes.end, corridorWidth, PathfindingAlgorithm.GBFS));

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
