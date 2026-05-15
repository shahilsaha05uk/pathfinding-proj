using System;

[Serializable]
public class EvaluationData
{
    public int PathLength;
    public float PathCost;
    public int VisitedNodes;
    public int CorridorIterations;

    // Search result quality
    public int Success; // 1 = success, 0 = failure
    public long PeakMemoryBytes;
    public int MaxOpenListSize;
    public int MaxClosedListSize;

    // Stats
    public float TimeTaken;
    public float MeasuredTimeMs;
    public float MemoryUsedBytes;

    public string Message;
}


