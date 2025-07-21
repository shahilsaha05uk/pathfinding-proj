using System;
using UnityEngine;

public static class Stats
{
    public static StatData RecordStats(Action action)
    {
        GCClean();

        long beforeMemory = GC.GetTotalMemory(false);
        float startTime = Time.realtimeSinceStartup; // Start timing

        action(); // Execute the action

        GCClean();

        float endTime = Time.realtimeSinceStartup; // End timing
        long afterMemory = GC.GetTotalMemory(true);

        var memoryUsage = afterMemory - beforeMemory;
        Debug.Log("Memory Usage: " + memoryUsage + " bytes");

        return new StatData
        {
            TimeTaken = (endTime - startTime) * 1000f, // Convert seconds → milliseconds
            SpaceTaken = memoryUsage
        };
    }

    public static (T result, StatData stats) RecordStats<T>(Func<T> func)
    {
        GCClean();
        long beforeMemory = GC.GetTotalMemory(true);
        float startTime = Time.realtimeSinceStartup; // Start timing

        T result = func(); // Execute the function

        GCClean();

        float endTime = Time.realtimeSinceStartup; // End timing
        long afterMemory = GC.GetTotalMemory(true);

        var memoryUsage = afterMemory - beforeMemory;
        Debug.Log("Memory Usage: " + memoryUsage + " bytes");

        return (result, new StatData
        {
            TimeTaken = (endTime - startTime) * 1000f, // Convert seconds → milliseconds
            SpaceTaken = memoryUsage
        });
    }

    private static void GCClean()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}