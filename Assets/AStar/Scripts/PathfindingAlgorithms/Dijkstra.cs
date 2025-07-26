

using System.Collections.Generic;

public class Dijkstra : BasePathfinding
{
    protected override PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
    {
        var openQueue = new PriorityQueue<Node, float>();
        var visited = new HashSet<Node>();

        start.gCost = 0;
        openQueue.Enqueue(start, 0);

        while (openQueue.Count > 0)
        {
            var current = openQueue.Dequeue();

            if(current == goal)
                return ReturnPath(start, goal, visited.Count);

            visited.Add(current);

            var neighbors = GetAllNeighbors(current);
            foreach (var neighbor in neighbors)
            {
                if(visited.Contains(neighbor) || 
                   !HeuristicHelper.IsNodeAllowed(neighbor, allowedNodes))
                    continue;

                var tentativeGCost = current.gCost + HeuristicHelper.GetManhattanDistance(current, neighbor);

                if(tentativeGCost < neighbor.gCost)
                {
                    neighbor.gCost = tentativeGCost;
                    neighbor.parent = current;
                    openQueue.Enqueue(neighbor, neighbor.gCost);
                }
            }
        }

        return null;
    }
}
