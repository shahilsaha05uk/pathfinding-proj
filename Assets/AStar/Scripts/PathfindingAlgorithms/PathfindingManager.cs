using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    [SerializeField] private Grid3D mGrid;

    [SerializeField] private AStar aStar;
    [SerializeField] private GBFS gbfs;
    [SerializeField] private Dijkstra dijkstra;
    [SerializeField] private ILS ils;
    [SerializeField] private JPS jps;

    public PathResult RunAStar(Node start, Node end) => aStar.Navigate(start, end);

    public PathResult RunGBFS(Node start, Node end) => gbfs.Navigate(start, end);

    public PathResult RunJPS(Node start, Node end) => jps.Navigate(start, end);

    public PathResult RunDijkstra(Node start, Node end) => dijkstra.Navigate(start, end);

    public PathResult RunILSWithAStar(Node start, Node end, int corridorWidth = 10) =>
        ils.Navigate(mGrid, start, end, corridorWidth, aStar);

    public PathResult RunILSWithGBFS(Node start, Node end, int corridorWidth = 10) =>
        ils.Navigate(mGrid, start, end, corridorWidth, gbfs);

    public PathResult RunILSWithDijkstra(Node start, Node end, int corridorWidth = 10) =>
        ils.Navigate(mGrid, start, end, corridorWidth, dijkstra);
}