    using System.Collections.Generic;
    using UnityEngine;

    public static class BLA
    {
        public static List<Vector2Int> GenerateLine(Node start, Node end)
        {
            var startPos = NodeMathHelper.GetPositionAs3DInt(start);
            var endPos = NodeMathHelper.GetPositionAs3DInt(end);
            return BresenhamLine(startPos, endPos);
            
            //Vector2Int a = new Vector2Int(Mathf.RoundToInt(start.transform.position.x), Mathf.RoundToInt(start.transform.position.z));
            //Vector2Int b = new Vector2Int(Mathf.RoundToInt(end.transform.position.x), Mathf.RoundToInt(end.transform.position.z));
            //return BresenhamLine(a, b);

        }

        private static List<Vector2Int> BresenhamLine(Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> points = new List<Vector2Int>();

            int x0 = start.x;
            int y0 = start.y;
            int x1 = end.x;
            int y1 = end.y;

            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = (x0 < x1) ? 1 : -1;
            int sy = (y0 < y1) ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                points.Add(new Vector2Int(x0, y0));

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;

                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return points;
        }
        
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
        public static void DrawLine(List<Vector2Int> points, float zPos = 1.0f, float thickness = 2.0f, float duration = 500f, Color? color = null)
        {
            if (points == null || points.Count < 2)
                return;

            Color drawColor = color ?? Color.red;

            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 start = new Vector3(points[i].x, zPos, points[i].y);
                Vector3 end = new Vector3(points[i + 1].x, zPos, points[i + 1].y);
                Debug.DrawLine(start, end, drawColor, duration);
            }
        }
    }
