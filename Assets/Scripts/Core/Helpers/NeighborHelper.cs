
using System.Collections.Generic;
using UnityEngine;

public static class NeighborHelper
{
    private static Dictionary<Vector3Int, List<(Vector3Int BlockedOffset, Vector3Int OpenOffset)>> ForcedNeighborRules = new()
{
        // Cardinal directions (6)
        {
            new Vector3Int(1, 0, 0), // Moving right (+X)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0)),    // If north is blocked, NE is forced
                (new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0)),  // If south is blocked, SE is forced
                (new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 1)),    // If forward is blocked, forward-right is forced
                (new Vector3Int(0, 0, -1), new Vector3Int(1, 0, -1))   // If backward is blocked, backward-right is forced
            }
        },
        {
            new Vector3Int(-1, 0, 0), // Moving left (-X)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, 0)),   // If north is blocked, NW is forced
                (new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, 0)), // If south is blocked, SW is forced
                (new Vector3Int(0, 0, 1), new Vector3Int(-1, 0, 1)),   // If forward is blocked, forward-left is forced
                (new Vector3Int(0, 0, -1), new Vector3Int(-1, 0, -1))  // If backward is blocked, backward-left is forced
            }
        },
        {
            new Vector3Int(0, 1, 0), // Moving up (+Y)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 0)),    // If east is blocked, NE is forced
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, 1, 0)),  // If west is blocked, NW is forced
                (new Vector3Int(0, 0, 1), new Vector3Int(0, 1, 1)),    // If forward is blocked, up-forward is forced
                (new Vector3Int(0, 0, -1), new Vector3Int(0, 1, -1))   // If backward is blocked, up-backward is forced
            }
        },
        {
            new Vector3Int(0, -1, 0), // Moving down (-Y)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(1, 0, 0), new Vector3Int(1, -1, 0)),   // If east is blocked, SE is forced
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, 0)), // If west is blocked, SW is forced
                (new Vector3Int(0, 0, 1), new Vector3Int(0, -1, 1)),   // If forward is blocked, down-forward is forced
                (new Vector3Int(0, 0, -1), new Vector3Int(0, -1, -1))  // If backward is blocked, down-backward is forced
            }
        },
        {
            new Vector3Int(0, 0, 1), // Moving forward (+Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(1, 0, 0), new Vector3Int(1, 0, 1)),    // If east is blocked, forward-right is forced
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, 0, 1)),  // If west is blocked, forward-left is forced
                (new Vector3Int(0, 1, 0), new Vector3Int(0, 1, 1)),    // If up is blocked, forward-up is forced
                (new Vector3Int(0, -1, 0), new Vector3Int(0, -1, 1))   // If down is blocked, forward-down is forced
            }
        },
        {
            new Vector3Int(0, 0, -1), // Moving backward (-Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(1, 0, 0), new Vector3Int(1, 0, -1)),   // If east is blocked, backward-right is forced
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, 0, -1)), // If west is blocked, backward-left is forced
                (new Vector3Int(0, 1, 0), new Vector3Int(0, 1, -1)),   // If up is blocked, backward-up is forced
                (new Vector3Int(0, -1, 0), new Vector3Int(0, -1, -1))  // If down is blocked, backward-down is forced
            }
        },

        // Diagonal directions in XY plane (4)
        {
            new Vector3Int(1, 1, 0), // NE diagonal (+X+Y)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 1)),    // If north is blocked, NE-forward is forced
                (new Vector3Int(0, 1, 0), new Vector3Int(1, 1, -1)),   // If north is blocked, NE-backward is forced
                (new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 1)),    // If east is blocked, NE-forward is forced
                (new Vector3Int(1, 0, 0), new Vector3Int(1, 1, -1))    // If east is blocked, NE-backward is forced
            }
        },
        {
            new Vector3Int(1, -1, 0), // SE diagonal (+X-Y)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 1)),  // If south is blocked, SE-forward is forced
                (new Vector3Int(0, -1, 0), new Vector3Int(1, -1, -1)), // If south is blocked, SE-backward is forced
                (new Vector3Int(1, 0, 0), new Vector3Int(1, -1, 1)),   // If east is blocked, SE-forward is forced
                (new Vector3Int(1, 0, 0), new Vector3Int(1, -1, -1))   // If east is blocked, SE-backward is forced
            }
        },
        {
            new Vector3Int(-1, 1, 0), // NW diagonal (-X+Y)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, 1)),   // If north is blocked, NW-forward is forced
                (new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, -1)),  // If north is blocked, NW-backward is forced
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, 1, 1)),  // If west is blocked, NW-forward is forced
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, 1, -1))  // If west is blocked, NW-backward is forced
            }
        },
        {
            new Vector3Int(-1, -1, 0), // SW diagonal (-X-Y)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, 1)), // If south is blocked, SW-forward is forced
                (new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, -1)),// If south is blocked, SW-backward is forced
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, 1)), // If west is blocked, SW-forward is forced
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, -1)) // If west is blocked, SW-backward is forced
            }
        },

        // Diagonal directions in XZ plane (4)
        {
            new Vector3Int(1, 0, 1), // Forward-right (+X+Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 0, 1), new Vector3Int(1, 1, 1)),    // If forward is blocked, forward-right-up is forced
                (new Vector3Int(0, 0, 1), new Vector3Int(1, -1, 1)),   // If forward is blocked, forward-right-down is forced
                (new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 1)),    // If right is blocked, forward-right-up is forced
                (new Vector3Int(1, 0, 0), new Vector3Int(1, -1, 1))    // If right is blocked, forward-right-down is forced
            }
        },
        {
            new Vector3Int(1, 0, -1), // Backward-right (+X-Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 0, -1), new Vector3Int(1, 1, -1)),  // If backward is blocked, backward-right-up is forced
                (new Vector3Int(0, 0, -1), new Vector3Int(1, -1, -1)), // If backward is blocked, backward-right-down is forced
                (new Vector3Int(1, 0, 0), new Vector3Int(1, 1, -1)),   // If right is blocked, backward-right-up is forced
                (new Vector3Int(1, 0, 0), new Vector3Int(1, -1, -1))   // If right is blocked, backward-right-down is forced
            }
        },
        {
            new Vector3Int(-1, 0, 1), // Forward-left (-X+Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 0, 1), new Vector3Int(-1, 1, 1)),   // If forward is blocked, forward-left-up is forced
                (new Vector3Int(0, 0, 1), new Vector3Int(-1, -1, 1)),  // If forward is blocked, forward-left-down is forced
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, 1, 1)),  // If left is blocked, forward-left-up is forced
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, 1))  // If left is blocked, forward-left-down is forced
            }
        },
        {
            new Vector3Int(-1, 0, -1), // Backward-left (-X-Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 0, -1), new Vector3Int(-1, 1, -1)), // If backward is blocked, backward-left-up is forced
                (new Vector3Int(0, 0, -1), new Vector3Int(-1, -1, -1)),// If backward is blocked, backward-left-down is forced
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, 1, -1)), // If left is blocked, backward-left-up is forced
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, -1)) // If left is blocked, backward-left-down is forced
            }
        },

        // Diagonal directions in YZ plane (4)
        {
            new Vector3Int(0, 1, 1), // Forward-up (+Y+Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 0, 1), new Vector3Int(1, 1, 1)),    // If forward is blocked, forward-up-right is forced
                (new Vector3Int(0, 0, 1), new Vector3Int(-1, 1, 1)),   // If forward is blocked, forward-up-left is forced
                (new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 1)),    // If up is blocked, forward-up-right is forced
                (new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, 1))    // If up is blocked, forward-up-left is forced
            }
        },
        {
            new Vector3Int(0, 1, -1), // Backward-up (+Y-Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 0, -1), new Vector3Int(1, 1, -1)),  // If backward is blocked, backward-up-right is forced
                (new Vector3Int(0, 0, -1), new Vector3Int(-1, 1, -1)), // If backward is blocked, backward-up-left is forced
                (new Vector3Int(0, 1, 0), new Vector3Int(1, 1, -1)),   // If up is blocked, backward-up-right is forced
                (new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, -1))   // If up is blocked, backward-up-left is forced
            }
        },
        {
            new Vector3Int(0, -1, 1), // Forward-down (-Y+Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 0, 1), new Vector3Int(1, -1, 1)),   // If forward is blocked, forward-down-right is forced
                (new Vector3Int(0, 0, 1), new Vector3Int(-1, -1, 1)),  // If forward is blocked, forward-down-left is forced
                (new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 1)),  // If down is blocked, forward-down-right is forced
                (new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, 1))  // If down is blocked, forward-down-left is forced
            }
        },
        {
            new Vector3Int(0, -1, -1), // Backward-down (-Y-Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 0, -1), new Vector3Int(1, -1, -1)),  // If backward is blocked, backward-down-right is forced
                (new Vector3Int(0, 0, -1), new Vector3Int(-1, -1, -1)), // If backward is blocked, backward-down-left is forced
                (new Vector3Int(0, -1, 0), new Vector3Int(1, -1, -1)),  // If down is blocked, backward-down-right is forced
                (new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, -1))  // If down is blocked, backward-down-left is forced
            }
        },

        // Full 3D diagonals (8 - combinations of all 3 axes)
        {
            new Vector3Int(1, 1, 1), // NE-forward (+X+Y+Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 1, 1), new Vector3Int(1, 1, 1)),    // If north-forward is blocked
                (new Vector3Int(1, 0, 1), new Vector3Int(1, 1, 1)),    // If east-forward is blocked
                (new Vector3Int(1, 1, 0), new Vector3Int(1, 1, 1)),    // If NE is blocked
                (new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 1)),    // If north is blocked
                (new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 1)),    // If east is blocked
                (new Vector3Int(0, 0, 1), new Vector3Int(1, 1, 1))     // If forward is blocked
            }
        },
        {
            new Vector3Int(1, 1, -1), // NE-backward (+X+Y-Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 1, -1), new Vector3Int(1, 1, -1)),  // If north-backward is blocked
                (new Vector3Int(1, 0, -1), new Vector3Int(1, 1, -1)),  // If east-backward is blocked
                (new Vector3Int(1, 1, 0), new Vector3Int(1, 1, -1)),   // If NE is blocked
                (new Vector3Int(0, 1, 0), new Vector3Int(1, 1, -1)),   // If north is blocked
                (new Vector3Int(1, 0, 0), new Vector3Int(1, 1, -1)),   // If east is blocked
                (new Vector3Int(0, 0, -1), new Vector3Int(1, 1, -1))   // If backward is blocked
            }
        },
        // ... (similar patterns for other 3D diagonals)
        {
            new Vector3Int(1, -1, 1), // SE-forward (+X-Y+Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, -1, 1), new Vector3Int(1, -1, 1)),  // If south-forward is blocked
                (new Vector3Int(1, 0, 1), new Vector3Int(1, -1, 1)),   // If east-forward is blocked
                (new Vector3Int(1, -1, 0), new Vector3Int(1, -1, 1)),  // If SE is blocked
                (new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 1)),  // If south is blocked
                (new Vector3Int(1, 0, 0), new Vector3Int(1, -1, 1)),   // If east is blocked
                (new Vector3Int(0, 0, 1), new Vector3Int(1, -1, 1))    // If forward is blocked
            }
        },
        {
            new Vector3Int(1, -1, -1), // SE-backward (+X-Y-Z)
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, -1, -1), new Vector3Int(1, -1, -1)),// If south-backward is blocked
                (new Vector3Int(1, 0, -1), new Vector3Int(1, -1, -1)), // If east-backward is blocked
                (new Vector3Int(1, -1, 0), new Vector3Int(1, -1, -1)), // If SE is blocked
                (new Vector3Int(0, -1, 0), new Vector3Int(1, -1, -1)), // If south is blocked
                (new Vector3Int(1, 0, 0), new Vector3Int(1, -1, -1)),  // If east is blocked
                (new Vector3Int(0, 0, -1), new Vector3Int(1, -1, -1))  // If backward is blocked
            }
        },
        {
            new Vector3Int(-1, 1, 1),
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 1, 1), new Vector3Int(-1, 1, 1)),   // north-forward blocked
                (new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 1)),  // west-forward blocked
                (new Vector3Int(-1, 1, 0), new Vector3Int(-1, 1, 1)),  // NW blocked
                (new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, 1)),   // north blocked
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, 1, 1)),  // west blocked
                (new Vector3Int(0, 0, 1), new Vector3Int(-1, 1, 1))    // forward blocked
            }
        },
        {
            new Vector3Int(-1, 1, -1),
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, 1, -1), new Vector3Int(-1, 1, -1)),  // north-backward blocked
                (new Vector3Int(-1, 0, -1), new Vector3Int(-1, 1, -1)), // west-backward blocked
                (new Vector3Int(-1, 1, 0), new Vector3Int(-1, 1, -1)),  // NW blocked
                (new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, -1)),   // north blocked
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, 1, -1)),  // west blocked
                (new Vector3Int(0, 0, -1), new Vector3Int(-1, 1, -1))   // backward blocked
            }
        },
        {
            new Vector3Int(-1, -1, 1),
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, -1, 1), new Vector3Int(-1, -1, 1)),  // south-forward blocked
                (new Vector3Int(-1, 0, 1), new Vector3Int(-1, -1, 1)),  // west-forward blocked
                (new Vector3Int(-1, -1, 0), new Vector3Int(-1, -1, 1)), // SW blocked
                (new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, 1)),  // south blocked
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, 1)),  // west blocked
                (new Vector3Int(0, 0, 1), new Vector3Int(-1, -1, 1))    // forward blocked
            }
        },
        {
            new Vector3Int(-1, -1, -1),
            new List<(Vector3Int, Vector3Int)>
            {
                (new Vector3Int(0, -1, -1), new Vector3Int(-1, -1, -1)), // south-backward blocked
                (new Vector3Int(-1, 0, -1), new Vector3Int(-1, -1, -1)), // west-backward blocked
                (new Vector3Int(-1, -1, 0), new Vector3Int(-1, -1, -1)), // SW blocked
                (new Vector3Int(0, -1, 0), new Vector3Int(-1, -1, -1)),  // south blocked
                (new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, -1)),  // west blocked
                (new Vector3Int(0, 0, -1), new Vector3Int(-1, -1, -1))   // backward blocked
            }
        }

};

    private static List<Vector3Int> directions = CreateAllDirectionList();

    public static List<Vector3Int> CreateAllDirectionList()
    {
        var directions = new List<Vector3Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;

                    directions.Add(new Vector3Int(x, y, z));
                }
            }
        }
        return directions;
    }

    public static List<Node> GetNeighbors(Node node, bool includeDiagonals = true)
    {
        var grid = Grid3D.Instance;

        var neighbors = new List<Node>();
        foreach (var dir in directions)
        {
            // Get the position of the neighbor node
            var neighborPos = node.GetNodePositionOnGrid() + dir;

            // Check if the neighbor position is within the grid bounds
            if(!grid.IsInsideGrid(neighborPos.x, neighborPos.y, neighborPos.z))
                continue;

            var neighborNode = grid.GetNodeAt(neighborPos);
            if(neighborNode != null && !neighborNode.bIsBlocked)
            {
                neighbors.Add(neighborNode);
            }
        }

        return neighbors;
    }

    public static List<Node> GetNeighborsInRange(Vector3Int point, int width = 1)
    {
        List<Node> neighbors = new List<Node>();
        var grid = Grid3D.Instance;
        for (int dx = -width; dx <= width; dx++)
        {
            for (int dy = -width; dy <= width; dy++)
            {
                for (int dz = -width; dz <= width; dz++)
                {
                    if (dx == 0 && dy == 0 && dz == 0)
                        continue;

                    int nx = point.x + dx;
                    int ny = point.y + dy;
                    int nz = point.z + dz;

                    var neighbor = grid.GetNodeAt(nx, ny, nz);
                    if (neighbor != null)
                        neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    public static bool HasForcedNeighbor(Node current, Vector3Int direction)
    {
        var grid = Grid3D.Instance;
        Vector3Int currentPos = current.GetNodePositionOnGrid();

        // Check if there is any of the orthogonal directions are blocked
        var orthogonalOffsets = GetOrthogonalOffsets(direction);
        foreach (var offset in orthogonalOffsets)
        {
            // position to check for blockage
            var blockedCheckPos = currentPos + offset;

            // position of the forced neighbor
            var forcedNeighborPos = currentPos + offset + direction;

            // Check...
            // if the forced neighbor or the assumed blocked node is not inside the grid,
            // continue with the next node
            if (!grid.IsInsideGrid(blockedCheckPos) &&
                !grid.IsInsideGrid(forcedNeighborPos))
                continue;

            // Get the blocked node
            var blockedNode = grid.GetNodeAt(blockedCheckPos);
            var forcedNeighborNode = grid.GetNodeAt(forcedNeighborPos);

            // if the blocked node is blocked AND
            // the forced neighbor node is not blocked,
            // its a forced neighbor
            if (blockedNode != null && 
                blockedNode.bIsBlocked &&
                forcedNeighborNode != null &&
               !forcedNeighborNode.bIsBlocked)
            {
                blockedNode.SetColor(Color.black);
                //forcedNeighborNode.SetColor(Color.red);
                return true;
            }
        }

        // Else, its not a forced neighbor
        return false;
    }

    // This method will add all the PERPENDICULAR offsets based on the given direction.
    public static List<Vector3Int> GetOrthogonalOffsets(Vector3Int direction)
    {
        var offsets = new List<Vector3Int>();

        if (direction.x == 0)
        {
            offsets.Add(new Vector3Int(1, 0, 0)); // Right
            offsets.Add(new Vector3Int(-1, 0, 0)); // Left
        }

        if (direction.y == 0)
        {
            offsets.Add(new Vector3Int(0, 1, 0)); // Up
            offsets.Add(new Vector3Int(0, -1, 0)); // Down
        }

        if (direction.z == 0)
        {
            offsets.Add(new Vector3Int(0, 0, 1)); // Forward
            offsets.Add(new Vector3Int(0, 0, -1)); // Backward
        }
        return offsets;
    }

    /// <summary>
    /// This method tries to get all the possible directions from the current node in the given direction.
    /// So basically, if the idea is to get to Nepal and I am looking North-East, 
    /// it will check the next nodes in that direction 
    ///     and get the forced neighbors if there is any
    /// </summary>
    public static List<Node> GetForcedNeighbors(Node current, Vector3Int direction)
    {
        var grid = Grid3D.Instance;

        var currentPos = current.GetNodePositionOnGrid();
        var forcedNeighbors = new List<Node>();

        // Check if the direction has a forced neighbor rule
        if(!HasForcedNeighborRule(direction, out var forcedOffsets))
        {
            return forcedNeighbors;
        }

        // if the rule exists, iterate through the offsets and see if they are blocked or not
        // for each possible blocked neighbor, there is a possible forced neighbor
        // NOTE: At this point, its still a POSSIBILITY that there is a forced neighbor,
        // this is why this loop will check if the possible forced neighbor qualify for the list entry
        foreach (var (blockedOffset, openOffset) in forcedOffsets)
        {
            var blockedPos = currentPos + blockedOffset;
            var openPos = currentPos + openOffset;

            // if the blocked position or the open position is not inside the grid, continue with the next possibility
            if (!grid.IsInsideGrid(blockedPos) || 
               !grid.IsInsideGrid(openPos))
                continue;

            // Get the blocked node and the open node
            var possibleBlockedNode = grid.GetNodeAt(blockedPos);
            var possibleOpenNode = grid.GetNodeAt(openPos);

            if (possibleBlockedNode != null &&
                possibleBlockedNode.bIsBlocked &&
                possibleOpenNode != null &&
               !possibleOpenNode.bIsBlocked)
            {
                // If the blocked node is blocked and the open node is not blocked,
                // add the open node to the list of forced neighbors
                forcedNeighbors.Add(possibleOpenNode);
            }
        }

        return forcedNeighbors;
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
    public static List<Vector3Int> GetNaturalNeighbors(Vector3Int direction)
    {
        var natural = new List<Vector3Int>();

        int[] dx = (direction.x == 0) ? new int[] { 0 } : new int[] { 0, direction.x };
        int[] dy = (direction.y == 0) ? new int[] { 0 } : new int[] { 0, direction.y };
        int[] dz = (direction.z == 0) ? new int[] { 0 } : new int[] { 0, direction.z };

        foreach (var x in dx)
        {
            foreach (var y in dy)
            {
                foreach (var z in dz)
                {
                    if (x == 0 && y == 0 && z == 0) continue; // Skip the zero vector
                    natural.Add(new Vector3Int(x, y, z));
                }
            }
        }
        return natural;
    }

    public static bool HasForcedNeighborRule(Vector3Int direction, out List<(Vector3Int blockOffset, Vector3Int openOffset)> Offsets)
    {
        if(!ForcedNeighborRules.ContainsKey(direction))
        {
            Offsets = null;
            return false;
        }

        Offsets = ForcedNeighborRules[direction];
        return true;
    }
}
