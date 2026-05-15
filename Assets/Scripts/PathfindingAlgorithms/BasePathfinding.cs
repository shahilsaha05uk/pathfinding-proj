using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class BasePathfinding : MonoBehaviour, INavigate
{
    // Track peak memory during search - made public so ILS can access it
    public long peakedMemoryDuringSearch = 0;

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
            totalCost += CalculateStepCost(currentNode.parent, currentNode);
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
            Success = 0, // 0 = failure
            MaxOpenListSize = 0,
        };

    protected virtual PathResult ReturnPath(Node start, Node goal, int visited = 0, int maxOpenSize = 0, int maxClosedSize = 0)
    {
        var (path, totalCost) = RetracePath(start, goal);

        // Record current peak memory
        long currentMemory = System.GC.GetTotalMemory(false);
        if (currentMemory > peakedMemoryDuringSearch)
            peakedMemoryDuringSearch = currentMemory;

        return new PathResult
        {
            Path = path,
            PathLength = path.Count,
            PathCost = totalCost,
            VisitedNodes = visited,
            Success = 1, // 1 = success
            Message = $"Path found with length {path.Count} and total cost {totalCost}.",
            MaxOpenListSize = maxOpenSize,
            MaxClosedListSize = maxClosedSize,
            PeakedMemoryBytes = peakedMemoryDuringSearch,
        };
    }

    protected virtual float CalculateHeuristicDistance(Node a, Node b) => HeuristicHelper.GetEuclideanDistance(a, b);

    protected virtual float CalculateStepCost(Node from, Node to)
    {
        if (from == null || to == null) return 0f;
        return CalculateHeuristicDistance(from, to) * to.GetMovementCost();
    }

    protected virtual List<Node> GetAllNeighbors(Node node) => NeighborHelper.GetNeighbors(node);
}
