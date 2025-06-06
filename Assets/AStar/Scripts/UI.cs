using System;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Controller Controller;
    
    public Button btnCreate;
    public Button btnSetStart;
    public Button btnSetEnd;
    public Button btnClear;
    public Button btnNavigate;

    public void Start()
    {
        btnCreate.onClick.AddListener(() => Controller.OnCreateGrid());
        btnSetStart.onClick.AddListener(() => Controller.OnSetStart());
        btnSetEnd.onClick.AddListener(() => Controller.OnSetEnd());
        btnClear.onClick.AddListener(() => Controller.OnClearGrid());
        btnNavigate.onClick.AddListener(() => Controller.OnNavigate());
    }
}
