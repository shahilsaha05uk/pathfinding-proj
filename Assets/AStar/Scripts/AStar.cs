using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static List<Node> Navigate(Node start, Node end)
    {
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(start);
        start.gCost = 0;
        start.hCost = Node.GetHeuristicDistance(start, end);
        start.fCost = start.gCost + start.hCost;
        
        while (openList.Count > 0)
        {
            // Get the current Node
            Node currentNode = FindLowestF(openList);
            
            if(openList.Contains(currentNode))
                openList.Remove(currentNode);
            
            closedList.Add(currentNode);
            
            // Found the goal
            if (currentNode == end)
            {
                return RetracePath(start, end);
            }
            
            // Generate the children nodes
            var neighbors = currentNode.GetNeighbors();

            foreach (var neighbor in neighbors)
            {
                if(closedList.Contains(neighbor))
                    continue;
                
                // Calculate f, g, and h values
                float tentativeGCost = currentNode.gCost + Node.GetHeuristicDistance(currentNode, neighbor);
                
                if (tentativeGCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = Node.GetHeuristicDistance(neighbor, end);
                    neighbor.fCost = neighbor.gCost + neighbor.hCost;
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }
        
        // No path found
        Debug.LogWarning("No path found!");
        return new List<Node>();
    }
    private static Node FindLowestF(List<Node> nodeList)
    {
        Node lowestNode = null;
        float lowestFCost = float.MaxValue;
        
        foreach (var node in nodeList)
        {
            if (node.fCost < lowestFCost)
            {
                lowestFCost = node.fCost;
                lowestNode = node;
            }
        }

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
