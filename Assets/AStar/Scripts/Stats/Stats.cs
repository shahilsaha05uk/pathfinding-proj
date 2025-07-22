using System;
using System.Diagnostics;
using UnityEngine;

public static class Stats
{
    //public static (T result, StatData stats) RecordStats<T>(Func<T> func)
    //{
    //    var process = System.Diagnostics.Process.GetCurrentProcess();

    //    long startMemory = process.PrivateMemorySize64;
    //    float startTime = Time.realtimeSinceStartup;

    //    var result = func();

    //    float endTime = Time.realtimeSinceStartup;
    //    long endMemory = process.PrivateMemorySize64;

    //    return (result, new StatData
    //    {
    //        TimeTaken = (float)(endTime - startTime) * 1000f,
    //        MemoryUsed = (float)(endMemory - startMemory) / 1024, // Convert bytes to KB
    //        PeakMemoryUsed = (float)(process.PeakWorkingSet64) / 1024f / 1024f // MB
    //    });
    //}

    public static (T result, StatData stats) RecordStats<T>(Func<T> func)
    {
        // Force garbage collection to get clean measurements
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long startMemory = GC.GetTotalMemory(true);
        float startTime = Time.realtimeSinceStartup;
        var result = func();

        float endTime = Time.realtimeSinceStartup;
        long endMemory = GC.GetTotalMemory(true);

        // Force another collection to measure cleanup
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        long finalMemory = GC.GetTotalMemory(true);

        return (result, new StatData
        {
            TimeTaken = (float)(endTime - startTime) * 1000f,
            MemoryUsed = (float)(endMemory - startMemory) / 1024f, // Convert to KB
            PeakMemoryUsed = (float)(finalMemory - startMemory) / 1024f
        });
    }
}

//public static StatData RecordStats(Action action)
//{
//    long beforeMemory = GC.GetTotalMemory(false);

//    var stopwatch = new Stopwatch();
//    stopwatch.Start();
//    action();
//    stopwatch.Stop();

//    long afterMemory = GC.GetTotalMemory(false);

//    return new StatData
//    {
//        TimeTaken = (float)stopwatch.Elapsed.TotalMilliseconds,
//        SpaceTaken = (float)(afterMemory - beforeMemory) / (1024 * 1024) // Convert bytes to MB
//    };
//}
//public static (T result, StatData stats) RecordStats<T>(Func<T> func)
//{
//    long beforeMemory = GC.GetTotalMemory(false);

//    float startTime = Time.realtimeSinceStartup;
//    var result = func();
//    float endTime = Time.realtimeSinceStartup;

//    long afterMemory = GC.GetTotalMemory(false);
//    var allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
//    var totalMemory = GC.GetTotalMemory(false);

//    UnityEngine.Debug.Log($"Allocated Bytes: {allocatedBytes}, Total Memory: {totalMemory}");

//    return (result, new StatData
//    {
//        TimeTaken = (float)(endTime - startTime) * 1000f,
//        SpaceTaken = (float)(afterMemory - beforeMemory) / 1024 // Convert bytes to KB
//    });
//}
