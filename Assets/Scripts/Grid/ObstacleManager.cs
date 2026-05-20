using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    private List<Node> potentialNodes = new();
    private List<Node> obstacleNodes = new();

    [SerializeField] private SO_TerrainConfig terrainConfig;

    public void Init(List<Node> nodes)
    {
        if (potentialNodes != null || potentialNodes.Count > 0)
            potentialNodes.Clear();
        if (obstacleNodes != null || obstacleNodes.Count > 0)
            obstacleNodes.Clear();

        potentialNodes = nodes;
    }

    /// <summary>
    /// Generate obstacles using Perlin noise clustering based on a seed.
    /// This creates terrain-like obstacle clusters instead of random scattered obstacles.
    /// The seed deterministically affects the Perlin noise pattern, ensuring different
    /// seeds produce different obstacle distributions while remaining reproducible.
    /// </summary>
    public void UpdateObstacleDensityWithSeed(
        int seed, 
        float densityThreshold = 0.5f, 
        float scale = 0.1f)
    {
        if (potentialNodes == null || potentialNodes.Count == 0)
            return;

        // Clear existing obstacles
        Clear();

        // Derive deterministic offsets directly from seed
        // This ensures different seeds produce different patterns
        float offsetX = (seed % 1000) + (seed / 1000f);
        float offsetY = ((seed * 7) % 1000) + ((seed * 7) / 1000f);
        float offsetZ = ((seed * 13) % 1000) + ((seed * 13) / 1000f);

        // Create obstacles based on Perlin noise
        foreach (var node in potentialNodes)
        {
            var pos = node.GetNodePositionOnGrid();

            // Sample Perlin noise at this position using seed-derived offsets
            float noiseValue = Mathf.PerlinNoise(
                (pos.x + offsetX) * scale,
                (pos.z + offsetZ) * scale
            );

            // Threshold for obstacle placement (tune this to adjust density)
            // densityThreshold = 0.3f → more obstacles (~70%)
            // densityThreshold = 0.5f → medium obstacles (~50%)
            // densityThreshold = 0.7f → fewer obstacles (~30%)
            if (noiseValue > densityThreshold)
            {
                node.UpdateNode(terrainConfig.GetData(TerrainType.Obstacle));
                obstacleNodes.Add(node);
            }
        }
    }

    public void UpdateObstacleDensity(float newDensity)
    {
        newDensity = Mathf.Clamp01(newDensity);

        if (potentialNodes == null || potentialNodes.Count == 0)
            return;

        int targetCount = Mathf.FloorToInt(potentialNodes.Count * newDensity);
        int currentCount = obstacleNodes.Count;
        int difference = targetCount - currentCount;

        if (difference > 0)
        {
            var availableNodes = potentialNodes.Except(obstacleNodes).ToList();
            var nodesToAdd = availableNodes.OrderBy(_ => Random.value).Take(difference).ToList();

            foreach (var node in nodesToAdd)
            {
                node.UpdateNode(terrainConfig.GetData(TerrainType.Obstacle));
                obstacleNodes.Add(node);
            }
        }
        else if (difference < 0)
        {
            var nodesToRemove = obstacleNodes.OrderBy(_ => Random.value).Take(-difference).ToList();

            foreach (var node in nodesToRemove)
            {
                node.ResetNode();
                obstacleNodes.Remove(node);
            }
        }
    }

    public void Remove(float percent)
    {
        percent = Mathf.Clamp01(percent);

        float currentPercent = (float)obstacleNodes.Count / potentialNodes.Count;
        float newPercent = currentPercent - percent;
        newPercent = Mathf.Clamp01(newPercent);
        if (newPercent == currentPercent) return;

        int currentObstacleCount = obstacleNodes.Count;
        int newObstacleCount = Mathf.FloorToInt(potentialNodes.Count * newPercent);
        int countToRemove = currentObstacleCount - newObstacleCount;

        var shuffled = obstacleNodes.OrderBy(_ => Random.value).Take(countToRemove).ToList();

        foreach (var node in shuffled)
        {
            node.ResetNode();
            obstacleNodes.Remove(node);
        }
    }

    public void Clear()
    {
        foreach (var node in obstacleNodes)
        {
            node.ResetNode();
        }

        obstacleNodes.Clear();
    }

    public float GetCurrentPercent()
    {
        if (potentialNodes == null || potentialNodes.Count == 0)
            return 0f;
        return Mathf.Clamp01((float)obstacleNodes.Count / potentialNodes.Count);
    }
}
