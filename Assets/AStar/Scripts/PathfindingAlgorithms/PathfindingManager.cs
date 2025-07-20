    using System.Collections.Generic;

public class PathfindingManager
{
    private Grid3D mGrid;

    public PathfindingManager(Grid3D grid)
    {
        mGrid = grid;
    }

    public List<Node> RunAStar(Node start, Node end) => AStar.Navigate(start, end);
    public List<Node> RunGBFS(Node start, Node end) => GBFS.Navigate(start, end);

    public List<Node> RunILS(Node start, Node end, int corridorWidth) =>
        ILS.Navigate(mGrid, start, end, corridorWidth);
}