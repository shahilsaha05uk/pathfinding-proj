using TMPro;

public class PanelLabel : TMP_LabelledPanel
{
    private string value;
    public string defaultValue = "N/A";
    
    public TMP_Text valueText;
    
    void Start()
    {
        valueText.text = value;
    }

    public void SetValue(string newValue, string hexColor = "#000")
    {
        value = newValue?? defaultValue;
        valueText.text = value;
    }

    public void SetDefaultValue(string newDefaultValue) => defaultValue = newDefaultValue;
    
    public string GetValue() => string.IsNullOrEmpty(value) ? defaultValue : value;
}
