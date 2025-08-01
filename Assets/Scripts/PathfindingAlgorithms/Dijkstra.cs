using System;
using System.Collections.Generic;

public class Dijkstra : BasePathfinding
{
    protected override PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
    {
        var openQueue = new PriorityQueue<Node, float>();
        var visited = new HashSet<Node>();
        var inOpenQueue = new Dictionary<Node, float>(); // To track nodes and their current priorities in queue

        // Initialize all nodes' gCost to infinity
        var allNodes = Grid3D.Instance.GetAllNodes();
        foreach (var node in allNodes)
        {
            if(node != null)
                node.gCost = float.MaxValue;
        }

        // Set the start node's gCost to 0 and add it to the open queue
        start.gCost = 0;
        openQueue.Enqueue(start, 0);
        inOpenQueue[start] = 0;

        // while there are nodes in the open queue
        while (openQueue.Count > 0)
        {
            // get the next node in the queue
            var current = openQueue.Dequeue();
            inOpenQueue.Remove(current);

            // if we reached the goal, return the path
            if (current == goal)
                return ReturnPath(start, goal, visited.Count);

            // if the current node has already been visited, skip it
            if (visited.Contains(current))
                continue;

            // mark the current node as visited
            visited.Add(current);

            // check all neighbors of the current node
            var neighbors = GetAllNeighbors(current);
            foreach (var neighbor in neighbors)
            {
                if (visited.Contains(neighbor))
                    continue;

                if (!HeuristicHelper.IsNodeAllowed(neighbor, allowedNodes))
                    continue;

                // Calculate the distance from the current node to the neighbor
                float distance = CalculateHeuristicDistance(current, neighbor);
                var tentativeGCost = current.gCost + distance;

                // If the tentative gCost is less than the neighbor's current gCost
                if (tentativeGCost < neighbor.gCost)
                {
                    // Update the neighbor's gCost and parent
                    neighbor.gCost = tentativeGCost;
                    neighbor.parent = current;

                    // If the neighbor is already in the open queue, update its priority
                    if (inOpenQueue.TryGetValue(neighbor, out float existingPriority))
                    {
                        if (tentativeGCost < existingPriority)
                        {
                            openQueue.UpdatePriority(neighbor, tentativeGCost);
                            inOpenQueue[neighbor] = tentativeGCost;
                        }
                    }
                    else
                    {
                        openQueue.Enqueue(neighbor, tentativeGCost);
                        inOpenQueue[neighbor] = tentativeGCost;
                    }
                }
            }
        }

        return null;
    }
}