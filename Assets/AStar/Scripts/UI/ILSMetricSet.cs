using UnityEngine;

public class ILSMetricSet : MetricSet
{
    public PanelLabel CorridorIteration;

    public override void UpdateMetric(EvaluationData data)
    {
        base.UpdateMetric(data);
        CorridorIteration.SetValue(data.CorridorIterations.ToString());
    }
}
