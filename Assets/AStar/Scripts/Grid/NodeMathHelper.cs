
using UnityEngine;

public static class NodeMathHelper
{
    public static Vector3Int GetPositionAs3DInt(Node node)
    {
        return new Vector3Int(
            Mathf.RoundToInt(node.transform.position.x),
            Mathf.RoundToInt(node.transform.position.y),
            Mathf.RoundToInt(node.transform.position.z)
        );
    }
}
