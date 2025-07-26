using System;
using UnityEngine;
using UnityEngine.UI;

public class EvaluatorsPanel : MonoBehaviour
{
    private Controller Controller;
    [SerializeField] private Button btnEvaluate;
    [SerializeField] private PanelInputField inputEvaluationSize;

    void Start()
    {
        btnEvaluate.onClick.AddListener(OnEvaluateButtonClick);

    }

    public void Init(Controller controller)
    {
        Controller = controller;
    }

    private void OnEvaluateButtonClick()
    {
        UIHelper.ValidateInputAsInt(inputEvaluationSize.GetValue(), out int size);
        Controller.OnEvaluate(size);
    }
}
