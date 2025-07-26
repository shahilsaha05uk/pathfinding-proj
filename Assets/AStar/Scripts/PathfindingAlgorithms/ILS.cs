using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;


public class ILS : BasePathfinding
{
    public PathResult Navigate<T>(Grid3D grid, Node start, Node end, int maxCorridorWidth, T algorithm) where T : BasePathfinding
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
                var pathResult = algorithm.Navigate(start, end, corridor, false);

                if (pathResult != null)
                {
                    return new PathResult
                    {
                        Path = pathResult.Path,
                        PathLength = pathResult.PathLength,
                        PathCost = pathResult.PathCost,
                        VisitedNodes = pathResult.VisitedNodes,
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
        return result;
    }
    
    // Step 1: Get the Line from BLA
    private List<Vector3Int> GenerateLine(Node start, Node end)
    {
        return BLA.GenerateLine(start, end);
    }
    
    // Step 2: Define the corridor
    private HashSet<Node> DefineCorridor(
        List<Vector3Int> linePoints, 
        Grid3D grid, 
        Node start, Node end,
        int width = 1)
    {
        var corridorNodes = new HashSet<Node>();

        foreach (var point in linePoints)
        {
            var neighbors = NeighborHelper.GetNeighborsInRange(point, width);
            corridorNodes.UnionWith(neighbors);
        }
        
        corridorNodes.Add(start);
        corridorNodes.Add(end);
        
        return corridorNodes;
    }
    
    private HashSet<Node> DefineCorridor(
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
    
}
