using System;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class ObjectItem_Ticket : Ticket
{
    public PDDItem Item;
    public string Name;
    public string Description;
    public Texture2D Preview;
    public Action<PDDItem> OnSelectedAction;

    [SerializeField] private Image _preview;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _name;

    public override void Initialize()
    {
        _name.text = Name;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnSelect);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }

    private void OnSelect()
    {
        if (OnSelectedAction != null)
        {
            OnSelectedAction.Invoke(Item);
        }
    }
}
