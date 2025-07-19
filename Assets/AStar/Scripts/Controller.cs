using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Controller : MonoBehaviour
{
    public Grid3D mGrid;
    private Node selectedNode;
    
    public bool bIsNodeHit = false;
    public Action<Node> OnNodeSet_Signature;
    
    [Space(10)][Header("Debugger")]
    public GameObject debugSpherePrefab;
    public bool bShowDebug;
    public float debugSize = 0.1f;
    public float yOffset;
    public List<Node> debugPoints;
    
    void Update()
    {
        if (Input.GetKey(KeyCode.S) && Input.GetMouseButtonUp(0))
        {
            var node = GetNodeHit();
            if (node == null)
            {
                Debug.LogWarning("No node hit!");
                return;
            }
            HandleOnShowNeighbours(node);
        }
        if (Input.GetMouseButtonUp(0) && bIsNodeHit)
        {
            if(Execute_OnNodeSet()) UnsubscribeFrom_OnNodeSet();
        }
    }

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
    
    public void CreateGrid() => mGrid.Create();
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
    
    private bool Execute_OnNodeSet()
    {
        var node = GetNodeHit();
        if (node == null)
        {
            Debug.LogWarning("No node hit!");
            return false;
        }
        OnNodeSet_Signature?.Invoke(node);
        return true;
    }
    
    public void OnNavigate()
    {
        var nodes = mGrid.GetStartEndNodes();
        var path = ILS.Navigate(mGrid, nodes.start, nodes.end);
        mGrid.SetPathNode(path);
    }

    private void HideSelectedNode()
    {
        if (selectedNode != null)
        {
            selectedNode.HideNeighbours();
            selectedNode = null;
        }
    }

    // Event handlers for node interactions
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
}
