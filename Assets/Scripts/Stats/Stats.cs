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

        // Calls the function (algorithm will track its own peak memory)
        var result = func();

        sw.Stop();

        // Sample memory after function completes
        long memAfter = System.GC.GetTotalMemory(false);
        long memUsed = memAfter - memBefore;

        // Extract peaked memory from the result if it's a PathResult
        // PeakedMemoryBytes is stored as absolute value by the algorithm
        long peakedMemoryAbsolute = memBefore;
        if (result is PathResult pathResult && pathResult.PeakedMemoryBytes > 0)
        {
            peakedMemoryAbsolute = pathResult.PeakedMemoryBytes;
        }
        else
        {
            // Fallback: use memAfter if no peaked value was recorded
            peakedMemoryAbsolute = memAfter;
        }

        // Calculate the relative peaked memory (how much memory was used during execution)
        long peakedMemoryRelative = peakedMemoryAbsolute - memBefore;

        // Records the memory usage after the function call
        float endTime = Time.realtimeSinceStartup;
        return (result, new StatData
        {
            TimeTaken = (float)(endTime - startTime) * 1000f,
            MeasuredTimeMs = (float)sw.Elapsed.TotalMilliseconds,
            MemoryUsedBytes = memUsed,                                  // Final memory still in use
            PeekedBytes = (float)peakedMemoryRelative                   // Peak memory relative to baseline
        });
    }
}