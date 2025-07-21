using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PathResult
{
    public double TimeTaken;
    public long SpaceTaken;
    public int PathLength;
    public float PathCost;
    public int CorridorIterations;
    public List<Node> Path;
}

