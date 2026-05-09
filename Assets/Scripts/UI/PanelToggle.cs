using UnityEngine;
using UnityEngine.UI;

public class PanelToggle : LabelledPanel
{
    [SerializeField] private Toggle toggle;
    public bool GetIsOn() => toggle.isOn;
    public void SetToggle(bool value) => toggle.isOn = value;

    public void Init(string label, bool isOn = false)
    {
        SetLabel(label);
        SetToggle(isOn);
    }
}
