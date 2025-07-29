using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MetricSet : TMP_LabelledPanel
{
    [Space(5)]
    [Header("Input Fields")]

    public PanelLabel Time;
    public PanelLabel PathLength;
    public PanelLabel PathCost;

    public void Start()
    {
        txtLabel.text = $"-- {txtLabel.text} --";
    }

    public virtual void UpdateMetric(AlgorithmType type, EvaluationData data)
    {
        txtLabel.text = $"-- {type.ToString()} --";

        Time.SetValue(data.TimeTaken.ToString());
        PathCost.SetValue(data.PathCost.ToString());
        PathLength.SetValue(data.PathLength.ToString());
    }
}
