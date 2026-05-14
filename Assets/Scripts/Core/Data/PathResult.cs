using System;
using System.Collections.Generic;

[Serializable]
public class PathResult
{
    // Path information
    public int PathLength;
    public float PathCost;
    public int VisitedNodes;
    public int CorridorIterations;
    public List<Node> Path;
    public string Message;
    
    // Performance recording
    public int Success; // 1 = success, 0 = failure
    public long PeakMemoryBytes; // Peak memory used during pathfinding
    public int MaxOpenListSize; // Maximum size of open list during search
}