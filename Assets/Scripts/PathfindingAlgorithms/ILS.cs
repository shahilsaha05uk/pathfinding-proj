using System.Collections.Generic;
using UnityEngine;

public class ILS : BasePathfinding
{

    public PathResult Navigate(Grid3D grid, Node start, Node end, int maxCorridorWidth, INavigate algorithm) {
        int currentWidth = 1, corridorIterations = 1;
        int maxWidth = maxCorridorWidth;
        var linePoints = BLA.GenerateLine(start, end);

        // Keep increasing the size of the corridor until a path is found or the maximum width is reached
        while (currentWidth <= maxWidth)
        {
            var corridor = DefineCorridor(linePoints, grid, start, end, currentWidth);
            var pathResult = algorithm.Navigate(start, end, corridor);

            if (pathResult.Success)
            {
                return new PathResult
                {
                    Path = pathResult.Path,
                    PathLength = pathResult.PathLength,
                    PathCost = pathResult.PathCost,
                    VisitedNodes = pathResult.VisitedNodes,
                    CorridorIterations = corridorIterations,
                    Success = pathResult.Success,
                    Message = pathResult.Message,
                };
            }
            currentWidth++;
            corridorIterations++;
        }
        return DefaultPath();
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
    
}
