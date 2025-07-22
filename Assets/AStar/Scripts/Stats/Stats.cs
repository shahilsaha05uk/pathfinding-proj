using System;
using UnityEngine;

public static class Stats
{
    public static (T result, StatData stats) RecordStats<T>(Func<T> func)
    {
        float startTime = Time.realtimeSinceStartup;
        var result = func();
        float endTime = Time.realtimeSinceStartup;
        return (result, new StatData
        {
            TimeTaken = (float)(endTime - startTime) * 1000f,
        });
    }
}