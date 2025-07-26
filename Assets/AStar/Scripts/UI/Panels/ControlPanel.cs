using System;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    private Controller Controller;

    public Action OnCreateGrid;
    public Action OnCreateDebugGrid;

    public Button btnCreate;
    public Button btnCreateDebug;
    public Button btnSetStart;
    public Button btnSetEnd;
    public Button btnClear;

    void Start()
    {
        btnCreate.onClick.AddListener(OnCreateGridButtonClick);
        btnCreateDebug.onClick.AddListener(OnDebugCreateButtonClick);
        btnSetStart.onClick.AddListener(OnStartNodeSetButtonClick);
        btnSetEnd.onClick.AddListener(OnEndNodeSetButtonClick);
        btnClear.onClick.AddListener(OnClearGridButtonClick);
    }

    public void Init(Controller controller)
    {
        Controller = controller;
    }

    public void UpdateCreateButtonInteractability(bool value)
    {
        btnCreate.interactable = value;
    }

    private void OnDebugCreateButtonClick()
    {
        OnCreateDebugGrid?.Invoke();
    }

    private void OnCreateGridButtonClick()
    {
        OnCreateGrid?.Invoke();
    }

    private void OnClearGridButtonClick() => Controller.ClearGrid();

    private void OnStartNodeSetButtonClick() => Controller.SubscribeTo_StartNodeSet();

    private void OnEndNodeSetButtonClick() => Controller.SubscribeTo_EndNodeSet();
}
