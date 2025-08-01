using System.Collections.Generic;
using UnityEngine;

public class ILS : BasePathfinding
{
    public float blockedThreshold = 0.5f;
    public PathResult Navigate(Grid3D grid, Node start, Node end, int maxCorridorWidth, INavigate algorithm)
    {
        // Record stats for the pathfinding operation
        var (result, stats) = Stats.RecordStats(() =>
        {
            int currentWidth = 1;
            int maxWidth = maxCorridorWidth;
            int corridorIterations = 1;
            var linePoints = GenerateLine(start, end);

            // Pre-check

            // Keep increasing the size of the corridor until a path is found or the maximum width is reached
            while (currentWidth <= maxWidth)
            {
                if(!HasOpenChannel(linePoints, currentWidth, blockedThreshold))
                {
                    currentWidth++;
                    corridorIterations++;
                    Debug.Log($"Early expanded... {currentWidth}");
                    continue;
                }

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
    
    private bool HasOpenChannel(List<Vector3Int> linePoints, int width, float threshold)
    {
        foreach (var point in linePoints)
        {
            var node = Grid3D.Instance.GetNodeAt(point);

            if(node == null) continue;

            // If the node is blocked, we need to check its neighbors
            if (node.bIsBlocked)
            {
                var neighbors = NeighborHelper.GetNeighborsInRange(point, width);
                if (neighbors == null || neighbors.Count == 0) return false; // avoid divide by zero
                int freeCount = 0;

                foreach (var neighbor in neighbors)
                {
                    if (neighbor == null) continue;

                    if (!neighbor.bIsBlocked)
                    {
                        freeCount++;
                        continue;
                    }
                }

                // If all neighbors are blocked, we can expand the corridor width
                float blockedRatio = 1f - (freeCount / (float)neighbors.Count);
                if (blockedRatio >= threshold) return false;
            }
        }
        return true;
    }
}

