using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;

public partial class Controller : MonoBehaviour
{
    public Grid3D mGrid;
    [SerializeField] private UI mUI;
    private EvaluationDataSaver evaluationDataSaveManager;

    [SerializeField] private PathfindingEvaluator evaluator;
    [SerializeField] private PathfindingManager pathfindingManager;
    
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
}
