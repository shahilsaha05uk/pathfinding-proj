using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Controller Controller;

    public PanelInputField inputGridSize;
    public PanelInputField inputMaxHeight;
    public PanelInputField inputTileSize;
    public PanelInputField inputTileSpacing;
    public PanelInputField inputMaxCorridorWidth;
    
    public Button btnCreate;
    public Button btnSetStart;
    public Button btnSetEnd;
    public Button btnClear;
    public Button btnNavigate;

    public PanelSlider offsetXSlider;
    public PanelSlider offsetYSlider;
    public PanelSlider offsetZSlider;
    public PanelSlider randomizeSlider;
    public PanelSlider noiseSlider;

    public void Start()
    {
        btnCreate.onClick.AddListener(OnCreateGridButtonClick);
        btnSetStart.onClick.AddListener(OnStartNodeSetButtonClick);
        btnSetEnd.onClick.AddListener(OnEndNodeSetButtonClick);
        btnClear.onClick.AddListener(OnClearGridButtonClick);
        btnNavigate.onClick.AddListener(OnNavigateButtonClick);
        
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
        OffsetRandomization = randomizeSlider.GetValue(),
        NoiseScale = noiseSlider.GetValue()
    });
    private void OnClearGridButtonClick() => Controller.ClearGrid();
    
    private void OnStartNodeSetButtonClick()
    {
        Controller.SubscribeTo_StartNodeSet();
    }
    
    private void OnEndNodeSetButtonClick()
    {
        Controller.SubscribeTo_EndNodeSet();
    }
    
    private void OnNavigateButtonClick()
    {
        Controller.OnNavigate(inputMaxCorridorWidth.GetValue());
    }
}
