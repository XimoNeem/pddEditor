using System.Collections;
using UnityEngine;
using PDDEditor.UI;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Popup_Window : WindowController
{
    [SerializeField] private Transform _elementsContent;

    [SerializeField] private TMP_Text _header;

    [SerializeField] private Button _buttonTemplate;
    [SerializeField] private TMP_InputField _inputTemplate;

    private string _title;
    private System.Action<string> _inputCallback;

    public override void Initialize(params object[] values)
    {
        base.Initialize(values);

        foreach (var item in values)
        {
            if (item.GetType() == typeof(PopupHeader))
            {
                PopupHeader data = (PopupHeader)item;

                if (_header != null) { _header.text = data.Text; }
                _title = data.Text;
            }

            else if (item.GetType() == typeof(PopupButton))
            {
                Button newButton = Instantiate(_buttonTemplate, _elementsContent);
                newButton.gameObject.SetActive(true);
                PopupButton data = (PopupButton)item;

                if (newButton.GetComponentInChildren<TMP_Text>())
                {
                    newButton.GetComponentInChildren<TMP_Text>().text = data.Name;
                }

                newButton.image.color = data.Color;
                newButton.onClick.AddListener( delegate { data.ClickAction.Invoke(); this.AssetContainer.Unload(); });
            }

            else if (item.GetType() == typeof(PopupInput))
            {
                TMP_InputField newInput = Instantiate(_inputTemplate, _elementsContent);
                newInput.gameObject.SetActive(true);
                PopupInput data = (PopupInput)item;

                _inputCallback = data.Action;

                newInput.placeholder.GetComponent<TMP_Text>().text = data.PlaceHolderText;
                newInput.GetComponentInChildren<Button>().onClick.AddListener(
                    delegate
                    {
                        if (newInput.text == "") { Context.Instance.UIDrawer.TintImageByTime(newInput.image, Color.red, Color.white); }
                        else { data.Action.Invoke(newInput.text); this.AssetContainer.Unload(); }
                    }
                );
            }

            else { Debug.LogError("Need object of type <PopupElement> for initialization"); return; }
        }
    }

    private void OnInput(string value)
    {
        _inputCallback.Invoke(value);
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }
}
