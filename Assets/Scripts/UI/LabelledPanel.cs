using UnityEngine.UI;

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