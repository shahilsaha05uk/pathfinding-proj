
using System.Collections.Generic;
using UnityEngine;
public enum TerrainType { Ground, HillTop, Cave, Obstacle }
public class Node : MonoBehaviour
{
    public float gCost, hCost, fCost;
    public int gridX, gridY, gridZ;
    public Node parent;
    public TerrainType terrainType;
    public bool isBlocked;
    [SerializeField] private List<Node> neighbors;
    [SerializeField] private Color defaultColor;

    private void Awake()
    {
        defaultColor = GetComponent<MeshRenderer>().material.color;
    }
    public void SetType(TerrainType type, string ColorHex, bool blocked)
    {
        if (ColorUtils.GetColorFromHex(ColorHex, out var color))
        {
            terrainType = type;
            isBlocked = blocked;
            SetColor(color);
        }
        else
        {
            Debug.LogError($"Invalid color hex: {ColorHex}");
            terrainType = type;
            isBlocked = blocked;
            SetColor(Color.black);
        }
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