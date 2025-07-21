
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

    public void SetType(TerrainType type, string ColorHex, bool blocked)
    {
        if (ColorUtils.GetColorFromHex(ColorHex, out var color))
        {
            terrainType = type;
            isBlocked = blocked;
            SetColor(color);
            defaultColor = color;
        }
        else
        {
            Debug.LogError($"Invalid color hex: {ColorHex}");
            terrainType = type;
            isBlocked = blocked;
            defaultColor = Color.black;
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

    public void ToggleNeighbours(bool value)
    {
        Color color = value ? Color.yellow : defaultColor;
        foreach (var n in neighbors)
            n.SetColor(color);
    }

    public void ResetNode()
    {
        parent = null;
        SetColor(defaultColor);
    }

    public void DestroyNode()
    {
        Destroy(gameObject);
    }
}