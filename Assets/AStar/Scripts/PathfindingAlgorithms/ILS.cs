using System;
using System.Collections.Generic;
using UnityEngine;


public static class ILS
{
    public static PathResult Navigate(Grid3D grid, Node start, Node end, int maxCorridorWidth, ILSAlgorithm algorithm)
    {
        var (result, stats) = Stats.RecordStats(() =>
        {
            int currentWidth = 1;
            int maxWidth = maxCorridorWidth;
            int corridorIterations = 1;
            while (currentWidth <= maxWidth)
            {
                var linePoints = GenerateLine(start, end);
                var corridor = DefineCorridor(linePoints, grid, start, end, currentWidth);
                var pathResult = FindPath(start, end, corridor, algorithm);

                if (pathResult != null)
                {
                    return new PathResult
                    {
                        Path = pathResult.Path,
                        PathLength = pathResult.PathLength,
                        PathCost = pathResult.PathCost,
                        CorridorIterations = corridorIterations,
                    };
                }
                
                currentWidth++;
                corridorIterations++;
            }
            Debug.LogWarning("No path found!");
            return new PathResult { Path = null };
        });

        result.TimeTaken = stats.TimeTaken;
        result.MemoryUsage = stats.MemoryUsed;
        return result;
    }
    
    // Step 1: Get the Line from BLA
    private static List<Vector3Int> GenerateLine(Node start, Node end)
    {
        return BLA.GenerateLine(start, end);
    }
    
    // Step 2: Define the corridor
    private static HashSet<Node> DefineCorridor(
        List<Vector3Int> linePoints, 
        Grid3D grid, 
        Node start, Node end,
        int width = 1)
    {
        var corridorNodes = new HashSet<Node>();

        foreach (var point in linePoints)
        {
            var neighbors = grid.GetNeighbors(point, width);
            corridorNodes.UnionWith(neighbors);
        }
        
        corridorNodes.Add(start);
        corridorNodes.Add(end);
        
        return corridorNodes;
    }
    
    private static HashSet<Node> DefineCorridor(
        List<Vector3Int> linePoints, 
        Grid3D grid, 
        Node start, Node end,
        CorridorShape shape,
        int width = 1)
    {
        var corridorNodes = new HashSet<Node>();

        foreach (var point in linePoints)
        {
            var neighbors = grid.GetManhattanRadius(point, width, shape);
            corridorNodes.UnionWith(neighbors);
        }
        
        corridorNodes.Add(start);
        corridorNodes.Add(end);
        
        return corridorNodes;
    }
    
    // Step 3: Pass it to the pathfinding algorithm
    private static PathResult FindPath(Node start, Node end, HashSet<Node> corridor, ILSAlgorithm algorithm)
    {
        switch (algorithm)
        {
            case ILSAlgorithm.AStar:
                return AStar.Navigate(start, end, corridor);
            case ILSAlgorithm.GBFS:
                return GBFS.Navigate(start, end, corridor);
            default:
                return AStar.Navigate(start, end, corridor);
        }
    }
}
