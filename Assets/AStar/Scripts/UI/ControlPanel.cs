using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    private Controller Controller;
    
    [Space(5)][Header("Input Fields")]
    public PanelInputField inputGridSize;
    public PanelInputField inputMaxHeight;
    public PanelInputField inputTileSize;
    public PanelInputField inputTileSpacing;
    public PanelInputField inputEvaluationSize;
    
    [Space(5)][Header("Buttons")]
    public Button btnCreate;
    public Button btnSetStart;
    public Button btnSetEnd;
    public Button btnClear;
    public Button btnNavigate;
    public Button btnResetNodes;
    public Button btnEvaluate;
    public Button btnSaveAndExport;
    
    [Space(5)][Header("Sliders")]
    public PanelSlider offsetXSlider;
    public PanelSlider offsetYSlider;
    public PanelSlider offsetZSlider;
    public PanelSlider randomizeSlider;
    public PanelSlider noiseSlider;
    public PanelSlider obstacleDensitySlider;

    [Space(5)][Header("Options")]
    public TMP_Dropdown optionAlgorithmType;

    public Toggle toggleAutoSavePostEvaluation;

    public Action<EvaluationResult> OnResultEvaluated;
    public Action<AlgorithmType, EvaluationData> OnNavigated;

    private AlgorithmType algorithmType;
    
    public void Start()
    {
        btnCreate.onClick.AddListener(OnCreateGridButtonClick);
        btnSetStart.onClick.AddListener(OnStartNodeSetButtonClick);
        btnSetEnd.onClick.AddListener(OnEndNodeSetButtonClick);
        btnClear.onClick.AddListener(OnClearGridButtonClick);
        btnNavigate.onClick.AddListener(OnNavigate);
        btnResetNodes.onClick.AddListener(OnResetNodes);
        btnEvaluate.onClick.AddListener(OnEvaluateButtonClick);
        btnSaveAndExport.onClick.AddListener(OnSaveAndExport);
        
        inputGridSize.onValueChanged += OnGridSizeChanged;
        OnGridSizeChanged(inputGridSize.GetValue());    // Run once at start to set the initial state of the button

        toggleAutoSavePostEvaluation.onValueChanged.AddListener(OnAutosaveValueChanged);
        OnAutosaveValueChanged(toggleAutoSavePostEvaluation.isOn);

        var options = UIHelper.CreateOptionListFromEnum<AlgorithmType>();
        optionAlgorithmType.AddOptions(options);
        optionAlgorithmType.onValueChanged.AddListener(OnAlgorithmChanged);
        OnAlgorithmChanged(optionAlgorithmType.value);

    }

    public void Init(Controller controller)
    {
        Controller = controller;
    }

    private void OnGridSizeChanged(string value)
    {
        btnCreate.interactable = UIHelper.ValidateInputAsInt(inputGridSize.GetValue(), out int gridSize);
    }

    private void OnAutosaveValueChanged(bool value)
    {
        btnSaveAndExport.interactable = !value;
    }


    private void OnCreateGridButtonClick() => Controller.CreateGrid(new GridConfig
    {
        GridSize = UIHelper.ValidateInputAsInt(inputGridSize.GetValue(), out int gridSize) ? gridSize : 10,
        MaxHeight = UIHelper.ValidateInputAsInt(inputMaxHeight.GetValue(), out int maxHeight) ? maxHeight : 7,
        TileSize = UIHelper.ValidateInputAsFloat(inputTileSize.GetValue(), out float tileSize) ? tileSize : 1f,
        TileSpacing = UIHelper.ValidateInputAsFloat(inputTileSpacing.GetValue(), out float tileSpacing) ? tileSpacing : 0.1f,
        Offset = new Vector3(
            offsetXSlider.GetValue(),
            offsetYSlider.GetValue(),
            offsetZSlider.GetValue()
        ),
        ObstacleDensity = obstacleDensitySlider.GetValue(),
        OffsetRandomization = randomizeSlider.GetValue(),
        NoiseScale = noiseSlider.GetValue()
    });

    private void OnClearGridButtonClick() => Controller.ClearGrid();
    
    private void OnStartNodeSetButtonClick() => Controller.SubscribeTo_StartNodeSet();
    
    private void OnEndNodeSetButtonClick() => Controller.SubscribeTo_EndNodeSet();
    
    private void OnNavigate()
    { 
        var data = Controller.OnNavigate(algorithmType);
        OnNavigated?.Invoke(algorithmType, data);
    }

    private void OnResetNodes() => Controller.OnResetPath();

    private void OnAlgorithmChanged(int option)
    {
        algorithmType = UIHelper.GetEnumValueFromOption<AlgorithmType>(option);
    }

    private void OnEvaluateButtonClick()
    {
        UIHelper.ValidateInputAsInt(inputEvaluationSize.GetValue(), out int size);

        var result = Controller.OnEvaluate(size, toggleAutoSavePostEvaluation.isOn);
        OnResultEvaluated?.Invoke(result);
    }

    private void OnSaveAndExport()
    {
        Controller.GetSaveManager().SaveAndExport();
    }
}
