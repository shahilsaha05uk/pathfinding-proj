
using UnityEngine;

public static class BLAHelper
{
    public static (int X, int Y, int Z) CalculateDeltas(LineData lineData)
    {
        int x0 = lineData.x0;
        int x1 = lineData.x1;
        int y0 = lineData.y0;
        int y1 = lineData.y1;
        int z0 = lineData.z0;
        int z1 = lineData.z1;

        // Calculate the absolute differences in x and y coordinates
        return (Mathf.Abs(x1 - x0), Mathf.Abs(y1 - y0), Mathf.Abs(z1 - z0));
    }
    public static int CalculateSlope(int x0, int x1, int y0, int y1, int z0, int z1)
    {
        float deltaX = Mathf.Abs(x1 - x0);
        float deltaY = Mathf.Abs(y1 - y0);
        float deltaZ = Mathf.Abs(z1 - z0);
        return Mathf.RoundToInt(deltaY / deltaX);
    }
    public static int CalculateTotalSteps(LineData lineData)
    {
        var delta = CalculateDeltas(lineData);
        return Mathf.Max(delta.X, Mathf.Max(delta.Y, delta.Z));
    }
}
