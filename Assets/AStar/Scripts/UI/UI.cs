using System;
using UnityEngine;

public class UI : MonoBehaviour
{
    public Controller Controller;
    public MetricsPanel metricsUI;
    public ControlPanel controlPanel;

    public PanelStatus statusPanel;

    private void Start()
    {
        controlPanel.OnResultEvaluated += OnResultEvaluated;
        controlPanel.OnNavigated += metricsUI.OnNavigated;
        metricsUI.OnSaveAndExport += OnSavedAndExport;
        Init(Controller);
    }

    public void Init(Controller controller)
    {
        Controller = controller;
        controlPanel.Init(controller);
    }

    private void OnSavedAndExport()
    {
        var status = Controller.GetSaveManager().SaveAndExport();
        statusPanel.SetValue(status, "#00f");
    }

    private void OnResultEvaluated(EvaluationResult result)
    {
        statusPanel.SetValue("Evaluation completed", "#0f0");
    }
}
