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
    
    public struct LineData
    {
        public readonly int x0;
        public readonly int x1;
        public readonly int y0;
        public readonly int y1;
        public readonly int z0;
        public readonly int z1;
        public (int X, int Y, int Z) deltas;
        
        public LineData(Vector3Int v1, Vector3Int v2) : this()
        {
            x0 = v1.x;
            x1 = v2.x;
            
            y0 = v1.y;
            y1 = v2.y;
            
            z0 = v1.z;
            z1 = v2.z;
            deltas = BLAHelper.CalculateDeltas(this);
        }
    }
    public static class BLA
    { 
        public static List<Vector3Int> GenerateLine(Node start, Node end, float offset = 0.2f)
        {
            var startPos = NodeMathHelper.GetPositionAs3DInt(start);
            var endPos = NodeMathHelper.GetPositionAs3DInt(end);
            var lineData = new LineData(startPos, endPos);

            List<Vector3Int> points = new List<Vector3Int>();

            int x0 = lineData.x0, y0 = lineData.y0, z0 = lineData.z0;
            int x1 = lineData.x1,   y1 = lineData.y1,   z1 = lineData.z1;
            
            // the slope of the line
            (int deltaX, int deltaY, int deltaZ) = lineData.deltas;
            
            // steps
            int stepX = (lineData.x0 < lineData.x1) ? 1 : -1;
            int stepY = (lineData.y0 < lineData.y1) ? 1 : -1;
            int stepZ = (lineData.z0 < lineData.z1) ? 1 : -1;
            
            // Add the goal point to the list
            int x = x0, y = y0, z = z0;
            points.Add(new Vector3Int(x, y, z));
            
            // Driving axis is X-axis
            if (deltaX >= deltaY && deltaX >= deltaZ)
            {
                int p1 = 2 * deltaY - deltaX;
                int p2 = 2 * deltaZ - deltaX;
                while (x != x1)
                {
                    x += stepX;
                    if (p1 >= 0)
                    {
                        y += stepY;
                        p1 -= 2 * deltaX;
                    }
                    if (p2 >= 0)
                    {
                        z += stepZ;
                        p2 -= 2 * deltaX;
                    }
                    p1 += 2 * deltaY;
                    p2 += 2 * deltaZ;
                    points.Add(new Vector3Int(x, y, z));
                }
            }
            // Driving axis is Y-axis
            else if (deltaY >= deltaX && deltaY >= deltaZ)
            {
                int p1 = 2 * deltaX - deltaY;
                int p2 = 2 * deltaZ - deltaY;
                while (y != y1)
                {
                    y += stepY;
                    if (p1 >= 0)
                    {
                        x += stepX;
                        p1 -= 2 * deltaY;
                    }
                    if (p2 >= 0)
                    {
                        z += stepZ;
                        p2 -= 2 * deltaY;
                    }
                    p1 += 2 * deltaX;
                    p2 += 2 * deltaZ;
                    points.Add(new Vector3Int(x, y, z));
                }
            }
            // Driving axis is Z-axis
            else
            {
                int p1 = 2 * deltaY - deltaZ;
                int p2 = 2 * deltaX - deltaZ;
                while (z != z1)
                {
                    z += stepZ;
                    if (p1 >= 0)
                    {
                        y += stepY;
                        p1 -= 2 * deltaZ;
                    }

                    if (p2 >= 0)
                    {
                        x += stepX;
                        p2 -= 2 * deltaZ;
                    }

                    p1 += 2 * deltaY;
                    p2 += 2 * deltaX;
                    points.Add(new Vector3Int(x, y, z));
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