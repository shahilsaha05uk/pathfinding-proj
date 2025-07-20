using System;
using System.Collections.Generic;
using UnityEngine;


public partial class Controller
{
    private Node selectedNode;
    public bool bIsNodeHit = false;
    
    public Action<Node> OnNodeSet_Signature; 
    
    [Header("Debugging")]
    List<Node> navigationPath = new List<Node>();
    
    public void EnableNodeHit() => bIsNodeHit = true;
    public void DisableNodeHit() => bIsNodeHit = false;
    public Node GetNodeHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.TryGetComponent(out Node node))
            {
                return node;
            }
        }
        return null;
    }
    public void CreateGrid(GridConfig config) => mGrid.Create(config);
    public void ClearGrid() => mGrid.Clear();
    public void SubscribeTo_StartNodeSet()
    {
        EnableNodeHit();
        OnNodeSet_Signature += HandleOnStartNodeSet;
    }
    public void SubscribeTo_EndNodeSet()
    {
        EnableNodeHit();
        OnNodeSet_Signature += HandleOnEndNodeSet;
    }
    public void UnsubscribeFrom_OnNodeSet()
    {
        DisableNodeHit();
        OnNodeSet_Signature = null;
    }

    private void HideSelectedNode()
    {
        if (selectedNode != null)
        {
            selectedNode.HideNeighbours();
            selectedNode = null;
        }
    }

    private void HandleOnStartNodeSet(Node node)
    {
        mGrid.SetStartNode(node);
        Debug.Log($"Start node set to: {node.name}");
    }
    private void HandleOnEndNodeSet(Node node)
    {
        mGrid.SetEndNode(node);
        Debug.Log($"End node set to: {node.name}");
    }
    private void HandleOnShowNeighbours(Node node)
    {
        if (selectedNode != null && selectedNode == node)
        {
            HideSelectedNode();
            return;
        }

        HideSelectedNode();
        selectedNode = node;
        selectedNode.ShowNeighbours();
    }
    private bool Execute_OnNodeSet()
    {
        var node = GetNodeHit();
        if (node == null) return false;

        OnNodeSet_Signature?.Invoke(node);
        return true;
    }
}