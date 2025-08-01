using System;
using UnityEngine;

public class UI : MonoBehaviour
{
    public Controller Controller;
    public MetricsPanel metricsUI;
    public SidePanel sidePanel;

    public PanelStatus statusPanel;

    private void Start()
    {
        sidePanel.OnEvaluationDataChangedSignature += metricsUI.OnNavigated;
        metricsUI.OnSaveAndExport += OnSavedAndExport;
        Init(Controller);
    }

    public void Init(Controller controller)
    {
        Controller = controller;
        sidePanel.Init(controller);
    }

    private void OnSavedAndExport()
    {
        var status = Controller.SaveAndExport();
        statusPanel.SetValue(status, "#00f");
    }
}
