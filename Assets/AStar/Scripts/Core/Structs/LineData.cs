using UnityEngine;

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
