
public class PathfindingManager
{
    private Grid3D mGrid;

    public PathfindingManager(Grid3D grid)
    {
        mGrid = grid;
    }

    public PathResult RunAStar(Node start, Node end) => AStar.Navigate(start, end);
    public PathResult RunGBFS(Node start, Node end) => GBFS.Navigate(start, end);

    public PathResult RunILS(Node start, Node end, int corridorWidth, PathfindingAlgorithm algorithm) =>
        ILS.Navigate(mGrid, start, end, corridorWidth, algorithm);
}