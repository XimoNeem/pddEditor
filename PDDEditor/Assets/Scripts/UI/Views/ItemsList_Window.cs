using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using PDDEditor.Types;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Lumin;

public class ItemsList_Window : WindowController
{
    [SerializeField] private TMP_Dropdown _typeDropdown;
    [SerializeField] private Transform _contentParent;

    [SerializeField] private ObjectItem_Ticket _ticket;

    private Action<PDDItem> _cachedAction;

    public override void OnEnable()
    {
        base.OnEnable();
        SetTypesDropdown();

        _typeDropdown.onValueChanged.AddListener(RefreshList);
        Context.Instance.EventManager.OnItemSelected.AddListener(delegate { AssetContainer.Unload(); });
    }

    public override void OnDisable()
    {
        base.OnDisable();
        _typeDropdown.onValueChanged.RemoveAllListeners();
    }

    public override void Initialize(params object[] values)
    {
        base.Initialize(values);

        if (values[0].GetType() != typeof(Action<PDDItem>)) { Debug.LogError("Need object of type <Action<PDDItem>> for initialization"); return; }

        _cachedAction = (Action<PDDItem>)values[0];

    }

    private void SetTypesDropdown()
    {
        ObjectType[] elements = (ObjectType[])Enum.GetValues(typeof(ObjectType));
        string[] elementStrings = Array.ConvertAll(elements, e => e.ToString());

        _typeDropdown.ClearOptions();
        _typeDropdown.AddOptions(new List<string>(elementStrings));
    }

    private void RefreshList(int value)
    {
        ObjectType type = (ObjectType)Enum.Parse(typeof(ObjectType), value.ToString());
        StopAllCoroutines();
        ClearList();
        StartCoroutine(AddItems(type));
    }

    private void ClearList()
    {
        for (int i = _contentParent.childCount - 1; i >= 0; i--)
        {
            Transform child = _contentParent.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    private IEnumerator AddItems(ObjectType type)
    {
        yield return new WaitForEndOfFrame();

        PDDItem[] items = Resources.LoadAll<PDDItem>("");

        foreach (var item in items)
        {
            if (item.Type == type)
            {
                ObjectItem_Ticket ticket = Instantiate(_ticket, _contentParent);
                ticket.Name = item.name;
                ticket.Item = item;
                ticket.OnSelectedAction = _cachedAction;
                ticket.Initialize();
            }
        }
    }
}
