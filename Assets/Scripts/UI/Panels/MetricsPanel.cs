using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MetricsPanel : MonoBehaviour
{
    [Space(5)][Header("Metrics")]
    public MetricSet Metric;
    public void OnNavigated(AlgorithmType type, EvaluationData data)
    {
        Metric.UpdateMetric(type, data);
    }
}
