using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelSlider : MonoBehaviour
{
    private float currentValue;
    
    public string label;
    public float minValue = 0f;
    public float maxValue = 1f;
    public bool bUseWholeNumbers = false;
    public float defaultValue = 0.5f;
    
    
    public Slider slider;
    public TMP_Text txtLabel;
    public TMP_Text txtValue;

    private void Start()
    {
        txtLabel.text = label;
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = defaultValue;
        slider.wholeNumbers = bUseWholeNumbers; // Allow decimal values
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(slider.value);
    }

    private void OnSliderValueChanged(float value)
    {
        currentValue = value;
        txtValue.text = $"{currentValue}";
    }

    public float GetValue()
    {
        return slider.value;
    }
}
