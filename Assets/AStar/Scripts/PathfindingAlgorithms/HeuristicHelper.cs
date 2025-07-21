    
using System.Collections.Generic;
using UnityEngine;

public static class HeuristicHelper
{
    /// <summary>
    /// Manhattan Distance (L1) — for grid-based movement (no diagonals).
    /// In 3D: sum of absolute differences on all three axes.
    /// </summary>
    public static float GetManhattanDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        int dz = Mathf.Abs(a.gridZ - b.gridZ);
        return dx + dy + dz;
    }

    /// <summary>
    /// Euclidean Distance (L2 norm) — straight-line distance.
    /// In 3D: √(dx² + dy² + dz²)
    /// </summary>
    public static float GetEuclideanDistance(Node a, Node b)
    {
        float dx = a.gridX - b.gridX;
        float dy = a.gridY - b.gridY;
        float dz = a.gridZ - b.gridZ;
        return Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    /// <summary>
    /// Chebyshev Distance — allows diagonal moves at the same cost as straight.
    /// In 3D: max of dx, dy, dz
    /// </summary>
    public static float GetChebyshevDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        int dz = Mathf.Abs(a.gridZ - b.gridZ);
        return Mathf.Max(dx, Mathf.Max(dy, dz));
    }
    
    public static (List<Node> path, float totalCost) RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;
        float totalCost = 0f;
        
        while (currentNode != start)
        {
            path.Add(currentNode);
            totalCost += currentNode.gCost;
            currentNode = currentNode.parent;
        }
        path.Add(start);
        path.Reverse();
        return (path, totalCost);
    }
    
    public static Node FindLowestF(List<Node> nodeList)
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
    
    public static Node FindLowestH(List<Node> nodeList)
    {
        Node lowestNode = null;
        float lowestHCost = float.MaxValue;
        
        // Go through every node in the list
        foreach (var node in nodeList)
        {
            // if the node has a lower fCost than the current lowest, set it as the new lowest
            if (node.hCost < lowestHCost)
            {
                lowestHCost = node.hCost;
                lowestNode = node;
            }
        }

        // return the node with the lowest fCost
        return lowestNode;
    }

    public static bool IsNodeAllowed(Node n, HashSet<Node> allowedNodes) =>
        allowedNodes == null || (!n.isBlocked && allowedNodes.Contains(n));
}
