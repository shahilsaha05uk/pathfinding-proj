using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GridColor
{
    public Color DefaultNodeColor;
    public Color StartNodeColor;
    public Color EndNodeColor;
    public Color PathNodeColor;
    public Color ObstacleNodeColor;
    public Color VisitedNodeColor;
}

public abstract class BaseGrid : MonoBehaviour
{
    public Node NodeObject;
    public GridColor GridColors;
    
    protected GridConfig mConfig;
    
    protected int mGridSize = 10;
    protected float mTileSize = 1;
    protected float mTileSpacing = 0.1f;
    protected float mObstacleDensity;
    protected Node startNode;
    protected Node endNode;

    public virtual void Create(GridConfig config)
    {
        mConfig = config;
        mGridSize = mConfig.GridSize;
        mTileSize = mConfig.TileSize;
        mTileSpacing = mConfig.TileSpacing;
        mObstacleDensity = mConfig.ObstacleDensity;
    }
    
    public virtual void Clear() { }

    public void SetStartNode(Node node)
    {
        if (node == null || node.isBlocked) return;
        
        if (startNode != null)
            SetNodeColor(startNode, GridColors.DefaultNodeColor);
        startNode = node;
        SetNodeColor(node, GridColors.StartNodeColor);

    }
    public void SetEndNode(Node node)
    {
        if (node == null || node.isBlocked) return;
        
        if (endNode != null)
            SetNodeColor(endNode, GridColors.DefaultNodeColor);
        endNode = node;
        SetNodeColor(node, GridColors.EndNodeColor);
    }
    public void SetPathNode(List<Node> path)
    {
        if (path == null || path.Count < 2)
        {
            Debug.LogWarning("Path is empty or too short to set path nodes.");
            return;
        }
        
        int size = path.Count;
        int startIndex = 1;
        int endIndex = size - 1;
        
        for(int i = startIndex; i < endIndex; i++)
            SetNodeColor(path[i], GridColors.PathNodeColor);
    }
    public (Node start, Node end) GetStartEndNodes()
    {
        return (startNode, endNode);
    }
    public GridConfig GetGridConfig() => mConfig;

    protected void SetNodeIndex(Node node, int x, int y, int z = 0)
    {
        if (node != null)
        {
            node.SetNodeIndex(x, y, z);
        }
    }
    protected virtual void SetNodeColor(Node node, Color color)
    {
        if (node != null)
        {
            node.SetColor(color);
        }
    }
    protected virtual void AssignNeighbors() { }

    protected virtual void AddObstacles(List<Node> potentialObstacles) { }
}
