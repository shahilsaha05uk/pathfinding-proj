
using System.Collections.Generic;
using UnityEngine;
public enum TerrainType { Ground, HillTop, Cave }
public class Node : MonoBehaviour
{
    public float gCost, hCost, fCost;
    public int gridX, gridY, gridZ;
    public Node parent;
    public TerrainType terrainType;
    public bool isBlocked;
    private List<Node> neighbors;
    [SerializeField] private Color defaultColor;

    private void Awake()
    {
        defaultColor = GetComponent<MeshRenderer>().material.color;
    }
    public void SetType(TerrainType type, Color color, bool blocked)
    {
        terrainType = type;
        isBlocked = blocked;
        SetColor(color);
    }
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
    public void SetNodeIndex(int x, int y, int z)
    {
        gridX = x;
        gridY = y;
        gridZ = z;
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

    public void ShowNeighbours()
    {
        foreach (var n in neighbors)
        {
            n.SetColor(Color.red);
        }
    }
    public void HideNeighbours()
    {
        foreach (var n in neighbors)
        {
            n.SetColor(defaultColor);
        }
    }

    public void DestroyNode()
    {
        Destroy(gameObject);
    }
}