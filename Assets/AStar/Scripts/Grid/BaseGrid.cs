using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGrid : MonoBehaviour
{
    protected List<Node> navPath;

    public Node NodeObject;
    public GridColor GridColors;
    
    protected GridConfig mConfig;
    
    protected int mGridSize = 10;
    protected float mTileSize = 1;
    protected float mTileSpacing = 0.1f;
    protected float mObstacleDensity;
    
    protected Node startNode;
    protected Node goalNode;

    public virtual void Create(GridConfig config)
    {
        mConfig = config;
        mGridSize = mConfig.GridSize;
        mTileSize = mConfig.TileSize;
        mTileSpacing = mConfig.TileSpacing;
        mObstacleDensity = mConfig.ObstacleDensity;
    }
    
    public virtual void Clear() { }

    public void SetStartNode(Node node) => SetNode(node, ref startNode, GridColors.StartNodeColor);
    
    public void SetEndNode(Node node) => SetNode(node, ref goalNode, GridColors.EndNodeColor);
    
    public void HighlightPath(List<Node> path)
    {
        if (path == null || path.Count < 2)
        {
            Debug.LogWarning("Path is empty or too short to set path nodes.");
            return;
        }
        
        navPath = path;
        int size = path.Count - 1;
        
        for(int i = 1; i < size; i++)
            SetNodeColor(path[i], GridColors.PathNodeColor);
    }

    public (Node start, Node goal) GetStartEndNodes() => (startNode, goalNode);
    
    public List<Node> GetPath() => navPath;

    public GridConfig GetGridConfig() => mConfig;

    public void ResetPath()
    {
        if(navPath == null || navPath.Count == 0)
            return;

        foreach (var node in navPath)
            node.ResetNode();

        if(startNode != null)
            SetNodeColor(startNode, GridColors.StartNodeColor);
        if (goalNode != null)
            SetNodeColor(goalNode, GridColors.EndNodeColor);
    }

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

    private void SetNode(Node newNode, ref Node targetNode, Color color)
    {
        if (newNode == null || newNode.isBlocked) return;

        if (targetNode != null)
            targetNode.ResetNode();
        targetNode = newNode;
        SetNodeColor(newNode, color);
    }
}
