using System;
using System.Collections.Generic;

[Serializable]
public class PathResult
{
    public int PathLength;
    public float PathCost;
    public int VisitedNodes;
    public int CorridorIterations;
    public List<Node> Path;
    public string Message;
    public bool Success;
}