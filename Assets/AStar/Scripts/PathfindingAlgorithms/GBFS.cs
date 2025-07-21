using System.Collections.Generic;
using UnityEngine;

public static class GBFS
{
    public static PathResult Navigate(Node start, Node goal, HashSet<Node> allowedNodes = null, bool trackStats = true)
    {
        if (trackStats)
        {
            var (result, stats) = Stats.RecordStats(() => FindPath(start, goal, allowedNodes));
            result.TimeTaken = stats.TimeTaken;
            result.SpaceTaken = stats.SpaceTaken;
            return result;
        }
        return FindPath(start, goal, allowedNodes);
    }

    private static PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
    {
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        
        // Step 1: Place the start node in the open list
        openList.Add(start);
        
        while (openList.Count > 0)
        {
            // Step 3: Pick the lowest hCost node from the open list
            var currentNode = HeuristicHelper.FindLowestH(openList);
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == goal)
            {
                var (path, totalCost) = HeuristicHelper.RetracePath(start, goal);   
                return new PathResult
                {
                    Path = path,
                    PathLength = path.Count,
                    PathCost = totalCost
                };
            }
            
            // Step 4: expand the neighbors
            var neighbors = currentNode.GetNeighbors();
            foreach (var neighbor in neighbors)
            {
                // if the path is blocked or has already been evaluated, skip it
                if(neighbor.isBlocked || closedList.Contains(neighbor))
                    continue;

                if (!openList.Contains(neighbor))
                {
                    neighbor.parent = currentNode;
                    neighbor.hCost = HeuristicHelper.GetManhattanDistance(neighbor, goal);
                    openList.Add(neighbor);
                }
            }
        }
        Debug.LogWarning("No path found to the goal!");
        return null;
    }

    // private static List<Node> RetracePath(Node start, Node end)
    // {
    //     List<Node> path = new List<Node>();
    //     Node currentNode = end;
    //
    //     // Retrace the path from the end node to the start node
    //     while (currentNode != null && currentNode != start)
    //     {
    //         path.Add(currentNode);
    //         currentNode = currentNode.parent;
    //     }
    //
    //     // Reverse the path to get it from start to end
    //     path.Add(start);
    //     path.Reverse();
    //     return path;
    // }
    // private static Node FindLowestH(List<Node> nodeList)
    // {
    //     Node lowestNode = null;
    //     float lowestHCost = float.MaxValue;
    //     
    //     // Go through every node in the list
    //     foreach (var node in nodeList)
    //     {
    //         // if the node has a lower fCost than the current lowest, set it as the new lowest
    //         if (node.hCost < lowestHCost)
    //         {
    //             lowestHCost = node.hCost;
    //             lowestNode = node;
    //         }
    //     }
    //
    //     // return the node with the lowest fCost
    //     return lowestNode;
    // }
}
