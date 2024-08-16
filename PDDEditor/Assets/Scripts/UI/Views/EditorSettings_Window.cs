using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class EditorSettings_Window : WindowController
{
    [SerializeField] private Slider _sensivitySlider;
    [SerializeField] private Slider _speedSlider;


    public override void OnEnable()
    {
        base.OnEnable();

        _sensivitySlider.onValueChanged.AddListener(SetSensitity);
        _speedSlider.onValueChanged.AddListener(SetSpeed);

        SetSensitity(Context.Instance.EditorBase.EditorSettings.MouseSensivity);
        _sensivitySlider.SetValueWithoutNotify(Context.Instance.EditorBase.EditorSettings.MouseSensivity);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        _sensivitySlider.onValueChanged.RemoveListener(SetSensitity);
        _speedSlider.onValueChanged.RemoveListener(SetSpeed);
    }

    private void SetSensitity(float value)
    {
        Context.Instance.EditorBase.EditorSettings.MouseSensivity = value;
        Context.Instance.EditorBase.Save();
    }

    private void SetSpeed(float value)
    {
        FindObjectOfType<FreeFlyCamera>().SetMoveSpeed(value);
    }
}
