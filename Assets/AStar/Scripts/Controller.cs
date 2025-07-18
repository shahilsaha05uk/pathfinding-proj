using System;
using System.Collections.Generic;
using UnityEngine;

public enum EDimension
{
    Grid2D,
    Grid3D
}

public class Controller : MonoBehaviour
{
    public bool bListenToMouseInput = false;
    public GameObject Grid;
    private BaseGrid mGrid;
    private Action<Node> OnNodeClickedSignature;

    private Node selectedNode;
    public float zPos = 1.0f;
    
    private void Start()
    {
        UpdateDimension(EDimension.Grid3D); // Default to 2D grid
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && Input.GetMouseButtonUp(0))
        {
            // Check if a tile is clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.TryGetComponent(out Node node))
                {
                    HandleOnShowNeighbours(node);
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && bListenToMouseInput)
        {
            HideSelectedNode();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.TryGetComponent(out Node node))
                {
                    OnNodeClickedSignature?.Invoke(node);
                }
            }
        }
    }

    public void EnableMouseInput() => bListenToMouseInput = true;
    public void DisableMouseInput() => bListenToMouseInput = false;

    public void OnDimensionChange(EDimension dimension)
    {
        UpdateDimension(dimension);
    }
    public void OnCreateGrid()
    {
        mGrid.Create();
    }
    public void OnSetStart()
    {
        OnNodeClickedSignature += HandleOnStartNodeSet;
        EnableMouseInput();
    }
    public void OnSetEnd()
    {
        OnNodeClickedSignature += HandleOnEndNodeSet;
        EnableMouseInput();
    }
    public void OnClearGrid()
    {
        mGrid.Clear();
    }
    public void OnNavigate()
    {
        var nodes = mGrid.GetStartEndNodes();
        var path = AStar.Navigate(nodes.start, nodes.end);
        mGrid.SetPathNode(path);
        
        
        //BLA.DrawLine(path, zPos, 5.0f);
    }

    private void UpdateDimension(EDimension dimension)
    {
        if (dimension is EDimension.Grid2D) mGrid = Grid.GetComponent<Grid2D>();
        else if (dimension is EDimension.Grid3D) mGrid = Grid.GetComponent<Grid3D>();
    }
    private void HandleOnStartNodeSet(Node node)
    {
        mGrid.SetStartNode(node);
        OnNodeClickedSignature -= HandleOnStartNodeSet;
        DisableMouseInput();
        Debug.Log($"Start node set to: {node.name}");
    }
    private void HandleOnEndNodeSet(Node node)
    {
        mGrid.SetEndNode(node);
        OnNodeClickedSignature -= HandleOnEndNodeSet;
        DisableMouseInput();
        Debug.Log($"End node set to: {node.name}");
    }

    private void HandleOnShowNeighbours(Node node)
    {
        HideSelectedNode();
        selectedNode = node;
        selectedNode.ShowNeighbours();
    }
    
    private void HideSelectedNode()
    {
        if (selectedNode != null)
        {
            selectedNode.HideNeighbours();
            selectedNode = null;
        }
    }
}
