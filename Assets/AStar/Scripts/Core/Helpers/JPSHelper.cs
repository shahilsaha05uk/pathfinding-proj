
using System.Collections.Generic;
using UnityEngine;

public static class JPSHelper
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
    
    public static bool TryGetForcedNeighborRule(Vector3Int direction, out List<(Vector3Int BlockedOffset, Vector3Int OpenOffset)> rule)
    {
        if (!HasForcedNeighborRule(direction))
        {
            rule = null;
            return false;
        }

        rule = ForcedNeighborRules[direction];
        return true;
    }
    
    public static bool HasForcedNeighborRule(Vector3Int direction)
    {
        return ForcedNeighborRules.ContainsKey(direction);
    }
}
