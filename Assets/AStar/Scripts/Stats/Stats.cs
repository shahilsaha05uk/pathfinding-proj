using System;
using System.Diagnostics;

public static class Stats
{
    public static float TimedStats(Action action)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        action();
        stopwatch.Stop();
        return (float) stopwatch.Elapsed.TotalMilliseconds;
    }
    
    public static (T result, float time) TimedStats<T>(Func<T> func)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = func();
        stopwatch.Stop();
        return (result, (float) stopwatch.Elapsed.TotalMilliseconds);
    }
}