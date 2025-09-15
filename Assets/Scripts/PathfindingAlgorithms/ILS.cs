using System.Collections.Generic;
using UnityEngine;

public class ILS : BasePathfinding
{
    public bool debugCorridor;
    public Color debugColor;

    public PathResult Navigate(Grid3D grid, Node start, Node end, int maxCorridorWidth, INavigate algorithm) {
        // Record stats for the pathfinding operation
        var (result, stats) = Stats.RecordStats(() => {
            int currentWidth = 1, corridorIterations = 1;
            int maxWidth = maxCorridorWidth;
            var linePoints = GenerateLine(start, end);

            // Keep increasing the size of the corridor until a path is found or the maximum width is reached
            while (currentWidth <= maxWidth) {
                var corridor = DefineCorridor(linePoints, grid, start, end, currentWidth);
                ColorCorridor(corridor);
                var pathResult = algorithm.Navigate(start, end, corridor, false);

                if (pathResult != null) {
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

    private void ColorCorridor(HashSet<Node> nodes)
    {
        if (!debugCorridor) return;

        foreach (var n in nodes)
        {
            n.SetColor(debugColor);
        }
    }
    
}
