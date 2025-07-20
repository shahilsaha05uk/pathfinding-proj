
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
        UIHelper.ValidateInputAsInt(mUI.inputMaxCorridorWidth.GetValue(), out int corridorWidth);
        var result = new EvaluationResult();
        var nodes = mGrid.GetStartEndNodes();

        // TODO: These should also return the traversal nodes count and space taken
        var stats_ils_aStar = Stats.TimedStats(() =>
        {
            mPathManager.RunILS(nodes.start, nodes.end, corridorWidth);
        });
        
        var stats_gbfs = Stats.TimedStats(() =>
        {
            mPathManager.RunGBFS(nodes.start, nodes.end);
        });
        
        var stats_aStar = Stats.TimedStats(() =>
        {
            mPathManager.RunAStar(nodes.start, nodes.end);
        });
        
        result.ILSWithAStarTime = stats_ils_aStar;
        result.GBFSTime = stats_gbfs;
        result.AStarTime = stats_aStar;

        return result;
    }
}
