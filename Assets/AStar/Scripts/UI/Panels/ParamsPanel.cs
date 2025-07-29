using System;
using UnityEngine;

public class ParamsPanel : MonoBehaviour
{
    private int gridSize;
    [Space(5)]
    [SerializeField] private PanelInputField inputGridSize;
    [SerializeField] private PanelInputField inputMaxHeight;

    [Space(5)]
    [SerializeField] private PanelMinMax inputOffsetX;
    [SerializeField] private PanelMinMax inputOffsetY;

    [Space(5)]
    public PanelSlider sliderNoise;

    public ObstaclePanel obstaclePanel;
    public Action<int, bool> OnGridSizeChangedSignature;

    void Start()
    {
        inputGridSize.onValueChanged += OnGridSizeChanged;

        OnGridSizeChanged(inputGridSize.GetValue());
    }

    public void UpdateConfig(ref GridConfig config)
    {
        config.OffsetX = inputOffsetX.GetValue();
        config.OffsetY = inputOffsetY.GetValue();
        config.GridSize = gridSize;
        config.ObstacleDensity = obstaclePanel.GetObstacleDensity();
        config.NoiseScale = sliderNoise.GetValue();
        config.MaxHeight = UIHelper.ValidateInputAsInt(inputMaxHeight.GetValue(), out int maxHeight) ? maxHeight : 7;
    }

    private void OnGridSizeChanged(string value)
    {
        if (UIHelper.ValidateInputAsInt(
            inputGridSize.GetValue(),
            out int gridSize))
        {
            this.gridSize = gridSize;
            OnGridSizeChangedSignature?.Invoke(gridSize, true);
        }
    }
}
