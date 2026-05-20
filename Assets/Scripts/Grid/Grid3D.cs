using System.Collections.Generic;
using UnityEngine;

public class Grid3D : BaseGrid
{
    // Full logical grid used by pathfinding. These nodes are data-only and do not need to be visible.
    private Node[,,] Nodes;

    private int maxHeight;
    private float offsetX;
    private float offsetY;
    private float noiseScale = 1f;
    private float caveOffsetX;
    private float caveOffsetY;

    [SerializeField] private int maxTraversableHeight = 3; // Hills above this = non-traversable
    [SerializeField] private SO_TerrainConfig terrainConfig;

    [Header("Chunk Streaming")]
    [SerializeField] private int chunkSize = 16;
    [SerializeField] private int viewRadiusInChunks = 1;
    [SerializeField] private Transform viewFocus;
    [SerializeField] private bool autoRefreshVisibleChunks = true;

    public static Grid3D Instance { get; private set; }

    private readonly Dictionary<Vector3Int, ChunkState> activeChunks = new();
    private Vector3Int lastVisibleChunkCenter = new(int.MinValue, int.MinValue, int.MinValue);

    private sealed class ChunkState
    {
        public GameObject Root;
    }

    // Unity lifecycle --------------------------------------------------------

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void LateUpdate()
    {
        // Keep chunk visuals around the active focus point.
        if (!autoRefreshVisibleChunks || Nodes == null)
            return;

        RefreshVisibleChunks();
    }

    // Grid creation / cleanup ------------------------------------------------

    public override void Create(GridConfig config)
    {
        // Start with a clean grid if one already exists.
        List<Node> potentialObstacles = new();
        if (Nodes != null && Nodes.Length > 0)
            Clear();

        base.Create(config);

        float offsetXMin = Mathf.Min(config.OffsetX.min, config.OffsetX.max);
        float offsetXMax = Mathf.Max(config.OffsetX.min, config.OffsetX.max);
        float offsetYMin = Mathf.Min(config.OffsetY.min, config.OffsetY.max);
        float offsetYMax = Mathf.Max(config.OffsetY.min, config.OffsetY.max);

        // Random offsets prevent the same seed from producing identical-looking terrain every time.
        offsetX = Random.Range(offsetXMin, offsetXMax);
        offsetY = Random.Range(offsetYMin, offsetYMax);
        caveOffsetX = Random.Range(offsetXMin, offsetXMax) + 1000f;
        caveOffsetY = Random.Range(offsetYMin, offsetYMax) + 2000f;

        noiseScale = Mathf.Max(0.0001f, config.NoiseScale);
        maxHeight = Mathf.Max(1, config.MaxHeight);

        int traversalHeight = Mathf.Max(1, Mathf.RoundToInt(terrainConfig.TraversalHeight));

        // Build the full logical 3D grid.
        Nodes = new Node[mGridSize, maxHeight, mGridSize];

        for (int x = 0; x < mGridSize; x++)
        {
            for (int z = 0; z < mGridSize; z++)
            {
                // Perlin noise gives us a height value for this column.
                float noise = AddNoiseXY(x, z, offsetX, offsetY);
                int terrainHeight = Mathf.Clamp(Mathf.FloorToInt(noise * maxHeight), 1, maxHeight);

                var groundData = terrainConfig.GetData(TerrainType.Ground);
                var caveData = terrainConfig.GetData(TerrainType.Cave);
                var lakeData = terrainConfig.GetData(TerrainType.Lake);
                var hillTopData = terrainConfig.GetData(TerrainType.HillTop);

                var columnTerrain = terrainConfig.GetData(noise);

                // Caves are only kept in clustered regions.
                if (columnTerrain.Type == TerrainType.Cave && !terrainConfig.IsCaveClusterPoint(x, z, caveOffsetX, caveOffsetY))
                    columnTerrain = groundData;

                // Obstacles are handled by obstacle manager after terrain generation.
                if (columnTerrain.Type == TerrainType.Obstacle)
                    columnTerrain = groundData;

                // Each Y slice becomes one logical node in the column.
                for (int y = 0; y < terrainHeight; y++)
                {
                    var node = new Node();
                    node.SetNodeIndex(x, y, z);
                    node.Init(groundData, new Vector3Int(x, y, z));
                    Nodes[x, y, z] = node;

                    TerrainData terrainData;
                    bool isTopLayer = y == terrainHeight - 1;

                    if (columnTerrain.Type == TerrainType.Lake)
                    {
                        // Lake columns render as water volume.
                        terrainData = lakeData;
                    }
                    else if (isTopLayer)
                    {
                        // Surface is ground unless too high, then hilltop.
                        terrainData = terrainHeight > traversalHeight ? hillTopData : groundData;
                    }
                    else if (columnTerrain.Type == TerrainType.Cave)
                    {
                        // Caves occupy lower strata of cave-designated columns.
                        float normalizedHeight = terrainHeight <= 1 ? 0f : y / (float)(terrainHeight - 1);
                        terrainData = normalizedHeight <= 0.65f ? caveData : groundData;
                    }
                    else
                    {
                        terrainData = groundData;
                    }

                    // Add any traversable node to potential obstacles list
                    // This allows obstacles to be placed on ground, cave, and other traversable terrains
                    if(!terrainData.IsBlocked)
                        potentialObstacles.Add(node);

                    node.UpdateNode(terrainData);
                }
            }
        }

        // Let the obstacle system decide which candidates become blocked.
        obstacleManager.Init(potentialObstacles);
        UpdateObstacles(config.ObstacleSeed, config.DensityThreshold);

        // Spawn only the visible chunk visuals.
        RefreshVisibleChunks(true);
    }

    public override void Clear()
    {
        // Remove any currently visible chunk visuals first.
        if (activeChunks.Count > 0)
        {
            foreach (var chunk in activeChunks.Values)
            {
                if (chunk != null && chunk.Root != null)
                    Destroy(chunk.Root);
            }
            activeChunks.Clear();
        }

        if (Nodes == null || Nodes.Length == 0)
        {
            Debug.LogWarning("No nodes to clear.");
            return;
        }

        // Clear the logical data grid.
        for (int x = 0; x < mGridSize; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                for (int z = 0; z < mGridSize; z++)
                {
                    if (Nodes[x, y, z] == null) continue;
                    Nodes[x, y, z].DestroyNode();
                    Nodes[x, y, z] = null;
                }
            }
        }

        Nodes = null;

        if (navPath != null && navPath.Count > 0)
            navPath.Clear();

        startNode = null;
        goalNode = null;
    }

    // Public grid API --------------------------------------------------------

    public void RefreshVisibleChunks(bool force = false)
    {
        if (Nodes == null)
            return;

        var center = GetCurrentChunkCenter();
        if (!force && center == lastVisibleChunkCenter)
            return;

        lastVisibleChunkCenter = center;

        // Decide which chunks should be visible around the focus point.
        var desiredChunks = new HashSet<Vector3Int>();
        for (int cx = center.x - viewRadiusInChunks; cx <= center.x + viewRadiusInChunks; cx++)
        {
            for (int cz = center.z - viewRadiusInChunks; cz <= center.z + viewRadiusInChunks; cz++)
            {
                var chunkCoord = new Vector3Int(cx, 0, cz);
                if (!IsChunkInsideGrid(chunkCoord))
                    continue;

                desiredChunks.Add(chunkCoord);
                if (!activeChunks.ContainsKey(chunkCoord))
                    LoadChunk(chunkCoord);
            }
        }

        // Destroy chunks that moved out of range.
        var toRemove = new List<Vector3Int>();
        foreach (var chunk in activeChunks.Keys)
        {
            if (!desiredChunks.Contains(chunk))
                toRemove.Add(chunk);
        }

        foreach (var chunkCoord in toRemove)
            UnloadChunk(chunkCoord);
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
                    if (dx == 0 && dy == 0 && dz == 0)
                        continue;

                    int nx = point.x + dx;
                    int ny = point.y + dy;
                    int nz = point.z + dz;

                    if (!IsInsideGrid(nx, ny, nz))
                        continue;

                    float distance = shape switch
                    {
                        CorridorShape.Cube => 1f,
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
        if (!IsInsideGrid(x, y, z))
            return null;

        return Nodes[x, y, z];
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

    public void UpdateObstacles(int seed, float densityThreshold = 0.5f) 
        => obstacleManager.UpdateObstacleDensityWithSeed(seed, densityThreshold);

    public void UpdateObstacles(float percent) => obstacleManager.UpdateObstacleDensity(percent);

    public Node[,,] GetAllNodes() => Nodes;

    protected void RemoveObstacles(float percent) => obstacleManager.Remove(percent);

    // Chunk helpers ----------------------------------------------------------

    private Vector3Int GetCurrentChunkCenter()
    {
        var nodeSpacing = terrainConfig.NodeSpacing;
        var focus = viewFocus != null ? viewFocus : Camera.main != null ? Camera.main.transform : transform;
        Vector3 local = focus.position - transform.position;
        int cellX = Mathf.FloorToInt(local.x / nodeSpacing);
        int cellZ = Mathf.FloorToInt(local.z / nodeSpacing);

        return new Vector3Int(
            Mathf.Clamp(cellX / chunkSize, 0, Mathf.Max(0, (mGridSize - 1) / chunkSize)),
            0,
            Mathf.Clamp(cellZ / chunkSize, 0, Mathf.Max(0, (mGridSize - 1) / chunkSize)));
    }

    private bool IsChunkInsideGrid(Vector3Int chunkCoord)
    {
        int chunkCount = Mathf.CeilToInt(mGridSize / (float)chunkSize);
        return chunkCoord.x >= 0 && chunkCoord.x < chunkCount &&
               chunkCoord.z >= 0 && chunkCoord.z < chunkCount;
    }

    private void LoadChunk(Vector3Int chunkCoord)
    {
        var state = new ChunkState
        {
            Root = new GameObject($"Chunk_{chunkCoord.x}_{chunkCoord.z}")
        };
        state.Root.transform.SetParent(transform, false);

        int startX = chunkCoord.x * chunkSize;
        int startZ = chunkCoord.z * chunkSize;
        int endX = Mathf.Min(startX + chunkSize, mGridSize);
        int endZ = Mathf.Min(startZ + chunkSize, mGridSize);

        for (int x = startX; x < endX; x++)
        {
            for (int z = startZ; z < endZ; z++)
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    var node = Nodes[x, y, z];
                    if (node == null)
                        continue;

                    var view = Instantiate(nodeObject, state.Root.transform);
                    view.name = $"Node_{x}_{y}_{z}";
                    view.transform.position = GetWorldPosition(x, y, z);
                    view.Bind(node);
                }
            }
        }

        activeChunks.Add(chunkCoord, state);
    }

    private void UnloadChunk(Vector3Int chunkCoord)
    {
        if (!activeChunks.TryGetValue(chunkCoord, out var state))
            return;

        if (state.Root != null)
            Destroy(state.Root);

        activeChunks.Remove(chunkCoord);
    }

    private Vector3 GetWorldPosition(int x, int y, int z)
    {
        var nodeSpacing = terrainConfig.NodeSpacing;

        float baseX = transform.position.x + x * nodeSpacing;
        float baseY = transform.position.y + y * nodeSpacing;
        float baseZ = transform.position.z + z * nodeSpacing;
        return new Vector3(baseX, baseY, baseZ);
    }

    private float AddNoiseXY(int x, int y, float offX, float offY)
    {
        return Mathf.PerlinNoise((x + offX) * noiseScale, (y + offY) * noiseScale);
    }
}
