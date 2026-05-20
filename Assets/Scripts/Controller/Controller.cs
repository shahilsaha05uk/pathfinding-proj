using System;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class Controller : MonoBehaviour
{
    [SerializeField] private UI ui;
    [SerializeField] private PathfindingEvaluator evaluator;
    [SerializeField] private PathfindingManager pathfindingManager;
    private EvaluationDataSaver saveManager;

    [SerializeField] private Grid3D grid;

    [Header("Camera Properties")]
    [SerializeField] private float cameraSpeed = 5f;
    [SerializeField] private float fastSpeedMultiplier = 3f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform cameraTransform;
    private float yaw = 0f;
    private float pitch = 0f;
    private bool bIsRightMouseHeld = false;

    [Header("Grid Properties")]
    [SerializeField] private bool bIsNodeHit = false;
    
    public Action<Node> OnNodeSet_Signature;

    private void Start()
    {
        saveManager = new EvaluationDataSaver();
    }

    private void Update()
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
        if (Input.GetMouseButtonUp(0) && bIsNodeHit && !EventSystem.current.IsPointerOverGameObject())
        {
            if(Execute_OnNodeSet()) UnsubscribeFrom_OnNodeSet();
        }
    }
    
    private void LateUpdate()
    {
        HandleCameraRotation();
        HandleCameraMovement();
    }

    public void UpdateConfigPanel(EvaluationLog data)
        => ui.configPanel.OnConfigChanged?.Invoke(data);
    public void UpdateAlgorithmType(AlgorithmType type)
        => ui.configPanel.OnAlgorithmComplete?.Invoke(type);

    public void UpdateBatchCount(int count)
        => ui.configPanel.OnBatchComplete?.Invoke(count);

    public EvaluationDataSaver GetSaveManager() => saveManager;
}
