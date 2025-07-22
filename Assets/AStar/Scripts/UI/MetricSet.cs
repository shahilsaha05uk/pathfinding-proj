using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MetricSet : MonoBehaviour
{
    [Space(5)]
    [Header("Input Fields")]
    public string Label;

    public TMP_Text txtLabel;
    public PanelLabel Time;
    public PanelLabel PathLength;
    public PanelLabel PathCost;


    public void Start()
    {
        txtLabel.text = $"-- {Label} --";
    }

    public virtual void UpdateMetric(AlgorithmType type, EvaluationData data)
    {
        txtLabel.text = $"-- {type.ToString()} --";

        Time.SetValue(data.TimeTaken.ToString());
        PathCost.SetValue(data.PathCost.ToString());
        PathLength.SetValue(data.PathLength.ToString());
    }
}
