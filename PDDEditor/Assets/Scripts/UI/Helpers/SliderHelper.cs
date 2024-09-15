using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(MeshRenderer))]
public class SliderHelper : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Slider _slider;

    private void Start()
    {
        _slider = this.transform.parent.GetComponentInChildren<Slider>();
        _inputField = this.GetComponent<TMP_InputField>();

        _slider.onValueChanged.AddListener(OnSliderChanged);
        _inputField.onValueChanged.AddListener(OnInput);
        _inputField.onEndEdit.AddListener(CorrectInput);

        _inputField.text = _slider.value.ToString();
    }

    private void OnSliderChanged(float value)
    {
        _inputField.text = value.ToString();
    }

    private void OnInput(string text)
    {
        try
        {
            float value = float.Parse(text);
            Mathf.Clamp(value, _slider.minValue, _slider.maxValue);
            _inputField.text = value.ToString();
            _slider.SetValueWithoutNotify(value);
        }
        catch (System.Exception e)
        {
            Context.Instance.Logger.LogError(e.ToString());
            throw;
        }
    }

    private void CorrectInput(string text)
    {
        _inputField.text = _slider.value.ToString();
    }
}
