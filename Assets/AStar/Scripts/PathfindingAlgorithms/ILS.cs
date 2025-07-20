using System.Collections.Generic;
using UnityEngine;


public enum CorridorShape
{
    Cube,
    Diamond,
    Sphere
}
public static class ILS
{
    public static List<Node> Navigate(Grid3D grid, Node start, Node end, int maxCorridorWidth)
    {
        var temp = Time.realtimeSinceStartup;

        int currentWidth = 1;
        int maxWidth = maxCorridorWidth;

        while (currentWidth <= maxWidth)
        {
            var linePoints = GenerateLine(start, end);
            var corridor = DefineCorridor(linePoints, grid, start, end, currentWidth);
            var path = FindPath(start, end, corridor);
            
            if(path is { Count: > 0 })
                return path;
            
            currentWidth++;
        }
        Debug.LogWarning("No path found!");
        return null;
    }
    
    // Step 1: Get the Line from BLA
    private static List<Vector3Int> GenerateLine(Node start, Node end)
    {
        return BLA.GenerateLine(start, end);
    }
    
    // Step 2: Define the corridor
    private static HashSet<Node> DefineCorridor(
        List<Vector3Int> linePoints, 
        Grid3D grid, 
        Node start, Node end,
        CorridorShape shape = CorridorShape.Diamond,
        int width = 1)
    {
        var corridorNodes = new HashSet<Node>();

        foreach (var point in linePoints)
        {
            var neighbors = grid.GetNeighbors(point, width);
            corridorNodes.UnionWith(neighbors);
        }
        
        corridorNodes.Add(start);
        corridorNodes.Add(end);
        

        return corridorNodes;
    }
    
    // Step 3: Pass it to the pathfinding algorithm
    private static List<Node> FindPath(Node start, Node end, HashSet<Node> corridor)
    {
        return AStar.Navigate(start, end, corridor);
    }
}
