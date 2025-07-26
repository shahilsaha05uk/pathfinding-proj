using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGrid : MonoBehaviour
{
    [SerializeField] protected ObstacleManager obstacleManager;

    protected List<Node> navPath = new();

    public Node NodeObject;
    public GridColor GridColors;
    
    protected GridConfig mConfig;
    
    protected int mGridSize = 10;
    protected float mTileSpacing = 0.1f;
    
    protected Node startNode;
    protected Node goalNode;

    public virtual void Create(GridConfig config)
    {
        mConfig = config;
        mGridSize = mConfig.GridSize;
    }
    
    public virtual void Clear() { }

    public virtual void ClearObstacles() { }

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

    public GridData GetGridData() => new GridData()
    {
        GridSize = mGridSize,
        MaxHeight = mConfig.MaxHeight,
        NoiseScale = mConfig.NoiseScale,
        ObstacleDensity = obstacleManager.GetCurrentPercent()
    };

    public virtual void ResetPath()
    {
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
    
    protected virtual void AddObstacles(float percent) { }

    private void SetNode(Node newNode, ref Node targetNode, Color color)
    {
        if (newNode == null || newNode.isBlocked) return;

        if (targetNode != null)
            targetNode.ResetNode();
        targetNode = newNode;
        targetNode.SetEndpoint(true);
        SetNodeColor(newNode, color);
    }
}
