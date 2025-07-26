using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ThetaStar : BasePathfinding
{
    //private List<Vector3Int> AllDirections;
    //public List<Node> JPs;

    //private void Awake()
    //{
    //    AllDirections = JPSHelper.CreateAllDirectionList();
    //}

    //protected override PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
    //{
    //    var open = new PriorityQueue<Node, float>();
    //    var closed = new HashSet<Node>();

    //    // Initialize nodes
    //    Grid3D.Instance.ForEachNode(node => {
    //        if (node != null)
    //        {
    //            node.gCost = float.MaxValue;
    //            node.hCost = Heuristic(node, goal);
    //            node.parent = null;
    //        }
    //    });

    //    start.gCost = 0;
    //    start.parent = null;  // Changed from start.parent = start
    //    start.fCost = start.hCost;

    //    open.Enqueue(start, start.fCost);

    //    while (open.Count > 0)
    //    {
    //        var current = open.Dequeue();

    //        // Moved goal check after processing to ensure best path
    //        if (current == goal)
    //            return ReturnPath(start, goal, 0);

    //        closed.Add(current);

    //        foreach (var neighbor in current.GetNeighbors())
    //        {
    //            if (neighbor.isBlocked || closed.Contains(neighbor) ||
    //                (allowedNodes != null && !allowedNodes.Contains(neighbor)))
    //                continue;

    //            // Try path through parent first (Theta* optimization)
    //            if (current.parent != null && HasLineOfSight(current.parent, neighbor))
    //            {
    //                float tentativeG = current.parent.gCost + Distance(current.parent, neighbor);

    //                if (tentativeG < neighbor.gCost)
    //                {
    //                    neighbor.parent = current.parent;
    //                    UpdateNodeCosts(neighbor, goal);
    //                    EnqueueOrUpdate(open, neighbor);
    //                }
    //            }
    //            // Regular A* update
    //            else
    //            {
    //                float tentativeG = current.gCost + Distance(current, neighbor);

    //                if (tentativeG < neighbor.gCost)
    //                {
    //                    neighbor.parent = current;
    //                    UpdateNodeCosts(neighbor, goal);
    //                    EnqueueOrUpdate(open, neighbor);
    //                }
    //            }
    //        }
    //    }

    //    return null; // No path found
    //}
    //private void UpdateNodeCosts(Node node, Node goal)
    //{
    //    node.gCost = node.parent.gCost + Distance(node.parent, node);
    //    node.hCost = Heuristic(node, goal);
    //    node.fCost = node.gCost + node.hCost;
    //}

    //private void EnqueueOrUpdate(PriorityQueue<Node, float> open, Node node)
    //{
    //    if (!open.Contains(node))
    //        open.Enqueue(node, node.fCost);
    //    else
    //        open.UpdatePriority(node, node.fCost);
    //}

    //float Heuristic(Node a, Node b)
    //{
    //    return Vector3Int.Distance(a.GetNodePositionOnGrid(), b.GetNodePositionOnGrid());
    //}

    //float Distance(Node a, Node b)
    //{
    //    return Vector3Int.Distance(a.GetNodePositionOnGrid(), b.GetNodePositionOnGrid()); // Or gCost delta if cost varies
    //}

    //bool HasLineOfSight(Node from, Node to)
    //{
    //    Vector3Int fromPos = from.GetNodePositionOnGrid();
    //    Vector3Int toPos = to.GetNodePositionOnGrid();

    //    // Simple implementation - should be replaced with proper 3D line traversal
    //    float step = 0.1f;
    //    Vector3 direction = (toPos - fromPos).normalized;
    //    float distance = Vector3.Distance(fromPos, toPos);

    //    for (float t = 0; t <= distance; t += step)
    //    {
    //        Vector3 checkPoint = fromPos + direction * t;
    //        Node node = Grid3D.Instance.GetNodeAt(Vector3Int.RoundToInt(checkPoint));
    //        if (node == null || node.isBlocked)
    //            return false;
    //    }
    //    return true;
    //}
}