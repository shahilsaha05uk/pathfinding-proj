using TMPro;
using UnityEngine;

public class PanelStatus : MonoBehaviour
{
    private string value;
    public string defaultValue = "status...";

    public TMP_Text valueText;

    void Start()
    {
        valueText.text = defaultValue;
    }

    public void SetValue(string newValue, string hexColor = "#000")
    {
        value = $"status: {newValue}" ?? defaultValue;
        valueText.text = value;
    }

    public void SetDefaultValue(string newDefaultValue) => defaultValue = newDefaultValue;
}
