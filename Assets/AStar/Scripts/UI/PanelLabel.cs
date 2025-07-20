using TMPro;
using UnityEngine;

public class PanelLabel : MonoBehaviour
{
    public string label;
    private string value;
    public string defaultValue = "N/A";
    
    public TMP_Text labelText;
    public TMP_Text valueText;
    
    void Start()
    {
        labelText.text = label;
        valueText.text = value;
    }

    public void SetLabel(string newLabel) => label = newLabel;

    public void SetValue(string newValue, string hexColor = "#000")
    {
        value = newValue?? defaultValue;
        valueText.text = value;
    }

    public void SetDefaultValue(string newDefaultValue) => defaultValue = newDefaultValue;
    
    public string GetValue() => string.IsNullOrEmpty(value) ? defaultValue : value;
}
