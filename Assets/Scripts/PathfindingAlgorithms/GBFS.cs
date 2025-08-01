using System.Collections.Generic;
using UnityEngine;

public class GBFS : BasePathfinding
{
    protected override PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
    {
        int visitedNodes = 0;
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        
        // Place the start node in the open list
        openList.Add(start);
        
        while (openList.Count > 0)
        {
            // Pick the lowest hCost node from the open list
            var currentNode = HeuristicHelper.FindLowestH(openList);
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == goal)
                return ReturnPath(start, goal, visitedNodes);

            // Expand the neighbors
            var neighbors = GetAllNeighbors(currentNode);
            foreach (var neighbor in neighbors)
            {
                // if the path is blocked or has already been evaluated, skip it
                if(neighbor.bIsBlocked || closedList.Contains(neighbor))
                    continue;

                if (!openList.Contains(neighbor))
                {
                    neighbor.parent = currentNode;
                    neighbor.hCost = CalculateHeuristicDistance(neighbor, goal);
                    openList.Add(neighbor);
                    visitedNodes++;
                }

            }
        }
        Debug.LogWarning("No path found to the goal!");
        return null;
    }
}
