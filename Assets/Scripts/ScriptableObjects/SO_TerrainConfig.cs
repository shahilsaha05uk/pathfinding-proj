using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainConfig", menuName = "AStar/TerrainConfig", order = 1)]
public class SO_TerrainConfig : ScriptableObject
{
    [SerializedDictionary("Terrain Type", "Data")] 
    [SerializeField] private SerializedDictionary<TerrainType, TerrainData> TerrainData;

    [SerializeField] private float MaxTraversalHeight;

    [Header("Cave Clustering")]
    [SerializeField] private float caveClusterScale = 0.08f;
    [SerializeField] [Range(0f, 1f)] private float caveClusterThreshold = 0.5f;

    public float TraversalHeight => MaxTraversalHeight - 1;
    public float CaveClusterScale => caveClusterScale;
    public float CaveClusterThreshold => caveClusterThreshold;

    public float NodeSpacing = 0.32f;

    public TerrainData GetData(TerrainType terrainType)
    {
        if (TerrainData.TryGetValue(terrainType, out var data))
        {
            return data;
        }
        else
        {
            Debug.LogError($"Terrain type {terrainType} not found in configuration.");
            return new TerrainData { Type = terrainType, Color = Color.black, IsBlocked = false };
        }
    }

    public TerrainData GetData(float noise)
    {
        // Find the first terrain entry where the noise falls within the inclusive range [HeightMin, HeightMax].
        foreach (var t in TerrainData.Values.OrderBy(v => v.HeightMin))
        {
            if (noise >= t.HeightMin && noise <= t.HeightMax)
                return t;
        }

        // Fallback to ground data if available.
        if (TerrainData.TryGetValue(TerrainType.Ground, out var defaultData))
            return defaultData;

        Debug.LogError($"No terrain found at noise level {noise}!!");
        return new TerrainData {
            Type = TerrainType.Obstacle,
            Color = Color.red,
            IsBlocked = true
        };
    }

    public bool IsCaveClusterPoint(int x, int z, float offsetX, float offsetY)
    {
        float safeScale = Mathf.Max(0.0001f, caveClusterScale);
        float clusterNoise = Mathf.PerlinNoise((x + offsetX) * safeScale, (z + offsetY) * safeScale);
        return clusterNoise >= caveClusterThreshold;
    }
}
