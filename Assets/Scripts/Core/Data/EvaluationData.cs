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
    public long MemoryUsedBytes;

    public string Message;
}


