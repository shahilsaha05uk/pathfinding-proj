using UnityEngine;
using UnityEngine.UI;


public class UI : MonoBehaviour
{
    public Controller Controller;

    [Space(5)][Header("Input Fields")]
    public PanelInputField inputGridSize;
    public PanelInputField inputMaxHeight;
    public PanelInputField inputTileSize;
    public PanelInputField inputTileSpacing;
    public PanelInputField inputMaxCorridorWidth;
    
    [Space(5)][Header("Buttons")]
    public Button btnCreate;
    public Button btnSetStart;
    public Button btnSetEnd;
    public Button btnClear;
    public Button btnNavigateILS;
    public Button btnNavigateGBFS;
    public Button btnEvaluate;
    
    public Button btnSave;
    public Button btnExport;
    public Button btnSaveAndExport;
    public Button btnClearAll;
    
    [Space(5)][Header("Sliders")]
    public PanelSlider offsetXSlider;
    public PanelSlider offsetYSlider;
    public PanelSlider offsetZSlider;
    public PanelSlider randomizeSlider;
    public PanelSlider noiseSlider;
    [Space(5)][Header("Obstacles")]
    public PanelSlider obstacleDensitySlider;

    [Space(5)][Header("Input Fields")]
    public PanelLabel AStarTimeLabel;
    public PanelLabel AStarSpaceLabel;
    [Space(5)]
    public PanelLabel GBFSTimeLabel;
    public PanelLabel GBFSSpaceLabel;
    [Space(5)]
    public PanelLabel ILSWithAStarTimeLabel;
    public PanelLabel ILSWithAStarSpaceLabel;
    public PanelLabel ILSIterationsLabel;
    
    
    public void Start()
    {
        btnCreate.onClick.AddListener(OnCreateGridButtonClick);
        btnSetStart.onClick.AddListener(OnStartNodeSetButtonClick);
        btnSetEnd.onClick.AddListener(OnEndNodeSetButtonClick);
        btnClear.onClick.AddListener(OnClearGridButtonClick);
        btnNavigateILS.onClick.AddListener(OnNavigateILSButtonClick);
        btnNavigateGBFS.onClick.AddListener(OnNavigateGBFSButtonClick);
        btnEvaluate.onClick.AddListener(OnEvaluateButtonClick);
        
        btnSave.onClick.AddListener(OnSaveButtonClick);
        btnExport.onClick.AddListener(OnExportButtonClick);
        btnSaveAndExport.onClick.AddListener(OnSaveAndExportButtonClick);
        btnClearAll.onClick.AddListener(OnClearDataButtonClick);
        
        inputGridSize.onValueChanged += OnGridSizeChanged;
        OnGridSizeChanged(inputGridSize.GetValue());    // Run once at start to set the initial state of the button
    }

    private void OnGridSizeChanged(string value)
    {
        btnCreate.interactable = UIHelper.ValidateInputAsInt(inputGridSize.GetValue(), out int gridSize);
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
    
    private void OnNavigateILSButtonClick() => Controller.OnNavigateILS(inputMaxCorridorWidth.GetValue());
    
    private void OnNavigateGBFSButtonClick() => Controller.OnNavigateGBFS();

    private void OnEvaluateButtonClick()
    {
        var result = Controller.OnEvaluate();
        
        AStarSpaceLabel.SetValue(result.AStarSpace.ToString());
        AStarTimeLabel.SetValue(result.AStarTime.ToString());
        
        GBFSSpaceLabel.SetValue(result.GBFSSpace.ToString());
        GBFSTimeLabel.SetValue(result.GBFSTime.ToString());
        
        ILSWithAStarSpaceLabel.SetValue(result.ILSWithAStarSpace.ToString());
        ILSWithAStarTimeLabel.SetValue(result.ILSWithAStarTime.ToString());
        ILSIterationsLabel.SetValue(result.ILSIterations.ToString());
    }

    private void OnSaveButtonClick() => Controller.Save();
    
    private void OnExportButtonClick() => Controller.Export();

    private void OnSaveAndExportButtonClick() => Controller.SaveAndExport();
    
    private void OnClearDataButtonClick() => Controller.ClearData();
    
}
