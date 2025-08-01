using System;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class Controller : MonoBehaviour
{
    [SerializeField] private UI ui;
    [SerializeField] private PathfindingEvaluator evaluator;
    [SerializeField] private PathfindingManager pathfindingManager;
    private EvaluationDataSaver evaluationDataSaveManager;

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
        evaluationDataSaveManager = new EvaluationDataSaver();
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

    public EvaluationDataSaver GetSaveManager() => evaluationDataSaveManager;

    public string SaveAndExport()
    {
        var results = evaluator.GetEvaluationResults();
        var gridData = grid.GetGridData();

        var data = evaluationDataSaveManager.CreateSaveData(gridData, results);
        evaluationDataSaveManager.AddSaveData(data);
        var status = evaluationDataSaveManager.SaveAndExport(gridData.GridSize, Mathf.FloorToInt(gridData.ObstacleDensity * 100));

        evaluator.ClearResults();
        return status;
    }
}
