using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavigatorsPanel : MonoBehaviour
{
    private Controller Controller;
    [SerializeField] private AlgorithmType algorithmType;

    public TMP_Dropdown optionAlgorithmType;
    public Button btnNavigate;
    public Button btnResetNodes;

    public Action<AlgorithmType, EvaluationData> OnNavigatedSignature;

    void Start()
    {
        btnNavigate.onClick.AddListener(OnNavigate);
        btnResetNodes.onClick.AddListener(OnResetNodes);
        
        var options = UIHelper.CreateOptionListFromEnum<AlgorithmType>();
        optionAlgorithmType.AddOptions(options);
        optionAlgorithmType.onValueChanged.AddListener(OnAlgorithmChanged);
        UpdateAlgorithmType(algorithmType);
    }

    public void Init(Controller controller)
    {
        Controller = controller;
    }

    public void UpdateAlgorithmType(AlgorithmType type)
    {
        algorithmType = type;
        optionAlgorithmType.value = (int)algorithmType;
    }

    private void OnNavigate()
    {
        var data = Controller.OnNavigate(algorithmType);
        OnNavigatedSignature?.Invoke(algorithmType, data);
    }

    private void OnResetNodes() => Controller.OnResetPath();

    private void OnAlgorithmChanged(int option)
    {
        algorithmType = UIHelper.GetEnumValueFromOption<AlgorithmType>(option);
    }
}
