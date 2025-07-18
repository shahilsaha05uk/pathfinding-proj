using System.Collections.Generic;
using UnityEngine;

public class Grid3D : BaseGrid
{
    public int maxHeight = 7;

    private Node[,,] Nodes;
    [Range(0f, 100f)][SerializeField]
    private float offsetX;
    [Range(0f, 100f)][SerializeField]
    private float offsetZ;
    [Range(0f, 10f)] [SerializeField]
    private float randomizeOffset;
    
    private float noiseScale = 0.1f;
    public override void Create()
    {
        // Create the array of nodes
        Nodes = new Node[gridSize, maxHeight, gridSize];
        
        // Randomize the offset values to create a more varied terrain
        offsetX += Random.Range(-randomizeOffset, randomizeOffset);
        offsetZ += Random.Range(-randomizeOffset, randomizeOffset);
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // This will create a random height based on the noise set
                float noise = AddNoiseXZ(x, z, offsetX, offsetZ);
                int terrainHeight = Mathf.FloorToInt(noise * maxHeight);

                // This will create nodes for that height, one for each [step]
                for (int y = 0; y < terrainHeight; y++)
                {
                    // Instantiate the node and set its initial transform to 0
                    var node = Instantiate(NodeObject, transform.position, Quaternion.identity, transform);
                    node.name = $"Node_{x}_{y}_{z}";
                    
                    SetNodePosition(node, x, y, z);
                    SetNodeIndex(node, x, y, z);
                    Nodes[x, y, z] = node;

                    // Apply type, color, and block logic
                    if (y == terrainHeight - 1)
                    {
                        // Hilltop
                        node.SetType(TerrainType.HillTop, Color.green, false);
                    }
                    else if (y == 0)
                    {
                        // Cave bottom
                        node.SetType(TerrainType.Cave, Color.gray, true);
                    }
                    else
                    {
                        // Middle ground
                        node.SetType(TerrainType.Ground, Color.yellow, false);
                    }
                }
            }
        }

        AssignNeighbors(); // If pathfinding requires neighbor linking
    }

    public override void Clear()
    {
        if (Nodes == null || Nodes.Length == 0)
        {
            Debug.LogWarning("No nodes to clear.");
            return;
        }
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    // Continue if the node is null
                    if (Nodes[x, y, z] == null) continue;
                    
                    // Destroy the node GameObject
                    Nodes[x, y, z].DestroyNode();
                }
            }
        }
        Nodes = null;
    }
    protected override void AssignNeighbors()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    var node = Nodes[x, y, z];
                    if (node == null) continue;
                    
                    // Set the node's neighbors
                    var neighbors = GetNeighbors(x, y, z);
                    node.SetNeighbors(neighbors);
                }
            }
        }
    }
    
    private List<Node> GetNeighbors(int x, int y, int z)
    {
        List<Node> neighbors = new List<Node>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dy == 0 && dz == 0)
                        continue;

                    int nx = x + dx;
                    int ny = y + dy;
                    int nz = z + dz;

                    if (IsInsideGrid(nx, ny, nz) && Nodes[nx, ny, nz] != null)
                        neighbors.Add(Nodes[nx, ny, nz]);
                }
            }
        }

        return neighbors;
    }
    private void SetNodePosition(Node node, int x, int y, int z)
    {
        // Calculate the idle position, as to where it should be without noise
        float baseX = transform.position.x + x * (cellSize + tileSpacing);
        float baseY = transform.position.y + y * (cellSize + tileSpacing);
        float baseZ = transform.position.z + z * (cellSize + tileSpacing);

        // TODO: These values should be set dynamically
        // Add random noise to the position
        float noiseX = Random.Range(-0.2f, 0.2f);
        float noiseY = Random.Range(-0.2f, 0.2f);
        float noiseZ = Random.Range(-0.2f, 0.2f);

        // Finally add the noise to the base position and create a final position
        Vector3 finalPosition = new Vector3(
            baseX + noiseX,
            baseY + noiseY,
            baseZ + noiseZ
        );

        // Set the node's transform properties
        node.transform.localScale = new Vector3(cellSize, cellSize, 1);
        node.transform.position = finalPosition;
    }
    private float AddNoiseXZ(int x, int z, float offX, float offZ)
    {
        return Mathf.PerlinNoise((x + offX) * noiseScale, (z + offZ) * noiseScale);
    }
}
