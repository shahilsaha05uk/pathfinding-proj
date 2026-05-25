using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorKeyPanelEntry : MonoBehaviour
{
    public TMP_Text Label;
    public Image ColorImage;
    public void Init(AlgorithmType type, Color color)
    {
        Label.text = type.ToString();
        ColorImage.color = color;
    }
}
