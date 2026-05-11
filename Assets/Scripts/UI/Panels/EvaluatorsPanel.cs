using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EvaluatorsPanel : MonoBehaviour
{
    private Controller Controller;
    [SerializeField] private Button btnEvaluate;
    [SerializeField] private PanelInputField inputEvaluationSize;

    [SerializeField] private NavigatorsPanel navigatorsPanel;
    [SerializeField] private GameObject togglesContainer;
    [SerializeField] private PanelToggle togglePrefab;
    [SerializeField] private SO_AlgorithmConfig algorithmConfig;
    [SerializeField] private SO_EvaluatorConfig evaluatorConfig;



    private SerializedDictionary<AlgorithmType, TogglerData> algorithmToggles;

    void Start()
    {
        btnEvaluate.onClick.AddListener(OnEvaluateButtonClick);

        algorithmToggles = algorithmConfig.GetData();
        foreach (var type in algorithmToggles.Keys)
        {
            var value = algorithmToggles[type];
            var toggler = GameObject.Instantiate(togglePrefab);
            toggler.Init(value.Label, value.IsOn);
            toggler.transform.SetParent(togglesContainer.transform, false);
        }
    }

    public void Init(Controller controller)
    {
        Controller = controller;
    }

    private void OnEvaluateButtonClick()
    {
        Controller.OnEvaluate();
    }
}
