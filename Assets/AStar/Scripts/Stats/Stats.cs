using System;
using System.Diagnostics;

[Serializable]
public struct StatData
{
    public float TimeTaken;
    public float SpaceTaken;
}

public static class Stats
{
    public static StatData RecordStats(Action action)
    {
        long beforeMemory = GC.GetTotalMemory(false);
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        action();
        stopwatch.Stop();
        
        long afterMemory = GC.GetTotalMemory(false);

        return new StatData
        {
            TimeTaken = (float)stopwatch.Elapsed.TotalMilliseconds,
            SpaceTaken = (float)(afterMemory - beforeMemory) / (1024 * 1024) // Convert bytes to MB
        };
    }
    
    public static (T result, StatData stats) RecordStats<T>(Func<T> func)
    {
        long beforeMemory = GC.GetTotalMemory(false);
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = func();
        stopwatch.Stop();
        
        long afterMemory = GC.GetTotalMemory(false);

        return (result, new StatData
        {
            TimeTaken = (float)stopwatch.Elapsed.TotalMilliseconds,
            SpaceTaken = (float)(afterMemory - beforeMemory) / (1024 * 1024) // Convert bytes to MB
        });
    }
}