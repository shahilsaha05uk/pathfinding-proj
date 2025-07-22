
using System.Collections.Generic;
using UnityEngine;

/*
    Orthogonal neighbors: Up, Down, Left, Right
    Diagonal neighbors: Up-Left, Up-Right, Down-Left, Down-Right
 */

public enum TerrainType { Ground, HillTop, Cave, Obstacle }

public class Node : MonoBehaviour
{
    private List<Node> neighbors;
    private List<Node> Neighbors_Orthogonal;
    private List<Node> Neighbors_Diagonal;
    [SerializeField] private Color defaultColor;

    public float gCost, hCost, fCost;
    public int gridX, gridY, gridZ;
    public Node parent;
    public TerrainType terrainType;
    public bool isBlocked;

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
    
    public void SetNeighbors(List<Node> neighbors) => this.neighbors = neighbors;
    
    public void SetOrthogonalNeighbours(List<Node> neighbors) => Neighbors_Orthogonal = neighbors;

    public void SetDiagonalNeighbours(List<Node> neighbors) => Neighbors_Diagonal = neighbors;

    public List<Node> GetNeighbors() => this.neighbors?? null;
    
    public List<Node> GetOrthogonalNeighbors() => Neighbors_Orthogonal ?? null;

    public List<Node> GetDiagonalNeighbors() => Neighbors_Diagonal ?? null;

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