using UnityEngine;
using Debug = UnityEngine.Debug;

public partial class Controller : MonoBehaviour
{
    public Grid3D mGrid;
    [SerializeField] private UI mUI;
    private SaveManager saveManager;

    private PathfindingEvaluator evaluator;
    private PathfindingManager pathfindingManager;
    
    private void Start()
    {
        saveManager = new SaveManager();
        pathfindingManager = new PathfindingManager(mGrid);
        evaluator = new PathfindingEvaluator(mGrid, mUI, pathfindingManager);
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
        if (Input.GetMouseButtonUp(0) && bIsNodeHit)
        {
            if(Execute_OnNodeSet()) UnsubscribeFrom_OnNodeSet();
        }
    }
    
    private void LateUpdate()
    {
        HandleCameraRotation();
        HandleCameraMovement();
    }
    
}
