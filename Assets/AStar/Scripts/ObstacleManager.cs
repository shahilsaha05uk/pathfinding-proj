using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    private List<Node> PotentialNodes = new();
    private List<Node> ObstacleNodes = new();

    private float currentPercent = 0f;
    [SerializeField] private SO_TerrainConfig terrainConfig;

    public void Init(List<Node> nodes)
    {
        if (PotentialNodes != null || PotentialNodes.Count > 0)
            PotentialNodes.Clear();
        if (ObstacleNodes != null || ObstacleNodes.Count > 0)
            ObstacleNodes.Clear();

        PotentialNodes = nodes;
    }

    public void UpdateObstacleDensity(float newDensity)
    {
        newDensity = Mathf.Clamp01(newDensity);

        if (PotentialNodes == null || PotentialNodes.Count == 0)
            return;

        int targetCount = Mathf.FloorToInt(PotentialNodes.Count * newDensity);
        int currentCount = ObstacleNodes.Count;
        int difference = targetCount - currentCount;

        if (difference > 0)
        {
            var availableNodes = PotentialNodes.Except(ObstacleNodes).ToList();
            var nodesToAdd = availableNodes.OrderBy(_ => Random.value).Take(difference).ToList();

            foreach (var node in nodesToAdd)
            {
                node.UpdateNode(terrainConfig.GetData(TerrainType.Obstacle));
                ObstacleNodes.Add(node);
            }
        }
        else if (difference < 0)
        {
            var nodesToRemove = ObstacleNodes.OrderBy(_ => Random.value).Take(-difference).ToList();

            foreach (var node in nodesToRemove)
            {
                node.ResetNode();
                ObstacleNodes.Remove(node);
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


        int currentObstacleCount = Mathf.FloorToInt(PotentialNodes.Count * currentPercent);
        int newObstacleCount = Mathf.FloorToInt(PotentialNodes.Count * newPercent);
        int countToRemove = currentObstacleCount - newObstacleCount;

        var shuffled = ObstacleNodes.OrderBy(_ => Random.value).Take(countToRemove).ToList();

        foreach (var node in shuffled)
        {
            node.ResetNode();
            ObstacleNodes.Remove(node);
        }

        currentPercent = newPercent;
    }

    public void Clear()
    {
        foreach (var node in ObstacleNodes)
        {
            node.ResetNode();
        }

        ObstacleNodes.Clear();
    }

    public float GetCurrentPercent()
    {
        return Mathf.Clamp01(currentPercent); // Cap at 100%
    }
}
