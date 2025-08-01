using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelSlider : TMP_LabelledPanel
{
    private float currentValue;
    
    public float minValue = 0f;
    public float maxValue = 1f;
    public bool bUseWholeNumbers = false;
    public float defaultValue = 0.5f;
    
    public Slider slider;
    public TMP_InputField txtInput;

    private void Start()
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = defaultValue;
        slider.wholeNumbers = bUseWholeNumbers; // Allow decimal values
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        txtInput.onValueChanged.AddListener(OnInputFieldValueChanged);

        OnSliderValueChanged(slider.value);
    }

    public void UpdateValue(float value)
    {
        OnSliderValueChanged(value);
    }

    private void OnInputFieldValueChanged(string inputValue)
    {
        slider.value = float.TryParse(inputValue, out float value) ? Mathf.Clamp(value, minValue, maxValue) : defaultValue;
    }

    private void OnSliderValueChanged(float value)
    {
        currentValue = value;
        txtInput.text = currentValue.ToString(bUseWholeNumbers ? "0" : "F2");
    }

    public float GetValue() => slider.value;
}
