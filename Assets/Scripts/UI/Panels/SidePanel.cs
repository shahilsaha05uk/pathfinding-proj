using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SidePanel : MonoBehaviour
{
    private Controller Controller;
    [SerializeField] private SO_GridConfig DebugGridConfig;
    public Action<AlgorithmType, EvaluationData> OnEvaluationDataChangedSignature;

    [SerializeField] private ControlPanel controlPanel;
    [SerializeField] private ParamsPanel paramsPanel;
    [SerializeField] private NavigatorsPanel navigatorsPanel;
    [SerializeField] private EvaluatorsPanel evaluatorsPanel;

    public void Start()
    {
        controlPanel.Init(Controller);
        controlPanel.OnCreateGrid += OnCreateGrid;
        controlPanel.OnCreateDebugGrid += OnDebugCreateButtonClick;

        paramsPanel.OnGridSizeChangedSignature += OnGridSizeChanged;

        navigatorsPanel.Init(Controller);
        navigatorsPanel.OnNavigatedSignature += OnNavigate;

        evaluatorsPanel.Init(Controller);
    }

    public void Init(Controller controller)
    {
        Controller = controller;
    }

    private void OnNavigate(AlgorithmType type, EvaluationData data)
    {
        OnEvaluationDataChangedSignature?.Invoke(type, data);
    }

    private void OnGridSizeChanged(int value, bool success)
    {
        controlPanel.UpdateCreateButtonInteractability(success);
    }

    private void OnCreateGrid()
    {
        var gridConfig = CreateGridConfig();
        if (gridConfig == null)
        {
            Debug.LogError("Grid Config is null!");
            return;
        }
        var grid = Grid3D.Instance;
        grid.Create(gridConfig);
    }

    private void OnDebugCreateButtonClick()
    {
        if (DebugGridConfig == null)
        {
            Debug.LogError("Debug Grid Config isnt set!!");
        }
        var grid = Grid3D.Instance;
        var config = DebugGridConfig.GetDebugGridConfig();
        
        grid.Create(config);

        if (config.bSetEndpoints)
        {
            var endPoints = DebugGridConfig.GetStartEndNodes();
            var startNode = grid.GetNodeAt(endPoints.start);
            var endNode = grid.GetNodeAt(endPoints.end);
            grid.SetStartNode(startNode);
            grid.SetEndNode(endNode);
        }

        navigatorsPanel.UpdateAlgorithmType(config.AlgorithmType);
    }

    private GridConfig CreateGridConfig()
    {
        GridConfig gridConfig = new();
        if (paramsPanel == null)
        {
            Debug.LogError("ParamsPanel is not set!");
            return gridConfig;
        }

        paramsPanel.UpdateConfig(ref gridConfig);

        return gridConfig;
    }
}
