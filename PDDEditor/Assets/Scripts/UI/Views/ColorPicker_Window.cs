using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ColorPicker_Window : WindowController
{
    [SerializeField] private Button _pickButton;
    [SerializeField] private Button[] _colorButtons;

    private Color _color;
    private Action<Color> _callBack;

    public override void Initialize(params object[] values)
    {
        base.Initialize(values);

        if (values[0].GetType() != typeof(Action<Color>)) { Debug.LogError("Need object of type <Action<Color>> for initialization"); return; }

        _callBack = (Action<Color>)values[0];
    }

    public override void OnEnable()
    {
        base.OnEnable();

        _pickButton.onClick.AddListener(PickColor);

        foreach (var button in _colorButtons)
        {
            button.onClick.AddListener(delegate { SelectColor(button.image.color); });
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    private void SelectColor(Color color)
    {
        _color = color;
    }

    private void PickColor()
    {
        if(_callBack != null)
        {
            _callBack.Invoke(_color);
        }
    }
}
