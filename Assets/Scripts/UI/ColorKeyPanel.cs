using AYellowpaper.SerializedCollections;
using UnityEngine;

public class ColorKeyPanel : TMP_LabelledPanel
{
    [Space(5)]
    [Header("Input Fields")]
    public SO_AlgorithmConfig Config;
    public ColorKeyPanelEntry ColorKeyPrefab;
    public GameObject ColorKeyContainer;

    public void Start()
    {
        if (Config == null)
            throw new System.Exception("AlgorithmConfig SO is not assigned in the inspector.");

        txtLabel.text = $"-- {txtLabel.text} --";

        var Algorithm = Config.ColorKeys;
        var containerTransform = ColorKeyContainer.transform;
        foreach (var type in Algorithm.Keys)
        {
            var color = Algorithm[type];
            var colorKeyInstance = GameObject.Instantiate(ColorKeyPrefab);
            colorKeyInstance.Init(type, color);
            colorKeyInstance.transform.SetParent(containerTransform, false);
        }
    }

}
