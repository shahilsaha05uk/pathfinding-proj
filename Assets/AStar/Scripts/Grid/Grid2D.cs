using System.Collections.Generic;
using UnityEngine;

public class Grid2D : BaseGrid
{
    public Node[,] Nodes;
    
    public override void Create()
    {
        Nodes = new Node[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                var tile = Instantiate(NodeObject, transform.position, Quaternion.identity, transform);
                tile.name = $"Node_{x}_{y}";
                SetNodePosition(tile, x, y);
                SetNodeIndex(tile, x, y);
                Nodes[x, y] = tile;
            }
        }
        
        // Assign neighbors after all nodes are created
        AssignNeighbors();
    }
    public override void Clear()
    {
        if (Nodes == null || Nodes.Length == 0)
        {
            Debug.LogWarning("No nodes to clear.");
            return;
        }

        Nodes = null;
    }

    protected override void AssignNeighbors()
    {
        // Iterate through each node in the grid
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
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

                Nodes[x, y].SetNeighbors(neighbors);
            }
        }
    }

    private void TryAddNeighbor(List<Node> list, int x, int y)
    {
        if (IsInsideGrid(x , y, 0))   // column index is less than grid size
        {
            list.Add(Nodes[x, y]);
        }
    }
    private void SetNodePosition(Node tile, int x, int z)
    {
        float posX = transform.position.x + x * (cellSize + tileSpacing);
        float posZ = transform.position.z + z * (cellSize + tileSpacing);

        tile.transform.localScale = new Vector3(cellSize, cellSize, 1);
        tile.transform.position = new Vector3(posX, 0, posZ);    
    }
}
