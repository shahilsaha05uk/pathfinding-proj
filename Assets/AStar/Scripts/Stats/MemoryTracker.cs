using System.Collections;
using Unity.Profiling;
using UnityEngine;
using System.Diagnostics;

public class MemoryTracker : MonoBehaviour
{
    private ProfilerRecorder systemMem;
    
    void Start()
    {
        systemMem = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory", 1);
        // StartCoroutine(LogMemoryUsage());
    }

    private void OnDisable()
    {
        if (systemMem.Valid)
            systemMem.Dispose();
    }

    private IEnumerator LogMemoryUsage()
    {
        var delay = new WaitForSeconds(1f);
        while (true)
        {
            if (systemMem.Valid)
            {
                var systemUsed = GetMemoryUsage();
                UnityEngine.Debug.Log($"System Used Memory: {systemUsed} MB");
            }

            yield return delay;
        }
    }

    private float GetMemoryUsage()
    {
        var process = GetProcess();
        float memoryUsage = process / (1024 * 1024); // Convert to MB
        return memoryUsage;
    }

    private long GetProcess()
    {
        using (var process = Process.GetCurrentProcess())
            return process.WorkingSet64;
    }
}
