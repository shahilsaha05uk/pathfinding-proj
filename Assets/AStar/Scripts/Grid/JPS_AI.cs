using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class JPSAI : BasePathfinding
{
    // Directions array for 3D (26 directions)
    private static readonly Vector3Int[] AllDirections =
    {
        new Vector3Int(-1,-1,-1), new Vector3Int(-1,-1,0), new Vector3Int(-1,-1,1),
        new Vector3Int(-1,0,-1), new Vector3Int(-1,0,0), new Vector3Int(-1,0,1),
        new Vector3Int(-1,1,-1), new Vector3Int(-1,1,0), new Vector3Int(-1,1,1),
        new Vector3Int(0,-1,-1), new Vector3Int(0,-1,0), new Vector3Int(0,-1,1),
        new Vector3Int(0,0,-1), /* (0,0,0) skipped */ new Vector3Int(0,0,1),
        new Vector3Int(0,1,-1), new Vector3Int(0,1,0), new Vector3Int(0,1,1),
        new Vector3Int(1,-1,-1), new Vector3Int(1,-1,0), new Vector3Int(1,-1,1),
        new Vector3Int(1,0,-1), new Vector3Int(1,0,0), new Vector3Int(1,0,1),
        new Vector3Int(1,1,-1), new Vector3Int(1,1,0), new Vector3Int(1,1,1)
    };

    protected override PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
    {
        var openList = new PriorityQueue<Node, float>();
        var closedSet = new HashSet<Node>();
        var directionMap = new Dictionary<Node, Vector3Int>();
        int visited = 0;

        // Initialize start node
        start.gCost = 0;
        start.hCost = HeuristicHelper.GetDiagonalDistance(start, goal);
        start.fCost = start.hCost;
        openList.Enqueue(start, start.fCost);
        directionMap[start] = Vector3Int.zero;

        while (openList.Count > 0)
        {
            var current = openList.Dequeue();
            closedSet.Add(current);

            if (current == goal)
                return ReturnPath(start, goal, visited);

            Vector3Int currentDir = directionMap[current];
            var successors = IdentifySuccessors(current, start, goal, currentDir);

            foreach (var jumpPoint in successors)
            {
                if (jumpPoint == null || closedSet.Contains(jumpPoint))
                    continue;

                float tentativeG = current.gCost + HeuristicHelper.GetDiagonalDistance(current, jumpPoint);

                if (!openList.Contains(jumpPoint) || tentativeG < jumpPoint.gCost)
                {
                    jumpPoint.gCost = tentativeG;
                    jumpPoint.hCost = HeuristicHelper.GetDiagonalDistance(jumpPoint, goal);
                    jumpPoint.fCost = jumpPoint.gCost + jumpPoint.hCost;
                    jumpPoint.parent = current;

                    Vector3Int dir = GetDirection(current, jumpPoint);
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

    private List<Node> IdentifySuccessors(Node node, Node start, Node goal, Vector3Int currentDir)
    {
        List<Node> successors = new List<Node>();

        if (currentDir == Vector3Int.zero)
        {
            // Start node - check all directions
            foreach (var dir in AllDirections)
            {
                Node jumpPoint = Jump(node, goal, dir);
                if (jumpPoint != null) successors.Add(jumpPoint);
            }
        }
        else
        {
            // Get natural neighbors based on current direction
            foreach (var dir in GetNaturalNeighbors(currentDir))
            {
                Node jumpPoint = Jump(node, goal, dir);
                if (jumpPoint != null) successors.Add(jumpPoint);
            }

            // Check for forced neighbors
            foreach (var dir in GetForcedNeighbors(node, currentDir))
            {
                Node jumpPoint = Jump(node, goal, dir);
                if (jumpPoint != null) successors.Add(jumpPoint);
            }
        }

        return successors;
    }

    private List<Vector3Int> GetNaturalNeighbors(Vector3Int direction)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        if (direction == Vector3Int.zero)
            return new List<Vector3Int>(AllDirections);

        // Cardinal directions
        if (direction.x != 0 && direction.y == 0 && direction.z == 0)
        {
            neighbors.Add(direction);
        }
        else if (direction.x == 0 && direction.y != 0 && direction.z == 0)
        {
            neighbors.Add(direction);
        }
        else if (direction.x == 0 && direction.y == 0 && direction.z != 0)
        {
            neighbors.Add(direction);
        }
        // Diagonal moves (combinations of two axes)
        else if (direction.x != 0 && direction.y != 0 && direction.z == 0)
        {
            neighbors.Add(new Vector3Int(direction.x, 0, 0));
            neighbors.Add(new Vector3Int(0, direction.y, 0));
            neighbors.Add(direction);
        }
        else if (direction.x != 0 && direction.y == 0 && direction.z != 0)
        {
            neighbors.Add(new Vector3Int(direction.x, 0, 0));
            neighbors.Add(new Vector3Int(0, 0, direction.z));
            neighbors.Add(direction);
        }
        else if (direction.x == 0 && direction.y != 0 && direction.z != 0)
        {
            neighbors.Add(new Vector3Int(0, direction.y, 0));
            neighbors.Add(new Vector3Int(0, 0, direction.z));
            neighbors.Add(direction);
        }
        // Full 3D diagonal (all three axes)
        else
        {
            neighbors.Add(new Vector3Int(direction.x, 0, 0));
            neighbors.Add(new Vector3Int(0, direction.y, 0));
            neighbors.Add(new Vector3Int(0, 0, direction.z));
            neighbors.Add(new Vector3Int(direction.x, direction.y, 0));
            neighbors.Add(new Vector3Int(direction.x, 0, direction.z));
            neighbors.Add(new Vector3Int(0, direction.y, direction.z));
            neighbors.Add(direction);
        }

        return neighbors;
    }

    private List<Vector3Int> GetForcedNeighbors(Node node, Vector3Int direction)
    {
        List<Vector3Int> forcedNeighbors = new List<Vector3Int>();

        return forcedNeighbors;
    }

    private Node Jump(Node current, Node goal, Vector3Int direction)
    {
        Node next = GetNodeInDirection(current, direction);

        if (next == null || next.isBlocked)
            return null;

        if (next == goal)
            return next;

        // Check for forced neighbors (condition 2 in paper)
        if (HasForcedNeighbor(next, direction))
            return next;

        // For diagonal moves, check straight directions (condition 3 in paper)
        if (direction.x != 0 && direction.y != 0 && direction.z != 0)
        {
            // Check both straight sub-directions
            foreach (var subDir in GetStraightSubDirections(direction))
            {
                if (Jump(next, goal, subDir) != null)
                    return next;
            }
        }

        // Continue jumping in same direction
        return Jump(next, goal, direction);
    }

    private bool HasForcedNeighbor(Node node, Vector3Int direction)
    {
        // Implement based on paper's Definition 1
        // Need to check for obstacles that would force certain neighbors
        // This is a simplified version - full implementation would need
        // to handle all direction cases

        // Example for one case:
        if (direction.x > 0 && direction.y == 0 && direction.z == 0) // Moving right
        {
            Node above = GetNodeInDirection(node, new Vector3Int(0, 1, 0));
            Node below = GetNodeInDirection(node, new Vector3Int(0, -1, 0));

            if ((above != null && above.isBlocked) ||
                (below != null && below.isBlocked))
                return true;
        }
        // Need to implement all other cases

        return false;
    }

    private bool IsNaturalNeighbor(Vector3Int offset, Vector3Int direction)
    {
        // A neighbor is natural if each of its components is either zero or matches the direction
        return
            (offset.x == 0 || offset.x == direction.x) &&
            (offset.y == 0 || offset.y == direction.y) &&
            (offset.z == 0 || offset.z == direction.z);
    }

    private List<Vector3Int> GetStraightSubDirections(Vector3Int diagonalDir)
    {
        List<Vector3Int> subDirs = new List<Vector3Int>();

        if (diagonalDir.x != 0) subDirs.Add(new Vector3Int(diagonalDir.x, 0, 0));
        if (diagonalDir.y != 0) subDirs.Add(new Vector3Int(0, diagonalDir.y, 0));
        if (diagonalDir.z != 0) subDirs.Add(new Vector3Int(0, 0, diagonalDir.z));

        return subDirs;
    }

    private Vector3Int GetDirection(Node from, Node to)
    {
        if (from == null || to == null) return Vector3Int.zero;

        Vector3Int dir = to.GetNodePositionOnGrid() - from.GetNodePositionOnGrid();
        return new Vector3Int(
            Mathf.Clamp(dir.x, -1, 1),
            Mathf.Clamp(dir.y, -1, 1),
            Mathf.Clamp(dir.z, -1, 1));
    }

    private Node GetNodeInDirection(Node node, Vector3Int direction)
    {
        if (node == null) return null;

        Vector3Int newPos = node.GetNodePositionOnGrid() + direction;
        return Grid3D.Instance.GetNodeAt(newPos);
    }
}