
using System;

[Serializable]
public class EvaluationResult
{
    public float AStarSpace;
    public float AStarTime;

    public float GBFSSpace;
    public float GBFSTime;

    public float ILSWithAStarSpace;
    public float ILSWithAStarTime;
    public float ILSIterations;
}