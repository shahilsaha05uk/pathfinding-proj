using UnityEngine;

public class UI : MonoBehaviour
{
    public Controller Controller;
    public MetricsUI metricsUI;
    public ControlPanel controlPanel;
    private void Start()
    {
        controlPanel.OnResultEvaluated += OnResultEvaluated;
        Init(Controller);
    }
    
    public void Init(Controller controller)
    {
        Controller = controller;
        metricsUI.Init(controller);
        controlPanel.Init(controller);
    }

    private void OnResultEvaluated(EvaluationResult result)
    {
        metricsUI.SetEvaluationResult(result);
    }
}
