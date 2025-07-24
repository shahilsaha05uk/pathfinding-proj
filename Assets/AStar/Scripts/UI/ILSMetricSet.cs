using UnityEngine;

public class ILSMetricSet : MetricSet
{
    public PanelLabel CorridorIteration;

    public override void UpdateMetric(AlgorithmType type, EvaluationData data)
    {
        base.UpdateMetric(type, data);
        CorridorIteration.SetValue(data.CorridorIterations.ToString());
    }
}
