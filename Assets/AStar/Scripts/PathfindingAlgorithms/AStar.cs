using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

/*
 * gCost = Cost of the start node
 * hCost = estimated cost from the current node to the end node
 * fCost = (gCost + hCost) represents the total cost of the PATH
 */

public class AStar : BasePathfinding
{
    protected override PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
    {
        var openQueue = new PriorityQueue<Node, float>();  // Priority queue for O(log n) operations
        var openSet = new HashSet<Node>();                 // For O(1) contains checks
        var closedSet = new HashSet<Node>();
        int visitedNodes = 0;

        // Initialize start node
        start.gCost = 0;
        start.hCost = CalculateHeuristicDistance(start, goal);
        start.fCost = start.gCost + start.hCost;

        openQueue.Enqueue(start, start.fCost);
        openSet.Add(start);

        while (openQueue.Count > 0)
        {
            var current = openQueue.Dequeue();
            openSet.Remove(current);

            if (current == goal)
                return ReturnPath(start, goal, visitedNodes);

            closedSet.Add(current);

            foreach (var neighbor in GetAllNeighbors(current))
            {
                if (closedSet.Contains(neighbor) ||
                    !HeuristicHelper.IsNodeAllowed(neighbor, allowedNodes))
                    continue;

                float tentativeGCost = current.gCost + CalculateHeuristicDistance(current, neighbor);

                if (!openSet.Contains(neighbor) || tentativeGCost < neighbor.gCost)
                {
                    neighbor.parent = current;
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = CalculateHeuristicDistance(neighbor, goal);
                    neighbor.fCost = neighbor.gCost + neighbor.hCost;

                    if (!openSet.Contains(neighbor))
                    {
                        openQueue.Enqueue(neighbor, neighbor.fCost);
                        openSet.Add(neighbor);
                        visitedNodes++;
                    }
                    else
                    {
                        openQueue.UpdatePriority(neighbor, neighbor.fCost);
                    }
                }
            }
        }

        return null;
    }
}