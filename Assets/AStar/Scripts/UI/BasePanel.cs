using TMPro;
using UnityEngine;

public abstract class BasePanel : MonoBehaviour
{
}

public class LabelledPanel : BasePanel
{
    public string defaultLabel = "Default Label";
    public TMP_Text txtLabel;

    private void OnValidate()
    {
        if (txtLabel != null && txtLabel.text != defaultLabel)
        {
            txtLabel.text = defaultLabel;
        }
    }

    public void SetLabel(string value) => txtLabel.text = value ?? defaultLabel;
}