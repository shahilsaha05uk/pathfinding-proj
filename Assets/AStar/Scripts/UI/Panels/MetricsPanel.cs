using System;
using UnityEngine;
using UnityEngine.UI;

public class MetricsPanel : MonoBehaviour
{
    [Space(5)][Header("Metrics")]
    public MetricSet Metric;
    
    public Action OnSaveAndExport;

    public void OnNavigated(AlgorithmType type, EvaluationData data)
    {
        Metric.UpdateMetric(type, data);
    }
}
