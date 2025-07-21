using System;
using UnityEngine;
using UnityEngine.UI;

public class MetricsPanel : MonoBehaviour
{
    private Controller Controller;

    [Space(5)][Header("Metrics")]
    public MetricSet AStarMetric;
    public ILSMetricSet IlsAStarMetric;
    public MetricSet GbfsMetric;
    public ILSMetricSet IlsGbfsMetric;
    
    public Action OnSaveAndExport;

    public void Init(Controller controller)
    {
        Controller = controller;     
    }

    public void SetEvaluationResult(EvaluationResult result)
    {
        if(result == null)
        {
            Debug.LogWarning("Evaluation result is null. Cannot update UI.");
            return;
        }
        
        AStarMetric.UpdateMetric(result.AStar);
        GbfsMetric.UpdateMetric(result.GBFS);
        IlsAStarMetric.UpdateMetric(result.ILSWithAStar);
        IlsGbfsMetric.UpdateMetric(result.ILSWithGBFS);
    }
    
    public void OnNavigated(AlgorithmType type, EvaluationData data)
    {
        switch (type)
        {
            case AlgorithmType.AStar:
                AStarMetric.UpdateMetric(data);
                break;
            case AlgorithmType.GBFS:
                GbfsMetric.UpdateMetric(data);
                break;
            case AlgorithmType.ILS_AStar:
                IlsAStarMetric.UpdateMetric(data);
                break;
            case AlgorithmType.ILS_GBFS:
                IlsGbfsMetric.UpdateMetric(data);
                break;
        }
    }
}
