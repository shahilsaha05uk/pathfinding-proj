using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public EDimension DefaultDimension;
    
    public Controller Controller;
    
    public TMP_Dropdown otnDimension;
    public Button btnCreate;
    public Button btnSetStart;
    public Button btnSetEnd;
    public Button btnClear;
    public Button btnNavigate;
    public void Start()
    {
        otnDimension.onValueChanged.AddListener(OnDimensionChanged);
        btnCreate.onClick.AddListener(() => Controller.OnCreateGrid());
        btnSetStart.onClick.AddListener(() => Controller.OnSetStart());
        btnSetEnd.onClick.AddListener(() => Controller.OnSetEnd());
        btnClear.onClick.AddListener(() => Controller.OnClearGrid());
        btnNavigate.onClick.AddListener(() => Controller.OnNavigate());

        otnDimension.value = (int)DefaultDimension;
    }

    private void OnDimensionChanged(int option)
    {
        if(option == 0) Controller.OnDimensionChange(EDimension.Grid3D);
        else if(option == 1) Controller.OnDimensionChange(EDimension.Grid2D);
    }
}
