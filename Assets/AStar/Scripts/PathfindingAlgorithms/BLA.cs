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
        public int m;
        public int totalSteps;
        public (int X, int Y, int Z) deltas;
        
        public LineData(Vector3Int v1, Vector3Int v2) : this()
        {
            x0 = v1.x;
            x1 = v2.x;
            
            y0 = v1.y;
            y1 = v2.y;
            
            z0 = v1.z;
            z1 = v2.z;
            
            m = BLAHelper.CalculateSlope(x0, x1, y0, y1, z0, z1);
            totalSteps = BLAHelper.CalculateTotalSteps(this);
            deltas = BLAHelper.CalculateDeltas(this);
        }
    }
    public static class BLA
    {
        public static List<Vector3Int> GenerateLine(Node start, Node end)
        {
            var startPos = NodeMathHelper.GetPositionAs3DInt(start);
            var endPos = NodeMathHelper.GetPositionAs3DInt(end);
            var points = BresenhamLine(startPos, endPos);
            DrawLine(points);
            return points;
        }

        public static List<Vector3Int> GenerateDebugLine(Node start, Node end, float offset = 0.2f)
        {
            var startPos = NodeMathHelper.GetPositionAs3DInt(start);
            var endPos = NodeMathHelper.GetPositionAs3DInt(end);

            List<Vector3Int> points = new List<Vector3Int>();

            var lineData = new LineData(startPos, endPos);
            
            // the slope of the line
            (int deltaX, int deltaY, int deltaZ) = lineData.deltas;
            
            // steps
            int stepX = (lineData.x0 < lineData.x1) ? 1 : -1;   // x0 = 2, x1 = 6 (moving on the positive x direction)
            int stepY = (lineData.y0 < lineData.y1) ? 1 : -1;   // y0 = 2, y1 = 6 (moving on the positive y direction)
            int stepZ = (lineData.z0 < lineData.z1) ? 1 : -1;   // y0 = 2, y1 = 6 (moving on the positive z direction)
            
            int totalSteps = lineData.totalSteps;
            
            int currentX = lineData.x0;
            int currentY = lineData.y0;
            int currentZ = lineData.z0;
            
            float errorX = deltaX / (float)totalSteps;
            float errorY = deltaY / (float)totalSteps;
            float errorZ = deltaZ / (float)totalSteps;
            
            float accumX = 0;
            float accumY = 0;
            float accumZ = 0;
            
            // for every step in the x direction
            for (int i = 0; i < totalSteps; i++)
            {
                accumX += errorX;
                accumY += errorY;
                accumZ += errorZ;
                
                points.Add(new Vector3Int(currentX, currentY, currentZ));

                if (accumX >= 1f)
                {
                    currentX += stepX;
                    accumX -= 1f;
                }

                if (accumY >= 1f)
                {
                    currentY += stepY;
                    accumY -= 1f;
                }

                if (accumZ >= 1f)
                {
                    currentZ += stepZ;
                    accumZ -= 1f;
                }
                
                // Break if we reach the end point
                if(currentX == lineData.x1 && currentZ == lineData.z1)
                    break;
            }
            
            return points;
        }


        private static List<Vector3Int> BresenhamLine(Vector3Int start, Vector3Int end)
        {
            List<Vector3Int> points = new List<Vector3Int>();

            // var slope = GetLineData(start, end);
            // var decisionParameter = CalculateDecisionParameter(slope.deltaX, slope.deltaY);
            //
            // var deltaX = slope.deltaX;
            //
            // // For every step in the x direction
            // for (int i = 0; i < deltaX; i++)
            // {
            //     Gizmos.color = Color.red;
            //     Gizmos.DrawSphere(new Vector3(start.x + i, start.y + i, start.z + i), 10f);
            // }
            
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

// public static List<Vector3Int> GenerateLine(Node start, Node end)
// {
//     Vector2Int a = new Vector2Int(Mathf.RoundToInt(start.transform.position.x), Mathf.RoundToInt(start.transform.position.z));
//     Vector2Int b = new Vector2Int(Mathf.RoundToInt(end.transform.position.x), Mathf.RoundToInt(end.transform.position.z));
//     return BresenhamLine(a, b);
// }
    
    // for (int i = 0; i < totalSteps; i++)
    // {
    //     points.Add(new Vector3(currentX, offset, currentZ));
    //
    //     int e2 = 2 * p;  // do NOT change the real error value
    //
    //     if (e2 > -deltaZ)
    //     {
    //         p -= deltaZ;
    //         currentX += stepX;
    //     }
    //
    //     if (e2 < deltaX)
    //     {
    //         p += deltaX;
    //         currentZ += stepZ;
    //     }
    //
    //     if (currentX == lineData.x1 && currentZ == lineData.z1)
    //         break;
    // }
// private static List<Vector2Int> BresenhamLine(Vector2Int start, Vector2Int end)
// {
//     List<Vector2Int> points = new List<Vector2Int>();
//
//     int x0 = start.x;
//     int y0 = start.y;
//     int x1 = end.x;
//     int y1 = end.y;
//
//     int dx = Mathf.Abs(x1 - x0);
//     int dy = Mathf.Abs(y1 - y0);
//     int sx = (x0 < x1) ? 1 : -1;
//     int sy = (y0 < y1) ? 1 : -1;
//     int err = dx - dy;
//
//     while (true)
//     {
//         points.Add(new Vector2Int(x0, y0));
//
//         if (x0 == x1 && y0 == y1) break;
//
//         int e2 = 2 * err;
//
//         if (e2 > -dy)
//         {
//             err -= dy;
//             x0 += sx;
//         }
//
//         if (e2 < dx)
//         {
//             err += dx;
//             y0 += sy;
//         }
//     }
//
//     return points;
// }
//
// public static List<Vector2Int> GenerateLine(Node start, Node end)
// {
//     // Get the position of the start and end nodes
//     var startPos = start.transform.position;
//     var endPos = end.transform.position;
//
//     // Get the x and y coordinates of the start and end nodes
//     int x0 = Mathf.RoundToInt(startPos.x);
//     int x1 = Mathf.RoundToInt(endPos.x);
//
//     int y0 = Mathf.RoundToInt(startPos.y);
//     int y1 = Mathf.RoundToInt(endPos.y);
//     
//     if(Mathf.Abs(x1 - x0) > Mathf.Abs(y1 - y0))
//     {
//         // Horizontal line
//         return DrawLineH(x0, x1, y0, y1);
//     }
//     else
//     {
//         // Vertical line
//         return DrawLineV(x0, x1, y0, y1);
//     }
// }
// public static void DrawLine(List<Vector2Int> points, float zPos = 1.0f, float thickness = 2.0f,
//     float duration = 500f, Color? color = null)
// {
//     if (points == null || points.Count < 2)
//         return;
//
//     Color drawColor = color ?? Color.red;
//
//     for (int i = 0; i < points.Count - 1; i++)
//     {
//         Vector3 start = new Vector3(points[i].x, zPos, points[i].y);
//         Vector3 end = new Vector3(points[i + 1].x, zPos, points[i + 1].y);
//         Debug.DrawLine(start, end, drawColor, duration);
//     }
// }
