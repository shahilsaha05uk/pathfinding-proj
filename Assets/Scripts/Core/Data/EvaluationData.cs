using System;

[Serializable]
public class EvaluationData
{
    public int PathLength;
    public float PathCost;
    public int VisitedNodes;
    public int CorridorIterations;

    // Stats
    public float TimeTaken;
    public float MeasuredTimeMs;
    public float MemoryUsedBytes;

    public string Message;
}


