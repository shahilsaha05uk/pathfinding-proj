using System;
using System.Diagnostics;
using UnityEngine;

public static class Stats
{
    public static (T result, StatData stats) RecordStats<T>(Func<T> func)
    {
        // Records the time
        float startTime = Time.realtimeSinceStartup;

        // Records the memory usage before the function call
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();

        long memBefore = System.GC.GetTotalMemory(false);
        var sw = Stopwatch.StartNew();

        // Calls the function
        var result = func();

        sw.Stop();
        long memAfter = System.GC.GetTotalMemory(false);
        long memUsed = memAfter - memBefore;

        // Records the memory usage after the function call
        float endTime = Time.realtimeSinceStartup;
        return (result, new StatData
        {
            TimeTaken = (float)(endTime - startTime) * 1000f,
            MemoryUsedBytes = memUsed,
            MeasuredTimeMs = (float)sw.Elapsed.TotalMilliseconds
        });
    }
}