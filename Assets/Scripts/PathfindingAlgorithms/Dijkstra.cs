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

        start.gCost = 0;
        openQueue.Enqueue(start, 0);
        inOpenQueue[start] = 0;

        while (openQueue.Count > 0)
        {
            var current = openQueue.Dequeue();
            inOpenQueue.Remove(current);

            if (current == goal)
                return ReturnPath(start, goal, visited.Count);

            if (visited.Contains(current))
                continue;

            visited.Add(current);

            var neighbors = GetAllNeighbors(current);
            foreach (var neighbor in neighbors)
            {
                if (visited.Contains(neighbor))
                    continue;

                if (!HeuristicHelper.IsNodeAllowed(neighbor, allowedNodes))
                    continue;

                float distance = CalculateHeuristicDistance(current, neighbor);
                var tentativeGCost = current.gCost + distance;

                if (tentativeGCost < neighbor.gCost)
                {
                    neighbor.gCost = tentativeGCost;
                    neighbor.parent = current;

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