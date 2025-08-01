using System.Collections.Generic;

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

        // Add start node to open set and queue
        openQueue.Enqueue(start, start.fCost);
        openSet.Add(start);

        while (openQueue.Count > 0)
        {
            // Get the next node in the queue
            var current = openQueue.Dequeue();
            openSet.Remove(current);

            // If we reached the goal, return the path
            if (current == goal)
                return ReturnPath(start, goal, visitedNodes);

            // Add current node to closed set
            closedSet.Add(current);

            // Check all neighbors of the current node
            foreach (var neighbor in GetAllNeighbors(current))
            {
                // Skip if neighbor is already in closed set or not allowed
                if (closedSet.Contains(neighbor) ||
                    !HeuristicHelper.IsNodeAllowed(neighbor, allowedNodes))
                    continue;

                // Calculate the cost to move to this neighbor
                float tentativeGCost = current.gCost + CalculateHeuristicDistance(current, neighbor);

                // If the neighbor is not in the open set or the new path is cheaper
                if (!openSet.Contains(neighbor) || tentativeGCost < neighbor.gCost)
                {
                    // Update the neighbor's costs and parent
                    neighbor.parent = current;
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = CalculateHeuristicDistance(neighbor, goal);
                    neighbor.fCost = neighbor.gCost + neighbor.hCost;

                    // If the neighbor is not in the open set, add it
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