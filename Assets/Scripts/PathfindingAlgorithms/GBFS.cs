using System.Collections.Generic;
using UnityEngine;

public class GBFS : BasePathfinding
{
    protected override PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
    {
        int visitedNodes = 0;
        int maxOpenSize = 0;
        int maxClosedSize = 0;
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        // Place the start node in the open list
        start.fCost = CalculateHeuristicDistance(start, goal) * start.GetMovementCost();
        openList.Add(start);

        // Reset and sample initial memory
        peakedMemoryDuringSearch = System.GC.GetTotalMemory(false);

        while (openList.Count > 0)
        {
            // Track max open list size
            if (openList.Count > maxOpenSize)
                maxOpenSize = openList.Count;

            // Sample memory during search
            long currentMemory = System.GC.GetTotalMemory(false);
            if (currentMemory > peakedMemoryDuringSearch)
                peakedMemoryDuringSearch = currentMemory;

            // Pick the node with the lowest weighted greedy score from the open list
            var currentNode = HeuristicHelper.FindLowestF(openList);
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            if (closedList.Count > maxClosedSize)
                maxClosedSize = closedList.Count;

            if (currentNode == goal)
                return ReturnPath(start, goal, visitedNodes, maxOpenSize, maxClosedSize);

            // Expand the neighbors
            var neighbors = GetAllNeighbors(currentNode);
            foreach (var neighbor in neighbors)
            {
                // if the path is blocked, outside allowed corridor,
                // or has already been evaluated, skip it
                if (!HeuristicHelper.IsNodeAllowed(neighbor, allowedNodes) ||
                    closedList.Contains(neighbor))
                    continue;

                float weightedHeuristic = CalculateHeuristicDistance(neighbor, goal) * neighbor.GetMovementCost();

                if (!openList.Contains(neighbor))
                {
                    neighbor.parent = currentNode;
                    neighbor.hCost = CalculateHeuristicDistance(neighbor, goal);
                    neighbor.fCost = weightedHeuristic;
                    openList.Add(neighbor);
                    visitedNodes++;
                }
                else if (weightedHeuristic < neighbor.fCost)
                {
                    neighbor.parent = currentNode;
                    neighbor.hCost = CalculateHeuristicDistance(neighbor, goal);
                    neighbor.fCost = weightedHeuristic;
                }

            }
        }
        return DefaultPath();
    }
}
