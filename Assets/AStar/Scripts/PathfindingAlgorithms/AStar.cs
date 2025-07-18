using System.Collections.Generic;
using UnityEngine;

/*
 * gCost = Cost of the start node
 * hCost = estimated cost from the current node to the end node
 * fCost = (gCost + hCost) represents the total cost of the PATH
 */

public static class AStar
{
    public static List<Node> Navigate(Node start, Node end)
    {
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(start);
        start.gCost = 0;                                                     // Cost of the start node is 0
        start.hCost = HeuristicHelper.GetManhattanDistance(start, end); // Heuristic cost from start to end
        start.fCost = start.gCost + start.hCost;                            // Total cost of the path from start to end
        
        // while there are nodes in the open list
        while (openList.Count > 0)
        {
            // Get the current Node with the lowest fCost
            Node currentNode = FindLowestF(openList);
            
            // if the node is there in the open list, move it to the closed list
            if(openList.Contains(currentNode))
                openList.Remove(currentNode);
            
            closedList.Add(currentNode);
            
            // if the goal node is the current node, retrace the path and return it
            if (currentNode == end)
                return RetracePath(start, end);
            
            // Else, get all the neighbors of the current node
            var neighbors = currentNode.GetNeighbors();

            // Loop through each neighbor
            foreach (var neighbor in neighbors)
            {
                // If the neighbor is already in the closed list or is blocked, skip it
                if(closedList.Contains(neighbor) || neighbor.isBlocked)
                    continue;
                
                // Gets the distance from the current node to the neighbor
                float tentativeGCost = currentNode.gCost + HeuristicHelper.GetManhattanDistance(currentNode, neighbor);
                
                // calculate the cost of the path to the neighbor
                if (tentativeGCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = HeuristicHelper.GetManhattanDistance(neighbor, end);
                    neighbor.fCost = neighbor.gCost + neighbor.hCost;
                    neighbor.parent = currentNode;

                    // Add it to the open list if it is not already there
                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }
        
        // No path found
        Debug.LogWarning("No path found!");
        return new List<Node>();
    }

    // This will find the node with the lowest cost (fCost) to the destination from the nodes in the open list
    private static Node FindLowestF(List<Node> nodeList)
    {
        Node lowestNode = null;
        float lowestFCost = float.MaxValue;
        
        // Go through every node in the list
        foreach (var node in nodeList)
        {
            // if the node has a lower fCost than the current lowest, set it as the new lowest
            if (node.fCost < lowestFCost)
            {
                lowestFCost = node.fCost;
                lowestNode = node;
            }
        }

        // return the node with the lowest fCost
        return lowestNode;
    }
    private static List<Node> RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(start);
        path.Reverse();
        return path;
    }
}
