using System.Collections.Generic;
using UnityEngine;

public class ILS : BasePathfinding
{
    // Track peak memory across all iterations
    private long ilsPeakedMemory = 0;

    public PathResult Navigate(Grid3D grid, Node start, Node end, INavigate algorithm)
    {
        int currentWidth = 1, corridorIterations = 1;
        var linePoints = BLA.GenerateLine(grid, start, end);

        // Initialize ILS peak memory tracking
        ilsPeakedMemory = System.GC.GetTotalMemory(false);

        int maxWidth = grid.MaxDimension;

        // Keep increasing the size of the corridor until a path is found or the maximum width is reached
        while (currentWidth <= maxWidth)
        {
            var corridor = DefineCorridor(linePoints, grid, start, end, currentWidth);
            var pathResult = algorithm.Navigate(start, end, corridor);

            // Track peak memory from inner algorithm
            BasePathfinding baseAlgo = algorithm as BasePathfinding;
            if (baseAlgo != null && baseAlgo.peakedMemoryDuringSearch > ilsPeakedMemory)
                ilsPeakedMemory = baseAlgo.peakedMemoryDuringSearch;

            if (pathResult.Success == 1)
            {
                return new PathResult
                {
                    Path = pathResult.Path,
                    PathLength = pathResult.PathLength,
                    PathCost = pathResult.PathCost,
                    VisitedNodes = pathResult.VisitedNodes,
                    CorridorIterations = corridorIterations,
                    CorridorSize = corridor.Count,
                    MaxCorridorWidth = maxWidth,
                    Success = pathResult.Success,
                    Message = pathResult.Message,
                    MaxOpenListSize = pathResult.MaxOpenListSize,
                    MaxClosedListSize = pathResult.MaxClosedListSize,
                    PeakedMemoryBytes = ilsPeakedMemory,
                };
            }
            currentWidth++;
            corridorIterations++;
        }

        var failed = DefaultPath();
        failed.MaxOpenListSize = 0;
        failed.PeakedMemoryBytes = ilsPeakedMemory;
        failed.CorridorSize = 0;
        failed.CorridorIterations = corridorIterations;
        failed.MaxCorridorWidth = maxWidth;
        return failed;
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
            foreach (var neighbor in neighbors)
            {
                // Only include nodes within grid bounds
                if (grid.IsInsideGrid(neighbor.Position))
                    corridorNodes.Add(neighbor);
            }
            corridorNodes.UnionWith(neighbors);
        }

        corridorNodes.Add(start);
        corridorNodes.Add(end);

        return corridorNodes;
    }

}