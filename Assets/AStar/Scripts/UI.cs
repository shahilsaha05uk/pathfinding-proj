using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public EDimension DefaultDimension;
    
    public Controller Controller;
    
    public Button btnCreate;
    public Button btnSetStart;
    public Button btnSetEnd;
    public Button btnClear;
    public Button btnNavigate;
    public void Start()
    {
        btnCreate.onClick.AddListener(OnCreateGridButtonClick);
        btnSetStart.onClick.AddListener(OnStartNodeSetButtonClick);
        btnSetEnd.onClick.AddListener(OnEndNodeSetButtonClick);
        btnClear.onClick.AddListener(OnClearGridButtonClick);
        btnNavigate.onClick.AddListener(OnNavigateButtonClick);
    }

    private void OnCreateGridButtonClick() => Controller.CreateGrid();
    private void OnClearGridButtonClick() => Controller.ClearGrid();
    
    private void OnStartNodeSetButtonClick()
    {
        Controller.SubscribeTo_StartNodeSet();
    }
    
    private void OnEndNodeSetButtonClick()
    {
        Controller.SubscribeTo_EndNodeSet();
    }
    
    private void OnNavigateButtonClick()
    {
        Controller.OnNavigate();
    }
}
