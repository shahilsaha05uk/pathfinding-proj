using System.Collections.Generic;
using UnityEngine;

/*
    Orthogonal neighbors: Up, Down, Left : Right: Total = 6
    Diagonal neighbors: Up-Left, Up-Right, Down-Left, Down-Right : Total = 8 corners +  
 */

public class Node
{
    private TerrainType defaultTerrainType;
    private Color defaultColor;
    private Color currentColor;
    private bool defaultIsBlocked;
    private bool bIsEndPoint;
    private TerrainType terrainType;
    private NodeView boundView;

    public int gridX, gridY, gridZ;
    public Vector3Int Position => new(gridX, gridY, gridZ);

    public Vector3Int travelDirection;

    public float gCost, hCost, fCost;
    public Node parent;
    public bool bIsBlocked;

    private float defaultMovementCost;
    private float movementCost;

    public void Init(TerrainData tData, Vector3Int gridPos)
    {
        gridX = gridPos.x;
        gridY = gridPos.y;
        gridZ = gridPos.z;

        defaultTerrainType = tData.Type;
        defaultColor = tData.Color;
        defaultIsBlocked = tData.IsBlocked;
        defaultMovementCost = tData.MovementCost;

        terrainType = defaultTerrainType;
        bIsBlocked = defaultIsBlocked;
        movementCost = defaultMovementCost;
        currentColor = defaultColor;

        SetColor(defaultColor);
    }

    public void UpdateNode(TerrainData data)
    {
        terrainType = data.Type;
        bIsBlocked = data.IsBlocked;
        defaultTerrainType = data.Type;
        defaultColor = data.Color;
        defaultIsBlocked = data.IsBlocked;
        defaultMovementCost = data.MovementCost;
        movementCost = data.MovementCost;
        currentColor = data.Color;
        SetColor(data.Color);
    }

    public void SetEndpoint(bool value) => bIsEndPoint = value;
    public void SetIsBlocked(bool value) => bIsBlocked = value;
    public void SetType(TerrainType type) => terrainType = type;
    public void SetColor(Color color)
    {
        currentColor = color;
        boundView?.SetColor(color);
    }

    public void AttachView(NodeView view)
    {
        boundView = view;
        boundView?.SetColor(currentColor);
    }

    public void DetachView(NodeView view)
    {
        if (boundView == view)
            boundView = null;
    }

    public Color GetCurrentColor() => currentColor;
    public bool HasView() => boundView != null;

    public void SetNodeIndex(int x, int y, int z)
    {
        gridX = x;
        gridY = y;
        gridZ = z;
    }

    public Vector3Int GetNodePositionOnGrid() => new Vector3Int(gridX, gridY, gridZ);
    public TerrainType GetTerrainType() => terrainType;
    public bool IsEndpoint() => bIsEndPoint;
    public float GetMovementCost() => movementCost <= 0f ? 1f : movementCost;

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
        ResetMovementCost();
    }
    public void ResetColor() => SetColor(defaultColor);
    public void ResetBlockStatus() => bIsBlocked = defaultIsBlocked;
    public void ResetTerrainType() => terrainType = defaultTerrainType;
    public void ResetMovementCost() => movementCost = defaultMovementCost;

    public void DestroyNode()
    {
        if (boundView != null)
            Object.Destroy(boundView.gameObject);
        boundView = null;
    }
}
