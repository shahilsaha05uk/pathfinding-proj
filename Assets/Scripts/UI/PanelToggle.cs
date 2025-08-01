using UnityEngine;
using UnityEngine.UI;

public class PanelToggle : LabelledPanel
{
    [SerializeField] private Toggle toggle;
    public bool GetIsOn() => toggle.isOn;
}
