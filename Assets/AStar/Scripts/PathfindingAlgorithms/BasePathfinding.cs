
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public abstract class BasePathfinding : MonoBehaviour
{
    public PathResult Navigate(Node start, Node end, HashSet<Node> allowedNodes = null, bool trackStats = true)
    {
        if (trackStats)
        {
            var (result, stats) = Stats.RecordStats(() => FindPath(start, end, allowedNodes));

            if (result != null)
            {
                result.TimeTaken = stats.TimeTaken;
                return result;
            }
            return null;
        }
        return FindPath(start, end, allowedNodes) ?? null;
    }

    protected virtual PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null) => null;
}
