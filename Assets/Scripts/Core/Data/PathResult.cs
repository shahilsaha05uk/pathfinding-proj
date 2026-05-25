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
    public int MaxCorridorWidth;
    public int CorridorSize;
    public List<Node> Path;
    public string Message;
    
    // Performance recording
    public int Success; // 1 = success, 0 = failure
    public int MaxOpenListSize; // Maximum size of open list during search
    public int MaxClosedListSize; // Maximum size of closed list during search
    public long PeakedMemoryBytes; // Peak memory usage recorded by the algorithm during search
}