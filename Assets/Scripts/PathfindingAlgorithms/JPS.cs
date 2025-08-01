using System.Collections.Generic;
using UnityEngine;

public class JPS : BasePathfinding
{
    private List<Vector3Int> allDirections;

    private void Awake()
    {
        allDirections = NeighborHelper.CreateAllDirectionList();
    }

    protected override PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
    {
        var openList = new PriorityQueue<Node, float>();
        var closedSet = new HashSet<Node>();
        var directionMap = new Dictionary<Node, Vector3Int>();
        var visited = 0;

        start.gCost = 0;
        start.hCost = CalculateHeuristicDistance(start, goal);
        start.fCost = start.hCost;

        openList.Enqueue(start, start.fCost);
        directionMap[start] = Vector3Int.zero;

        while (openList.Count > 0)
        {
            var current = openList.Dequeue();
            closedSet.Add(current);

            if (current == goal)
            {
                var path = ReturnPath(start, goal, visited);
                return path;
            }

            var currentDir = directionMap[current];
            var successors = IdentifySuccessors(current, start, goal, currentDir);

            foreach (var jumpPoint in successors)
            {
                if (jumpPoint == null || closedSet.Contains(jumpPoint))
                    continue;

                float tentativeG = current.gCost + CalculateHeuristicDistance(current, jumpPoint);

                if (!openList.Contains(jumpPoint) || tentativeG < jumpPoint.gCost)
                {
                    jumpPoint.gCost = tentativeG;
                    jumpPoint.hCost = CalculateHeuristicDistance(jumpPoint, goal);
                    jumpPoint.fCost = jumpPoint.gCost + jumpPoint.hCost;
                    jumpPoint.parent = current;

                    Vector3Int dir = GridHelper.GetDirection(current, jumpPoint);
                    directionMap[jumpPoint] = dir;

                    if (!openList.Contains(jumpPoint))
                    {
                        openList.Enqueue(jumpPoint, jumpPoint.fCost);
                        visited++;
                    }
                    else
                    {
                        openList.UpdatePriority(jumpPoint, jumpPoint.fCost);
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// if will check if the current direction is zero, (this means that no direction is known yet)
    ///     if true: it will check all possible directions and return the jump points in all directions
    ///     else:
    ///         it would mean that the current direction is known as it was identified in the first run
    ///         therfore, it will only check the successors in the current direction
    ///         this means, it will only get the nodes that are reachable in the current direction (natural neighbors)
    ///         once you have these directions, you continue to jump in the same direction
    /// </summary>
    private List<Node> IdentifySuccessors(Node node, Node start, Node goal, Vector3Int currentDir)
    {
        var successors = new List<Node>();

        if (currentDir == Vector3Int.zero)
        {
            // First node: explore all directions
            foreach (var dir in allDirections)
            {
                var jumpPoint = Jump(node, goal, dir);
                if (jumpPoint != null)
                    successors.Add(jumpPoint);
            }
        }
        else
        {
            // Natural directions
            foreach (var dir in NeighborHelper.GetNaturalNeighbors(currentDir))
            {
                var jumpPoint = Jump(node, goal, dir);
                if (jumpPoint != null)
                    successors.Add(jumpPoint);
            }

            // Forced neighbors
            var forcedNeighbors = NeighborHelper.GetForcedNeighbors(node, currentDir);
            foreach (var forced in forcedNeighbors)
            {
                var dir = GridHelper.GetDirection(node, forced);
                var jumpPoint = Jump(node, goal, dir);
                if (jumpPoint != null)
                    successors.Add(jumpPoint);
            }
        }

        return successors;
    }

    /// <summary>
    /// This method will recursively check in the given direction to see:
    ///     if the goal can be reached
    ///     if the next node is blocked (forced neighbor)
    ///     if it can move further
    /// </summary>
    private Node Jump(Node current, Node goal, Vector3Int direction)
    {
        if (current == goal) return current;

        Vector3Int nextPos = current.GetNodePositionOnGrid() + direction;
        if (!Grid3D.Instance.IsInsideGrid(nextPos.x, nextPos.y, nextPos.z))
            return null;

        Node nextNode = Grid3D.Instance.GetNodeAt(nextPos);
        if (nextNode == null || nextNode.bIsBlocked)
            return null;

        if (nextNode == goal)
            return nextNode;

        // -- updates
        var forcedNeighbors = NeighborHelper.GetForcedNeighbors(nextNode, direction);
        if (forcedNeighbors != null && forcedNeighbors.Count > 0)
        {
            // Found forced neighbors, nextNode is a jump point
            return nextNode;
        }

        // Diagonal movement: check component directions
        if (IsDiagonal(direction))
        {
            // Check for jump points in component directions
            foreach (var component in GetComponentDirections(direction))
            {
                var jumpPoint = Jump(nextNode, goal, component);

                if (jumpPoint != null)
                {
                    jumpPoint.SetColor(Color.orange);
                    return nextNode;
                }
            }
        }

        // Recurse
        return Jump(nextNode, goal, direction);
    }

    private bool IsDiagonal(Vector3Int dir)
    {
        int nonZeroAxes = 0;
        if (dir.x != 0) nonZeroAxes++;
        if (dir.y != 0) nonZeroAxes++;
        if (dir.z != 0) nonZeroAxes++;
        return nonZeroAxes >= 2;
    }

    private List<Vector3Int> GetComponentDirections(Vector3Int dir)
    {
        var components = new List<Vector3Int>();

        if (dir.x != 0)
            components.Add(new Vector3Int(dir.x, 0, 0));
        if (dir.y != 0)
            components.Add(new Vector3Int(0, dir.y, 0));
        if (dir.z != 0)
            components.Add(new Vector3Int(0, 0, dir.z));

        return components;
    }
}