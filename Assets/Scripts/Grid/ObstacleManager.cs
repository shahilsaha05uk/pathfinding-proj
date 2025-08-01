using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    private List<Node> potentialNodes = new();
    private List<Node> obstacleNodes = new();

    private float currentPercent = 0f;
    [SerializeField] private SO_TerrainConfig terrainConfig;

    public void Init(List<Node> nodes)
    {
        if (potentialNodes != null || potentialNodes.Count > 0)
            potentialNodes.Clear();
        if (obstacleNodes != null || obstacleNodes.Count > 0)
            obstacleNodes.Clear();

        potentialNodes = nodes;
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

        currentPercent = newDensity;
    }

    public void Remove(float percent)
    {
        percent = Mathf.Clamp01(percent);

        float newPercent = currentPercent - percent;
        newPercent = Mathf.Clamp01(newPercent); // Don't go below 0
        if (newPercent == currentPercent) return;


        int currentObstacleCount = Mathf.FloorToInt(potentialNodes.Count * currentPercent);
        int newObstacleCount = Mathf.FloorToInt(potentialNodes.Count * newPercent);
        int countToRemove = currentObstacleCount - newObstacleCount;

        var shuffled = obstacleNodes.OrderBy(_ => Random.value).Take(countToRemove).ToList();

        foreach (var node in shuffled)
        {
            node.ResetNode();
            obstacleNodes.Remove(node);
        }

        currentPercent = newPercent;
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
        return Mathf.Clamp01(currentPercent); // Cap at 100%
    }
}
