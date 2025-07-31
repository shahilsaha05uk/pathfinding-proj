using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour
{
}

public class TMP_LabelledPanel : BasePanel
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

public class LabelledPanel : BasePanel
{
    public string defaultLabel = "Default Label";
    public Text txtLabel;

    private void OnValidate()
    {
        if (txtLabel != null && txtLabel.text != defaultLabel)
        {
            txtLabel.text = defaultLabel;
        }
    }

    public void SetLabel(string value) => txtLabel.text = value ?? defaultLabel;
}