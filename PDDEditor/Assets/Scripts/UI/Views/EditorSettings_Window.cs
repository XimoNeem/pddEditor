using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class EditorSettings_Window : WindowController
{
    [SerializeField] private Slider _sensivitySlider;
    [SerializeField] private Slider _speedSlider;


    public override void OnEnable()
    {
        base.OnEnable();

        _sensivitySlider.onValueChanged.AddListener(SetSensitity);
        _speedSlider.onValueChanged.AddListener(SetSpeed);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        _sensivitySlider.onValueChanged.RemoveListener(SetSensitity);
        _speedSlider.onValueChanged.RemoveListener(SetSpeed);
    }

    private void SetSensitity(float value)
    {
        FindObjectOfType<FreeFlyCamera>()._mouseSense = value;
    }

    private void SetSpeed(float value)
    {
        FindObjectOfType<FreeFlyCamera>().SetMoveSpeed(value);
    }
}
