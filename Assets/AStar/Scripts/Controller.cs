using System;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Grid3D mGrid;
    private Node selectedNode;
    
    public bool bIsNodeHit = false;
    public Action<Node> OnNodeSet_Signature;
    
    [Header("Camera Controls")]
    public float cameraSpeed = 5f;
    public float fastSpeedMultiplier = 3f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    private float yaw = 0f;
    private float pitch = 0f;
    private bool isRightMouseHeld = false;
    
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

    void LateUpdate()
    {
        HandleCameraRotation();
        HandleCameraMovement();
    }
    
    private void HandleCameraRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseHeld = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isRightMouseHeld = false;
            Cursor.lockState = CursorLockMode.None;
        }

        if (isRightMouseHeld)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }

    private void HandleCameraMovement()
    {
        if (isRightMouseHeld)
        {
            float speed = cameraSpeed * (Input.GetKey(KeyCode.LeftShift) ? fastSpeedMultiplier : 1f);

            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) direction += cameraTransform.forward;
            if (Input.GetKey(KeyCode.S)) direction -= cameraTransform.forward;
            if (Input.GetKey(KeyCode.A)) direction -= cameraTransform.right;
            if (Input.GetKey(KeyCode.D)) direction += cameraTransform.right;
            if (Input.GetKey(KeyCode.E)) direction += cameraTransform.up;
            if (Input.GetKey(KeyCode.Q)) direction -= cameraTransform.up;

            cameraTransform.position += direction.normalized * (speed * Time.deltaTime);
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
    
    private bool Execute_OnNodeSet()
    {
        var node = GetNodeHit();
        if (node == null) return false;

        OnNodeSet_Signature?.Invoke(node);
        return true;
    }
    
    public void OnNavigate(string maxCorridorWidth)
    {
        if(UIHelper.ValidateInputAsInt(maxCorridorWidth, out int corridorWidth))
        {
            var nodes = mGrid.GetStartEndNodes();
            var path = ILS.Navigate(mGrid, nodes.start, nodes.end, corridorWidth);
            
            if(path != null)
                mGrid.SetPathNode(path);
        }
        else
        {
            Debug.LogWarning("Invalid corridor width input.");
        }
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
