
using UnityEngine;

public static class GridHelper
{
    public static int CalculateTotalSteps(LineData lineData)
    {
        var delta = GridHelper.CalculateDeltas(lineData);
        return Mathf.Max(delta.X, Mathf.Max(delta.Y, delta.Z));
    }

    public static Vector3Int GetPositionAs3DInt(Node node)
    {
        return new Vector3Int(
            Mathf.RoundToInt(node.transform.position.x),
            Mathf.RoundToInt(node.transform.position.y),
            Mathf.RoundToInt(node.transform.position.z)
        );
    }

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

    public static (int X, int Y, int Z) CalculateDeltas(Node n1, Node n2)
    {
        var n1_pos = n1.GetNodePositionOnGrid();
        var n2_pos = n2.GetNodePositionOnGrid();

        int x0 = n1_pos.x;
        int x1 = n2_pos.x;
        int y0 = n1_pos.y;
        int y1 = n2_pos.y;
        int z0 = n1_pos.z;
        int z1 = n2_pos.z;

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

    public static Vector3Int GetDirection(Node from, Node to)
    {
        Vector3Int dir = to.GetNodePositionOnGrid() - from.GetNodePositionOnGrid();
        return new Vector3Int(
            Mathf.Clamp(dir.x, -1, 1),
            Mathf.Clamp(dir.y, -1, 1),
            Mathf.Clamp(dir.z, -1, 1));
    }


}
