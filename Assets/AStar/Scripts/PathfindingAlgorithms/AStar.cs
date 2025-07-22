using System;
using System.Collections.Generic;

/*
 * gCost = Cost of the start node
 * hCost = estimated cost from the current node to the end node
 * fCost = (gCost + hCost) represents the total cost of the PATH
 */

public static class AStar
{
    public static PathResult Navigate(Node start, Node end, HashSet<Node> allowedNodes = null, bool trackStats = true)
    {
        if (trackStats)
        {
            var (result, stats) = Stats.RecordStats(() => FindPath(start, end, allowedNodes));
            if (result != null)
            {
                result.TimeTaken = stats.TimeTaken;
                result.MemoryUsage = stats.MemoryUsed;
                return result;
            }
            return null;
        }
        return FindPath(start, end, allowedNodes)?? null;
    }

    private static PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
    {
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(start);
        start.gCost = 0; // Cost of the start node is 0
        start.hCost = HeuristicHelper.GetManhattanDistance(start, goal); // Heuristic cost from start to end
        start.fCost = start.gCost + start.hCost; // Total cost of the path from start to end

        // while there are nodes in the open list
        while (openList.Count > 0)
        {
            // Get the current Node with the lowest fCost
            Node currentNode = HeuristicHelper.FindLowestF(openList);

            // if the node is there in the open list, move it to the closed list
            if (openList.Contains(currentNode))
                openList.Remove(currentNode);

            closedList.Add(currentNode);

            // if the goal node is the current node, retrace the path and return it
            if (currentNode == goal)
            {
                var (path, totalCost) = HeuristicHelper.RetracePath(start, goal);
                return new PathResult
                {
                    Path = path,
                    PathLength = path.Count,
                    PathCost = totalCost,
                };
            }

            // Else, get all the neighbors of the current node
            var neighbors = currentNode.GetNeighbors();

            // Loop through each neighbor
            foreach (var neighbor in neighbors)
            {
                // If the neighbor is already in the closed list or is blocked, skip it
                if (closedList.Contains(neighbor) || !HeuristicHelper.IsNodeAllowed(neighbor, allowedNodes))
                    continue;

                // Gets the distance from the current node to the neighbor
                float tentativeGCost =
                    currentNode.gCost + HeuristicHelper.GetManhattanDistance(currentNode, neighbor);

                // calculate the cost of the path to the neighbor
                if (tentativeGCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = HeuristicHelper.GetManhattanDistance(neighbor, goal);
                    neighbor.fCost = neighbor.gCost + neighbor.hCost;
                    neighbor.parent = currentNode;

                    // Add it to the open list if it is not already there
                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        // No path was found, return null
        return null;
    }
}
