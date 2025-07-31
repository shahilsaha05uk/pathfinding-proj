using UnityEngine;
using UnityEngine.UI;

public class PanelToggle : LabelledPanel
{
    [SerializeField] private Toggle mToggle;
    public bool GetIsOn() => mToggle.isOn;
}
