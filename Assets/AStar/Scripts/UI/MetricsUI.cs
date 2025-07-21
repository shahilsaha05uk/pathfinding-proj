using UnityEngine;
using UnityEngine.UI;

public class MetricsUI : MonoBehaviour
{
    private Controller Controller;
    
    [Space(5)][Header("Input Fields")]
    public PanelLabel AStarTimeLabel;
    public PanelLabel AStarSpaceLabel;
    public PanelLabel AStarPathLengthLabel;
    public PanelLabel AStarPathCostLabel;
    [Space(5)]
    public PanelLabel GBFSTimeLabel;
    public PanelLabel GBFSSpaceLabel;
    public PanelLabel GBFSPathLengthLabel;
    public PanelLabel GBFSPathCostLabel;
    [Space(5)]
    public PanelLabel ILSWithAStarTimeLabel;
    public PanelLabel ILSWithAStarSpaceLabel;
    public PanelLabel ILSWithAStarPathCostLabel;
    public PanelLabel ILSWithAStarPathLengthLabel;
    public PanelLabel ILSWithAStarCorridorIterationLabel;
    [Space(5)]
    public PanelLabel ILSWithGBFSTimeLabel;
    public PanelLabel ILSWithGBFSSpaceLabel;
    public PanelLabel ILSWithGBFSPathCostLabel;
    public PanelLabel ILSWithGBFSPathLengthLabel;
    public PanelLabel ILSWithGBFSCorridorIterationLabel;

    [Space(5)]
    public Button btnSave;
    public Button btnExport;
    public Button btnSaveAndExport;
    public Button btnClearAll;
    
    public void Init(Controller controller)
    {
        Controller = controller;
        
        btnSave.onClick.AddListener(OnSaveButtonClick);
        btnExport.onClick.AddListener(OnExportButtonClick);
        btnSaveAndExport.onClick.AddListener(OnSaveAndExportButtonClick);
        btnClearAll.onClick.AddListener(OnClearDataButtonClick);
    }
    
    public void SetEvaluationResult(EvaluationResult result)
    {
        if(result == null)
        {
            Debug.LogWarning("Evaluation result is null. Cannot update UI.");
            return;
        }
        
        // A*
        var stats = result.AStar;
        AStarSpaceLabel.SetValue(stats.SpaceTaken.ToString());
        AStarTimeLabel.SetValue(stats.TimeTaken.ToString());
        AStarPathCostLabel.SetValue(stats.PathCost.ToString());
        AStarPathLengthLabel.SetValue(stats.PathLength.ToString());

        // ILS with A*
        stats = result.ILSWithAStar;
        ILSWithAStarSpaceLabel.SetValue(stats.SpaceTaken.ToString());
        ILSWithAStarTimeLabel.SetValue(stats.TimeTaken.ToString());
        ILSWithAStarPathCostLabel.SetValue(stats.PathCost.ToString());
        ILSWithAStarPathLengthLabel.SetValue(stats.PathLength.ToString());
        ILSWithAStarCorridorIterationLabel.SetValue(stats.CorridorIterations.ToString());
        
        // GBFS
        stats = result.GBFS;
        GBFSSpaceLabel.SetValue(stats.SpaceTaken.ToString());
        GBFSTimeLabel.SetValue(stats.TimeTaken.ToString());
        GBFSPathCostLabel.SetValue(stats.PathCost.ToString());
        GBFSPathLengthLabel.SetValue(stats.PathLength.ToString());
        
        // ILS with GBFS
        stats = result.ILSWithGBFS;
        ILSWithGBFSSpaceLabel.SetValue(stats.SpaceTaken.ToString());
        ILSWithGBFSTimeLabel.SetValue(stats.TimeTaken.ToString());
        ILSWithGBFSPathCostLabel.SetValue(stats.PathCost.ToString());
        ILSWithGBFSPathLengthLabel.SetValue(stats.PathLength.ToString());
        ILSWithGBFSCorridorIterationLabel.SetValue(stats.CorridorIterations.ToString());
    }
    
    private void OnSaveButtonClick() => Controller.Save();
    
    private void OnExportButtonClick() => Controller.Export();

    private void OnSaveAndExportButtonClick() => Controller.SaveAndExport();
    
    private void OnClearDataButtonClick() => Controller.ClearData();
}
