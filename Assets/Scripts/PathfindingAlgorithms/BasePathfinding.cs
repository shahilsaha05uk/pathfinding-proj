using System.Collections.Generic;
using UnityEngine;

public abstract class BasePathfinding : MonoBehaviour, INavigate
{
    public PathResult Navigate(Node start, Node end, HashSet<Node> allowedNodes = null, bool trackStats = true)
    {
        if (trackStats)
        {
            var (result, stats) = Stats.RecordStats(() => FindPath(start, end, allowedNodes));

            if (result != null)
            {
                result.TimeTaken = stats.TimeTaken;
                return result;
            }
            return null;
        }
        return FindPath(start, end, allowedNodes) ?? null;
    }

    protected virtual (List<Node> path, float totalCost) RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
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

    protected virtual PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null) => null;

    protected virtual PathResult ReturnPath(Node start, Node goal, int visited = 0)
    {
        var (path, totalCost) = RetracePath(start, goal);
        return new PathResult
        {
            Path = path,
            PathLength = path.Count,
            PathCost = totalCost,
            VisitedNodes = visited,
        };
    }

    protected virtual float CalculateHeuristicDistance(Node a, Node b) => HeuristicHelper.GetEuclideanDistance(a, b);

    protected virtual List<Node> GetAllNeighbors(Node node) => NeighborHelper.GetNeighbors(node);
}
