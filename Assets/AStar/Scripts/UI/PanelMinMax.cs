using TMPro;
using UnityEngine;

public class PanelMinMax : TMP_LabelledPanel
{
    private float minValue = 0f;
    private float maxValue = 1f;
    
    public float defaultMinValue = 0.0f;
    public float defaultMaxValue = 1.0f;

    public TMP_InputField.ContentType FieldType;
    public TMP_InputField min;
    public TMP_InputField max;

    private void Start()
    {
        min.text = defaultMinValue.ToString();
        max.text = defaultMaxValue.ToString();

        min.onValueChanged.AddListener(OnMinValueChanged);
        max.onValueChanged.AddListener(OnMaxValueChanged);
    }

    private void OnMinValueChanged(string value)
    {
        if (float.TryParse(value, out minValue)) minValue = 0f;
    }

    private void OnMaxValueChanged(string value)
    {
        if (float.TryParse(value, out maxValue)) maxValue = 0f;
    }

    public (float min, float max) GetValue() => (minValue, maxValue);
}
