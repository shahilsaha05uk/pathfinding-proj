using System;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public bool bListenToMouseInput = false;
    public Grid3D Grid;
    private Action<Node> OnNodeClickedSignature;

    private void Start()
    {
        DisableMouseInput();
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && bListenToMouseInput)
        {
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

    public void OnCreateGrid()
    {
        Grid.CreateGrid();
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
        Grid.DestroyGrid();
    }
    public void OnNavigate()
    {
        var nodes = Grid.GetStartEndNodes();
        var path = AStar.Navigate(nodes.start, nodes.end);
        Grid.SetPathNode(path);
    }
    private void HandleOnStartNodeSet(Node node)
    {
        Grid.SetStartNode(node);
        OnNodeClickedSignature -= HandleOnStartNodeSet;
        DisableMouseInput();
        Debug.Log($"Start node set to: {node.name}");
    }
    private void HandleOnEndNodeSet(Node node)
    {
        Grid.SetEndNode(node);
        OnNodeClickedSignature -= HandleOnStartNodeSet;
        DisableMouseInput();
        Debug.Log($"End node set to: {node.name}");
    }
}
