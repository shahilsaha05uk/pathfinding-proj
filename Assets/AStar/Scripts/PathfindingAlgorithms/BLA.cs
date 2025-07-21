    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using UnityEngine;

    // the formula for a line is: y = mx + c
    
    /*
     * p = 2dz - dx
     * p = 2 * 2 - 12
     * p = 4 - 12
     * p = -8
     * 
     */
    
    public static class BLA
    {
    public static List<Vector3Int> GenerateLine(Node start, Node end)
    {
        var startPos = NodeMathHelper.GetPositionAs3DInt(start);
        var endPos = NodeMathHelper.GetPositionAs3DInt(end);

        int x0 = startPos.x, y0 = startPos.y, z0 = startPos.z;
        int x1 = endPos.x, y1 = endPos.y, z1 = endPos.z;

        int deltaX = Math.Abs(x1 - x0);
        int deltaY = Math.Abs(y1 - y0);
        int deltaZ = Math.Abs(z1 - z0);

        int stepX = (x0 < x1) ? 1 : -1;
        int stepY = (y0 < y1) ? 1 : -1;
        int stepZ = (z0 < z1) ? 1 : -1;

        // Precompute doubled values for efficiency
        int deltaX2 = deltaX * 2;
        int deltaY2 = deltaY * 2;
        int deltaZ2 = deltaZ * 2;

        // Preallocate list to avoid resizing
        int maxPoints = Math.Max(Math.Max(deltaX, deltaY), deltaZ) + 1;
        var points = new List<Vector3Int>(maxPoints);
        points.Add(new Vector3Int(x0, y0, z0));

        // Driving axis: X
        if (deltaX >= deltaY && deltaX >= deltaZ)
        {
            int p1 = deltaY2 - deltaX;
            int p2 = deltaZ2 - deltaX;
            while (x0 != x1)
            {
                x0 += stepX;
                if (p1 >= 0) { y0 += stepY; p1 -= deltaX2; }
                if (p2 >= 0) { z0 += stepZ; p2 -= deltaX2; }
                p1 += deltaY2;
                p2 += deltaZ2;
                points.Add(new Vector3Int(x0, y0, z0));
            }
        }
        // Driving axis: Y
        else if (deltaY >= deltaX && deltaY >= deltaZ)
        {
            int p1 = deltaX2 - deltaY;
            int p2 = deltaZ2 - deltaY;
            while (y0 != y1)
            {
                y0 += stepY;
                if (p1 >= 0) { x0 += stepX; p1 -= deltaY2; }
                if (p2 >= 0) { z0 += stepZ; p2 -= deltaY2; }
                p1 += deltaX2;
                p2 += deltaZ2;
                points.Add(new Vector3Int(x0, y0, z0));
            }
        }
        // Driving axis: Z
        else
        {
            int p1 = deltaY2 - deltaZ;
            int p2 = deltaX2 - deltaZ;
            while (z0 != z1)
            {
                z0 += stepZ;
                if (p1 >= 0) { y0 += stepY; p1 -= deltaZ2; }
                if (p2 >= 0) { x0 += stepX; p2 -= deltaZ2; }
                p1 += deltaY2;
                p2 += deltaX2;
                points.Add(new Vector3Int(x0, y0, z0));
            }
        }

        return points;
    }
    public static void DrawLine(List<Vector3Int> points,
            float duration = 500f, Color? color = null)
        {
            if (points == null || points.Count < 2)
                return;

            Color drawColor = color ?? Color.red;

            for (int i = 0; i < points.Count - 1; i++)
            {
                Debug.DrawLine(points[i], points[i + 1], drawColor, duration);
            }
        }
    }