using System;
using UnityEngine;

public class UI : MonoBehaviour
{
    public Controller Controller;
    public MetricsPanel metricsUI;
    public SidePanel sidePanel;
    public ColorKeyPanel colorKeyPanel;
    public ConfigPanel configPanel;

    public PanelStatus statusPanel;

    private void Start()
    {
        sidePanel.OnEvaluationDataChangedSignature += metricsUI.OnNavigated;
        Init(Controller);
    }

    public void Init(Controller controller)
    {
        Controller = controller;
        sidePanel.Init(controller);
    }
}
