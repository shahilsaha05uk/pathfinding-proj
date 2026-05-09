using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class BasePathfinding : MonoBehaviour, INavigate
{
    public PathResult Navigate(Node start, Node end, HashSet<Node> allowedNodes = null)
        => FindPath(start, end, allowedNodes) ?? DefaultPath();

    protected virtual (List<Node> path, float totalCost) RetracePath(Node start, Node end)
    {
        List<Node> path = new();
        Node currentNode = end;
        float totalCost = 0f;

        while (currentNode != start)
        {
            path.Add(currentNode);
            totalCost += CalculateHeuristicDistance(currentNode, currentNode.parent);
            currentNode = currentNode.parent;
        }
        path.Add(start);
        path.Reverse();
        return (path, totalCost);
    }

    protected virtual PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
        => DefaultPath();

    protected static PathResult DefaultPath()
        => new()
        {
            Path = null,
            PathLength = 0,
            CorridorIterations = 0,
            PathCost = 0,
            VisitedNodes = 0,
            Message = "No valid path found.",
            Success = false,
        };

    protected virtual PathResult ReturnPath(Node start, Node goal, int visited = 0)
    {
        var (path, totalCost) = RetracePath(start, goal);
        return new PathResult
        {
            Path = path,
            PathLength = path.Count,
            PathCost = totalCost,
            VisitedNodes = visited,
            Success = true,
            Message = $"Path found with length {path.Count} and total cost {totalCost}.",
        };
    }

    protected virtual float CalculateHeuristicDistance(Node a, Node b) => HeuristicHelper.GetEuclideanDistance(a, b);

    protected virtual List<Node> GetAllNeighbors(Node node) => NeighborHelper.GetNeighbors(node);
}
