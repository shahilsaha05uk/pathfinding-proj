using System;
using System.Collections.Generic;
using System.Threading;
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


public class Grid3D : MonoBehaviour
{
    public GridColor GridColors;
    public Node NodeObject;
    public List<List<Node>> Nodes;
    
    public int gridSize = 10;
    public float cellSize = 1;
    public float tileSpacing = 0.1f;
    
    private Node startNode;
    private Node endNode;
    
    public void CreateGrid()
    {
        int rows = gridSize;
        int cols = gridSize;
        Nodes = new List<List<Node>>(rows);
        
        for (int x = 0; x < rows; x++)
        {
            List<Node> colNodes = new List<Node>(cols);
            for (int y = 0; y < cols; y++)
            {
                var tile = Instantiate(NodeObject, transform.position, Quaternion.identity, transform);
                tile.name = $"Node_{x}_{y}";
                SetTilePosition(tile, x,y);
                SetNodeIndex(tile, x, y);
                colNodes.Add(tile);
            }
            Nodes.Add(colNodes);
        }
        
        // Assign neighbors after all nodes are created
        AssignNeighbors();
    }
    public void SetStartNode(Node node)
    {
        if (node == null) return;
        
        if (startNode != null)
            SetNodeColor(startNode, GridColors.DefaultNodeColor);
        startNode = node;
        SetNodeColor(node, GridColors.StartNodeColor);
    }
    public void SetEndNode(Node node)
    {
        if (node == null) return;
        
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
    public void DestroyGrid()
    {
        foreach (var row in Nodes)
        {
            foreach (var node in row)
            {
                if (node != null)
                    Destroy(node.gameObject);
            }
        }

        Nodes.Clear(); // optional: reset the list
    }

    public void ClearGrid()
    {
        
    }
    
    public (Node start, Node end) GetStartEndNodes()
    {
        return (startNode, endNode);
    }
    private void SetTilePosition(Node tile, int row, int col)
    {
        float posX = transform.position.x + col * (cellSize + tileSpacing);
        float posZ = transform.position.z + row * (cellSize + tileSpacing);

        tile.transform.localScale = new Vector3(cellSize, cellSize, 1);
        tile.transform.position = new Vector3(posX, 0, posZ);
    }
    private void SetNodeColor(Node node, Color color)
    {
        if (node != null)
        {
            node.SetColor(color);
        }
    }
    private void SetNodeIndex(Node node, int row, int col)
    {
        node.SetNodeIndex(row, col);
    }
    private void AssignNeighbors()
    {
        // Get the number of rows and columns in the grid
        int rows = Nodes.Count;
        int cols = Nodes[0].Count;
        
        // Iterate through each node in the grid
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                List<Node> neighbors = new List<Node>();

                // 4-directional (NSEW)
                TryAddNeighbor(neighbors, x - 1, y); // Left
                TryAddNeighbor(neighbors, x + 1, y); // Right
                TryAddNeighbor(neighbors, x, y - 1); // Down
                TryAddNeighbor(neighbors, x, y + 1); // Up

                // Optional: add diagonals for 8-way movement
                TryAddNeighbor(neighbors, x - 1, y - 1); // Bottom-left
                TryAddNeighbor(neighbors, x - 1, y + 1); // Top-left
                TryAddNeighbor(neighbors, x + 1, y - 1); // Bottom-right
                TryAddNeighbor(neighbors, x + 1, y + 1); // Top-right

                Nodes[x][y].SetNeighbors(neighbors);
            }
        }
    }
    private void TryAddNeighbor(List<Node> list, int x, int y)
    {
        if (x >= 0 &&   // row index is valid
            x < gridSize &&     // row index is less than grid size
            y >= 0 &&   // column index is valid
            y < gridSize)   // column index is less than grid size
        {
            list.Add(Nodes[x][y]);
        }
    }
}
