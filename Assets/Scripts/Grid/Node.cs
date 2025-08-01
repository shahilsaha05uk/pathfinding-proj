
using System.Collections.Generic;
using UnityEngine;

/*
    Orthogonal neighbors: Up, Down, Left : Right: Total = 6
    Diagonal neighbors: Up-Left, Up-Right, Down-Left, Down-Right : Total = 8 corners +  
 */

public class Node : MonoBehaviour
{
    private TerrainType defaultTerrainType;
    private Color defaultColor;
    private bool defaultIsBlocked;
    private bool bIsEndPoint;
    private TerrainType terrainType;

    public int gridX, gridY, gridZ;

    public Vector3Int travelDirection;

    public float gCost, hCost, fCost;
    public Node parent;
    public bool bIsBlocked;

    public void Init(TerrainData tData, Vector3Int gridPos)
    {
        gridX = gridPos.x;
        gridY = gridPos.y;
        gridZ = gridPos.z;

        defaultTerrainType = tData.Type;
        defaultColor = tData.Color;
        defaultIsBlocked = tData.IsBlocked;

        terrainType = defaultTerrainType;
        bIsBlocked = defaultIsBlocked;

        SetColor(defaultColor);
    }

    public void UpdateNode(TerrainData data)
    {
        terrainType = data.Type;
        bIsBlocked = data.IsBlocked;
        SetColor(data.Color);
    }

    public void SetEndpoint(bool value) => bIsEndPoint = value;
    public void SetIsBlocked(bool value) => bIsBlocked = value;
    public void SetType(TerrainType type) => terrainType = type;
    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }
    public void SetColor(string hex)
    {
        Color color;
        ColorUtils.GetColorFromHex(hex, out color);
        SetColor(color);
    }

    public void SetNodeIndex(int x, int y, int z)
    {
        gridX = x;
        gridY = y;
        gridZ = z;
    }

    public Vector3Int GetNodePositionOnGrid() => new Vector3Int(gridX, gridY, gridZ);
    public TerrainType GetTerrainType() => terrainType;
    public bool IsEndpoint() => bIsEndPoint;
    public void ToggleNeighbours(bool value, List<Node> neighbors)
    {
        Color color = value ? Color.yellow : defaultColor;
        foreach (var n in neighbors)
            n.SetColor(color);
    }

    public void ResetNode()
    {
        parent = null;
        gCost = 0;
        hCost = 0;
        fCost = 0;
        travelDirection = Vector3Int.zero;
        bIsEndPoint = false;
        ResetColor();
        ResetBlockStatus();
        ResetTerrainType();
    }
    public void ResetColor() => SetColor(defaultColor);
    public void ResetBlockStatus() => bIsBlocked = defaultIsBlocked;
    public void ResetTerrainType() => terrainType = defaultTerrainType;

    public void DestroyNode()
    {
        Destroy(gameObject);
    }
}
