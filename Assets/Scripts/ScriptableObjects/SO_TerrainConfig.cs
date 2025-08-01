using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainConfig", menuName = "AStar/TerrainConfig", order = 1)]
public class SO_TerrainConfig : ScriptableObject
{
    [SerializedDictionary("Terrain Type", "Data")] 
    [SerializeField] private SerializedDictionary<TerrainType, TerrainData> TerrainData;

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
}
