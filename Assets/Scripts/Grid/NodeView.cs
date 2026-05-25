using UnityEngine;

public class NodeView : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;

    private Node boundNode;
    private MaterialPropertyBlock propertyBlock;

    public Node Node => boundNode;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        propertyBlock = new MaterialPropertyBlock();
    }

    public void Bind(Node node)
    {
        boundNode = node;
        boundNode?.AttachView(this);
        SetColor(node != null ? node.GetCurrentColor() : Color.white);
    }

    public void SetColor(Color color)
    {
        if (targetRenderer == null)
            return;

        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", color);
        propertyBlock.SetColor("_BaseColor", color);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }

    private void OnDestroy()
    {
        if (boundNode != null)
        {
            var node = boundNode;
            boundNode = null;
            node.DetachView(this);
        }
    }
}
