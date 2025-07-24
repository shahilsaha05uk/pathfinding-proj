using Assets.AStar.Scripts.Core.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;

public class JPS : BasePathfinding
{
    private List<Vector3Int> AllDirections;

    private void Awake()
    {
        AllDirections = GetAllDirections();
    }

    protected override PathResult FindPath(Node start, Node goal, HashSet<Node> allowedNodes = null)
    {
        // Initialise the data: openlist, closedList and direction tracker
        var openList = new PriorityQueue<Node, float>();
        var closedSet = new HashSet<Node>();
        var directionMap = new Dictionary<Node, Vector3Int>();
        var visited = 0;


        // initialise the start node  
        start.gCost = 0;
        start.hCost = HeuristicHelper.GetManhattanDistance(start, goal);
        start.fCost = start.hCost;

        // add the start node to the open list  
        openList.Enqueue(start, start.fCost);
        directionMap[start] = Vector3Int.zero;


        // while the open list is not empty  
        while (openList.Count > 0)
        {
            // get the current node
            var current = openList.Dequeue();
            closedSet.Add(current);

            // if the goal is reached, reconstruct the path and return the result
            if (current == goal)
                return ReturnPath(start, goal, visited);

            //// get all the neighbors of the current node
            //var currentDir = directionMap[current];
            //var directions = (currentDir == Vector3Int.zero) ? AllDirections : GetNaturalNeighbours(currentDir);
            //// for each of the neighbor:  
            //foreach (var dir in directions)
            //{
            //    // Get the next jump point
            //    Node jumpPoint = Jump(current, goal, dir);
            //    // move to the next direction if the jump point in the current direction wasnt found
            //    if (jumpPoint == null ||
            //        closedSet.Contains(jumpPoint))
            //        continue;
            //    // this is the estimated cost from the start node to the current node
            //    float tentativeG = current.gCost + HeuristicHelper.GetManhattanDistance(current, jumpPoint);
            //    if (!openList.Contains(jumpPoint) ||
            //        tentativeG < jumpPoint.gCost)
            //    {
            //        jumpPoint.gCost = tentativeG;
            //        jumpPoint.hCost = HeuristicHelper.GetManhattanDistance(jumpPoint, goal);
            //        jumpPoint.fCost = jumpPoint.gCost + jumpPoint.hCost;
            //        jumpPoint.parent = current;
            //        // add it to the open queue
            //        if (!openList.Contains(jumpPoint))
            //            openList.Enqueue(jumpPoint, jumpPoint.fCost);
            //        else
            //            openList.UpdatePriority(jumpPoint, jumpPoint.fCost);
            //        visited++;
            //    }
            //}
            //foreach (var forcedDir in GetForcedNeighbors(current, currentDir))
            //{
            //    Node jumpPoint = Jump(current, goal, forcedDir);
            //    if (jumpPoint != null ||
            //        !closedSet.Contains(jumpPoint))
            //        directions.Add(forcedDir);
            //}
        }

        return base.FindPath(start, goal, allowedNodes);
    }


    /// <summary>
    /// This method checks if the node has a forced neighbor in the given direction.
    /// 
    /// Orthogonals: these are the four cardinal directions 
    ///              (up, down, left, right) in 3D space. (total: 6)
    /// Diagonals: these are the eight diagonal directions in 3D space 
    ///            (up-left, up-right, down-left, down-right, etc.) (total: 20)
    /// This checks if any of the orthogonal neighbors are blocked,
    ///     if it is, then it gets the diagonal neighbors
    ///         it checks if any of the diagonal neighbors are blocked,
    ///             if they are, then it is a forced neighbor
    /// 
    /// </summary>
    private bool HasForcedNeighbor(Node node, Vector3Int direction)
    {
        var orthogonals = node.GetOrthogonalNeighbors();

        foreach (var o in orthogonals)
        {
            if(o == null || o.isBlocked)
            {
                // check the corresponding diagonal direction
                var diagonals = node.GetDiagonalNeighbors();

                // if the diagonal exists and is not blocked, its a forced neighbor
                foreach (var d in diagonals)
                {
                    if (d.isBlocked) return true;
                }
            }
        }
        return false;
    }

    private List<Vector3Int> GetAllDirections() => JPSHelper.CreateAllDirectionList();

    private Node GetNodeInDirection(Node node, Vector3Int direction)
    {
        if (node == null) return null;

        Vector3Int newPos = node.GetNodePositionOnGrid() + direction;
        return Grid3D.Instance.GetNodeAt(newPos);
    }

    private Vector3Int GetDirection(Node n1, Node n2)
    {
        return Vector3Int.zero;
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
        
        // 
        if (currentDir == Vector3Int.zero)
        {
            foreach (var dir in AllDirections)
            {
                var jumpPoint = Jump(node, goal, dir);
                successors.Add(jumpPoint);
            }
            return successors;
        }
        else
        {
            foreach (var dir in GetNaturalNeighbours(currentDir))
            {
                var jumpPoint = Jump(node, goal, dir);
                successors.Add(jumpPoint);
            }
            foreach (var dir in GetForcedNeighbors(node, currentDir))
            {
                var jumpPoint = Jump(node, goal, dir);
                successors.Add(jumpPoint);
            }
        }

        return null;
    }

    /// <summary>
    /// This method will recursively check in the given direction to see:
    ///     if the goal can be reached
    ///     if the next node is blocked (forced neighbor)
    ///     if it can move further
    /// </summary>
    private Node Jump(Node current, Node goal, Vector3Int direction)
    {
        int x = current.gridX + direction.x;
        int y = current.gridY + direction.y;
        int z = current.gridZ + direction.z;

        // return nothing if the expected node is outside the grid
        if (!Grid3D.Instance.IsInsideGrid(x, y, z))
            return null;

        Node nextNode = Grid3D.Instance.GetNodeAt(new Vector3Int(x, y, z));

        // return nothing if the next node is null or blocked
        if (nextNode == null || nextNode.isBlocked)
            return null;

        // if the next node is the goal, return it
        if (nextNode == goal)
            return nextNode;

        // if the next node has a forced neighbor in the direction, return it
        if (HasForcedNeighbor(nextNode, direction))
            return nextNode;

        // recursively jump in the same direction until the end
        return Jump(nextNode, goal, direction);
    }

    /// <summary>
    ///     so for instance:
    ///         you start from the origin (0, 0, 0) // lets assume Delhi and North is Kashmir
    ///         you want to go towards Nepal which is North-East (1, 0, 1)
    ///         you move two steps in that direction, and you reach (3, 0, 3)
    ///         your current direction is North-East
    ///         so your natural direction (successors) would be: 
    ///             North (0, 0, 1), 
    ///             East (1, 0, 0), 
    ///             North-East(1, 0, 1)
    /// </summary>
    private List<Vector3Int> GetNaturalNeighbours(Vector3Int direction)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        if (direction == Vector3Int.zero)
            return GetAllDirections();

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

    /// <summary>
    /// This method tries to get all the possible directions from the current node in the given direction.
    /// So basically, if the idea is to get to Nepal and I am looking North-East, 
    /// it will check the next nodes in that direction 
    ///     and get the forced neighbors if there is any
    /// </summary>
    private List<Vector3Int> GetForcedNeighbors(Node current, Vector3Int currentDir)
    {
        List<Vector3Int> forcedNeighbors = new List<Vector3Int>();

        if(!JPSHelper.TryGetForcedNeighborRule(currentDir, out var rule))
            return forcedNeighbors;

        foreach (var (blockedOffset, openOffset) in rule)
        {
            var blockedNode = GetNodeInDirection(current, blockedOffset);

            if(blockedNode != null && blockedNode.isBlocked)
            {
                var node = GetNodeInDirection(current, openOffset);
                if (node != null && !node.isBlocked)
                {
                    forcedNeighbors.Add(openOffset);
                }
            }
        }

        return forcedNeighbors;
    }
}

// these directions represent the 26 possible moves in a 3D grid, including diagonal moves.

//List<Vector3Int> directions = new List<Vector3Int>
//{
//    new Vector3Int(-1, -1, -1), // Backward-Down-Left
//    new Vector3Int(-1, -1,  0), // Down-Left
//    new Vector3Int(-1, -1,  1), // Forward-Down-Left
//    new Vector3Int( 0, -1, -1), // Backward-Down
//    new Vector3Int( 0, -1,  0), // Down
//    new Vector3Int( 0, -1,  1), // Forward-Down
//    new Vector3Int( 1, -1, -1), // Backward-Down-Right
//    new Vector3Int( 1, -1,  0), // Down-Right
//    new Vector3Int( 1, -1,  1), // Forward-Down-Right

//    new Vector3Int(-1,  0, -1), // Backward-Left
//    new Vector3Int(-1,  0,  0), // Left
//    new Vector3Int(-1,  0,  1), // Forward-Left
//    new Vector3Int( 0,  0, -1), // Backward
//    new Vector3Int(0, 0, 0),    // (Omitted) Origin
//    new Vector3Int( 0,  0,  1), // Forward
//    new Vector3Int( 1,  0, -1), // Backward-Right
//    new Vector3Int( 1,  0,  0), // Right
//    new Vector3Int( 1,  0,  1), // Forward-Right

//    new Vector3Int(-1,  1, -1), // Backward-Up-Left
//    new Vector3Int(-1,  1,  0), // Up-Left
//    new Vector3Int(-1,  1,  1), // Forward-Up-Left
//    new Vector3Int( 0,  1, -1), // Backward-Up
//    new Vector3Int( 0,  1,  0), // Up
//    new Vector3Int( 0,  1,  1), // Forward-Up
//    new Vector3Int( 1,  1, -1), // Backward-Up-Right
//    new Vector3Int( 1,  1,  0), // Up-Right
//    new Vector3Int( 1,  1,  1), // Forward-Up-Right
//};

