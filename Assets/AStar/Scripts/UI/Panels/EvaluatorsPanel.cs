using System;
using UnityEngine;
using UnityEngine.UI;

public class EvaluateAlgorithms
{
    public bool AStar;
    public bool GBFS;
    public bool Dijkstra;
    public bool JPS;
    public bool ILSAStar;
    public bool ILSGBFS;
    public bool ILSDijkstra;
}

public class EvaluatorsPanel : MonoBehaviour
{
    private Controller Controller;
    [SerializeField] private Button btnEvaluate;
    [SerializeField] private PanelInputField inputEvaluationSize;

    [SerializeField] private Toggle toggleAStar;
    [SerializeField] private Toggle toggleDijkstra;
    [SerializeField] private Toggle toggleGBFS;
    [SerializeField] private Toggle toggleJPS;
    [SerializeField] private Toggle toggleILSAStar;
    [SerializeField] private Toggle toggleILSGBFS;
    [SerializeField] private Toggle toggleILSDijkstra;

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
        Controller.OnEvaluate(size, new EvaluateAlgorithms()
        {
            AStar = toggleAStar.isOn,
            Dijkstra = toggleDijkstra.isOn,
            GBFS = toggleGBFS.isOn,
            JPS = toggleJPS.isOn,
            ILSAStar = toggleILSAStar.isOn,
            ILSGBFS = toggleILSGBFS.isOn,
            ILSDijkstra = toggleILSDijkstra.isOn
        });
    }
}
