
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float gCost, hCost, fCost;
    public int gridX, gridY;
    private List<Node> neighbors;
    public Node parent;
    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }
    public void SetNeighbors(List<Node> neighbors)
    {
        this.neighbors = neighbors;
    }
    public List<Node> GetNeighbors()
    {
        if (neighbors != null && neighbors.Count > 0)
            return neighbors;
        return new List<Node>();
    }
    public void SetNodeIndex(int x, int y)
    {
        gridX = x;
        gridY = y;
    }
    
    public static float GetHeuristicDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        // Diagonal movement cost = 14, straight = 10 (classic heuristic)
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}