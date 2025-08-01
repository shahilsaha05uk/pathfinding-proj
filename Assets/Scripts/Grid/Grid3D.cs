using System.Collections.Generic;
using UnityEngine;

public class Grid3D : BaseGrid
{
    private Node[,,] Nodes;

    private int maxHeight;
    private float offsetX;
    private float offsetY;
    private float noiseScale = 1f;

    [SerializeField] private float waterLevel = 0.2f;  // Below this = lake
    [SerializeField] private float caveLevel = 0.4f;  // Below this = lake
    [SerializeField] private int maxTraversableHeight = 3; // Hills above this = non-traversable
    [SerializeField] private SO_TerrainConfig terrainConfig;

    public static Grid3D Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void Create(GridConfig config)
    {
        List<Node> potentialObstacles = new();
        if (Nodes != null && Nodes.Length > 0) Clear();
        
        base.Create(config);

        offsetX = Random.Range(config.OffsetX.min, config.OffsetX.max);
        offsetY = Random.Range(config.OffsetY.min, config.OffsetY.max);

        noiseScale = config.NoiseScale;
        maxHeight = config.MaxHeight;
        
        // Create the array of nodes
        Nodes = new Node[mGridSize, maxHeight, mGridSize];
        
        for (int x = 0; x < mGridSize; x++)
        {
            for (int z = 0; z < mGridSize; z++)
            {
                // This will create a random height based on the noise set
                float noise = AddNoiseXY(x, z, offsetX, offsetY);
                int terrainHeight = Mathf.FloorToInt(noise * maxHeight);

                // This will create nodes for that height, one for each [step]
                for (int y = 0; y < terrainHeight; y++)
                {
                    // Instantiate the node and set its initial transform to 0
                    var node = Instantiate(nodeObject, transform.position, Quaternion.identity, transform);
                    node.name = $"Node_{x}_{y}_{z}";
                    
                    SetNodePosition(node, x, y, z);
                    SetNodeIndex(node, x, y, z);
                    Nodes[x, y, z] = node;

                    node.Init(terrainConfig.GetData(TerrainType.Ground), new Vector3Int(x, y, z));

                    // Apply type, color, and block logic
                    if (y == 0)
                    {
                        // Bottom layer → either cave or lake based on noise
                        if (noise < waterLevel)
                            node.Init(terrainConfig.GetData(TerrainType.Lake), new Vector3Int(x, y, z));
                        else if (noise < caveLevel)
                            node.Init(terrainConfig.GetData(TerrainType.Cave), new Vector3Int(x, y, z));
                        else
                        {
                            node.Init(terrainConfig.GetData(TerrainType.Ground), new Vector3Int(x, y, z));
                            potentialObstacles.Add(node);
                        }
                    }
                    else if (y == terrainHeight - 1)
                    {
                        // Surface level - determine if hilltop or ground
                        if (terrainHeight > maxTraversableHeight)
                            node.Init(terrainConfig.GetData(TerrainType.HillTop), new Vector3Int(x, y, z));
                        else
                        {
                            node.Init(terrainConfig.GetData(TerrainType.Ground), new Vector3Int(x, y, z));
                            potentialObstacles.Add(node);
                        }
                    }
                    else
                    {
                        // In-between layers
                        node.Init(terrainConfig.GetData(TerrainType.Ground), new Vector3Int(x, y, z));
                        potentialObstacles.Add(node);
                    }
                }
            }
        }

        obstacleManager.Init(potentialObstacles); // Initialize the obstacle manager with potential nodes
        UpdateObstacles(config.ObstacleDensity); // Add obstacles based on the percentage
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

        if(navPath != null && navPath.Count > 0) navPath.Clear();
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

    public bool IsInsideGrid(Vector3Int point)
    {
        return IsInsideGrid(point.x, point.y, point.z);
    }

    public override void ClearObstacles() => obstacleManager.Clear();
    
    public void UpdateObstacles(float percent) => obstacleManager.UpdateObstacleDensity(percent);
    
    public Node[,,] GetAllNodes() => Nodes;
    
    protected void RemoveObstacles(float percent) => obstacleManager.Remove(percent);

    private void SetNodePosition(Node node, int x, int y, int z)
    {
        // Calculate the idle position, as to where it should be without noise
        float baseX = transform.position.x + x;
        float baseY = transform.position.y + y;
        float baseZ = transform.position.z + z;

        Vector3 finalPosition = new Vector3(baseX, baseY, baseZ);

        // Set the node's transform properties
        node.transform.position = finalPosition;
    }
    
    private float AddNoiseXY(int x, int y, float offX, float offY)
    {
        return Mathf.PerlinNoise((x + offX) * noiseScale, (y + offY) * noiseScale);
    }
}
