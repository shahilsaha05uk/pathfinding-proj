using System;
using TMPro;
using UnityEngine;

public class PanelInputField : MonoBehaviour
{
    private string fieldValue;
    public Action<string> onValueChanged;
    
    public TMP_InputField inputField;
    public TMP_InputField.ContentType FieldType;
    public string PlaceholderText;
    public string defaultValue;

    public void Start()
    {
        inputField.placeholder.GetComponent<TMP_Text>().text = PlaceholderText;
        inputField.onValueChanged.AddListener(OnInputValueChanged);
        inputField.contentType = FieldType;
        inputField.text = defaultValue;
    }
    
    public void SetValue(string value)
    {
        fieldValue = value;
        inputField.text = value;
    }
    public string GetValue()
    {
        return fieldValue;
    }
    
    private void OnInputValueChanged(string value)
    {
        fieldValue = value;
        onValueChanged?.Invoke(value);
    }
}
