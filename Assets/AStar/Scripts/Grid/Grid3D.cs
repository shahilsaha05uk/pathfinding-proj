using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid3D : BaseGrid
{
    public static Grid3D Instance { get; private set; }
    private Node[,,] Nodes;
    
    private int maxHeight;
    private float offsetX;
    private float offsetZ;
    private float randomizeOffset;
    
    private float noiseScale = 0.1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void Create(GridConfig config)
    {
        if(Nodes != null && Nodes.Length > 0) Clear();
        
        base.Create(config);
        
        List<Node> potentialObstacles = new List<Node>();
        
        offsetX = config.Offset.x;
        offsetZ = config.Offset.z;
        randomizeOffset = config.OffsetRandomization;
        noiseScale = config.NoiseScale;
        maxHeight = config.MaxHeight;
        
        // Create the array of nodes
        Nodes = new Node[mGridSize, maxHeight, mGridSize];
        
        // Randomize the offset values to create a more varied terrain
        offsetX += Random.Range(-randomizeOffset, randomizeOffset);
        offsetZ += Random.Range(-randomizeOffset, randomizeOffset);
        
        for (int x = 0; x < mGridSize; x++)
        {
            for (int z = 0; z < mGridSize; z++)
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
                        node.SetType(TerrainType.HillTop, "#707070", false);
                        potentialObstacles.Add(node);
                    }
                    else if (y == 0)
                    {
                        // Cave bottom
                        node.SetType(TerrainType.Cave, "#290e04", true);
                    }
                    else
                    {
                        // Middle ground
                        node.SetType(TerrainType.Ground, "#0b4f0e", false);
                        potentialObstacles.Add(node);

                    }
                }
            }
        }

        AssignNeighbors(); // If pathfinding requires neighbor linking
        AddObstacles(potentialObstacles); // Add obstacles based on the percentage
    }
    
    public override void Clear()
    {
        if (Nodes == null || Nodes.Length == 0)
        {
            Debug.LogWarning("No nodes to clear.");
            return;
        }
        
        for (int x = 0; x < mGridSize; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                for (int z = 0; z < mGridSize; z++)
                {
                    // Continue if the node is null
                    if (Nodes[x, y, z] == null) continue;
                    
                    // Destroy the node GameObject
                    Nodes[x, y, z].DestroyNode();
                }
            }
        }
        Nodes = null;
        navPath.Clear();
    }

    public List<Node> GetNeighbors(Vector3Int point, int width)
    {
        List<Node> neighbors = new List<Node>();

        for (int dx = -width; dx <= width; dx++)
        {
            for (int dy = -width; dy <= width; dy++)
            {
                for (int dz = -width; dz <= width; dz++)
                {
                    if (dx == 0 && dy == 0 && dz == 0)
                        continue;

                    int nx = point.x + dx;
                    int ny = point.y + dy;
                    int nz = point.z + dz;

                    var neighbor = GetNodeAt(nx, ny, nz);
                    if (neighbor != null)
                        neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    public List<Node> GetManhattanRadius(Vector3Int point, int width, CorridorShape shape)
    {
        var corridorNodes = new List<Node>();
        for (int dx = -width; dx <= width; dx++)
        {
            for (int dy = -width; dy <= width; dy++)
            {
                for (int dz = -width; dz <= width; dz++)
                {
                    if (dx == 0 && dy == 0 && dz == 0) continue;

                    int nx = point.x + dx;
                    int ny = point.y + dy;
                    int nz = point.z + dz;

                    if (!IsInsideGrid(nx, ny, nz)) continue;

                    float distance = shape switch
                    {
                        CorridorShape.Cube => 1f, // Always include
                        CorridorShape.Diamond => Mathf.Abs(dx) + Mathf.Abs(dy) + Mathf.Abs(dz),
                        CorridorShape.Sphere => Mathf.Sqrt(dx * dx + dy * dy + dz * dz),
                        _ => float.MaxValue
                    };

                    if (distance <= width)
                    {
                        var node = GetNodeAt(nx, ny, nz);
                        if (node != null)
                            corridorNodes.Add(node);
                    }
                }
            }
        }
        return corridorNodes;
    }

    public Node GetNodeAt(int x, int y, int z)
    {
        if (!IsInsideGrid(x, y, z)) return null;
        
        var node = Nodes[x, y, z];
        return node != null ? node : null;
    }
    
    public Node GetNodeAt(Vector3Int point)
    {
        return GetNodeAt(point.x, point.y, point.z);
    }

    public bool IsInsideGrid(int x, int y, int z)
    {
        return x >= 0 && x < mGridSize &&
               y >= 0 && y < maxHeight &&
               z >= 0 && z < mGridSize;
    }

    protected override void AssignNeighbors()
    {
        for (int x = 0; x < mGridSize; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                for (int z = 0; z < mGridSize; z++)
                {
                    var node = Nodes[x, y, z];
                    if (node == null) continue;
                    
                    // Set all the neighbors of this node
                    AddAllNeighbors(ref node, x, y, z);

                    // Set orthogonal neighbors
                    AddOrthogonalNeighbors(ref node, x, y, z);

                    // Set diagonal neighbors
                    AddDiagonalNeighbors(ref node, x, y, z);
                }
            }
        }
    }

    protected override void AddObstacles(List<Node> potentialObstacles)
    {
        int obstacleCount = Mathf.FloorToInt(potentialObstacles.Count * mObstacleDensity);
        var shuffled = potentialObstacles.OrderBy(_ => Random.value).ToList();

        for (int i = 0; i < obstacleCount; i++)
        {
            var node = shuffled[i];
            node.SetType(TerrainType.Obstacle, "#a30808", true); // New type or reuse blocked
        }
    }
    
    private void AddAllNeighbors(ref Node node, int x, int y, int z)
    {
        var neighbors = GetNeighbors(x, y, z);
        node.SetNeighbors(neighbors);
    }

    private void AddOrthogonalNeighbors(ref Node node, int x, int y, int z)
    {
        var orthogonalNeighbors = GetOrthogonalNeighbors(x, y, z);
        node.SetOrthogonalNeighbours(orthogonalNeighbors);
    }

    private void AddDiagonalNeighbors(ref Node node, int x, int y, int z)
    {
        var diagonalNeighbors = GetDiagonalNeighbors(x, y, z);
        node.SetDiagonalNeighbours(diagonalNeighbors);
    }

    private List<Node> GetNeighbors(int x, int y, int z)
    {
        return GetNeighbors(new Vector3Int(x,y,z), 1);
    }

    private List<Node> GetOrthogonalNeighbors(int x, int y, int z)
    {
        var directions = new List<Vector3Int>
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };

        return GetDirectionalNeighbors(x, y, z, directions);
    }

    private List<Node> GetDirectionalNeighbors(int x, int y, int z, List<Vector3Int> directions)
    {
        List<Node> neighbors = new List<Node>();

        foreach (var dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;
            int nz = z + dir.z;

            if (IsInsideGrid(nx, ny, nz))
            {
                var neighbor = GetNodeAt(nx, ny, nz);
                if (neighbor != null)
                    neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private List<Node> GetDiagonalNeighbors(int x, int y, int z)
    {
        List<Node> diagonals = new List<Node>();

        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dy == 0 && dz == 0) continue;
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) + Mathf.Abs(dz) == 1) continue; // skip orthogonals

                    int nx = x + dx;
                    int ny = y + dy;
                    int nz = z + dz;

                    var neighbor = GetNodeAt(nx, ny, nz);
                    if (neighbor != null)
                        diagonals.Add(neighbor);
                }

        return diagonals;
    }

    private void SetNodePosition(Node node, int x, int y, int z)
    {
        // Calculate the idle position, as to where it should be without noise
        float baseX = transform.position.x + x * (mTileSize + mTileSpacing);
        float baseY = transform.position.y + y * (mTileSize + mTileSpacing);
        float baseZ = transform.position.z + z * (mTileSize + mTileSpacing);

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
        node.transform.localScale = new Vector3(mTileSize, mTileSize, 1);
        node.transform.position = finalPosition;
    }
    
    private float AddNoiseXZ(int x, int z, float offX, float offZ)
    {
        return Mathf.PerlinNoise((x + offX) * noiseScale, (z + offZ) * noiseScale);
    }
}
