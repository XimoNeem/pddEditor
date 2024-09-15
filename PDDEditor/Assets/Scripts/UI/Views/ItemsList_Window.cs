using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using PDDEditor.Types;
using System.Collections.Generic;
using Unity.VisualScripting;
using PDDEditor.Assets;
using System.Linq;

public class ItemsList_Window : WindowController
{
    [SerializeField] private TMP_Dropdown _typeDropdown;
    [SerializeField] private TMP_InputField _searchInput;
    [SerializeField] private Transform _contentParent;

    [SerializeField] private ObjectItem_Ticket _ticket;

    private Action<string> _cachedAction;

    public override void OnEnable()
    {
        base.OnEnable();
        
        Context.Instance.UIDrawer.InitTypesDropdown(_typeDropdown);

        _searchInput.onValueChanged.AddListener(delegate { RefreshList(_typeDropdown.value); });
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

        if (values[0].GetType() != typeof(Action<string>)) { Debug.LogError("Need object of type <Action<string>> for initialization"); return; }

        _cachedAction = (Action<string>)values[0];

    }

    private void RefreshList(int value)
    {
        //ObjectType type = (ObjectType)Enum.Parse(typeof(ObjectType), value.ToString());
        string type = _typeDropdown.captionText.text;
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

    private IEnumerator AddItems(string type)
    {
        List<PDDAssetData> items = Context.Instance.AssetRegister.GetAssets(type);

        if (!string.IsNullOrEmpty(_searchInput.text))
        {
            items = items.Where(item => item.Name.ToLower().Contains(_searchInput.text.ToLower())).ToList();
        }

        foreach (PDDAssetData item in items)
        {
            ObjectItem_Ticket ticket = Instantiate(_ticket, _contentParent);
            ticket.Name = item.Name;
            ticket.AssetPath = item.Path;
            ticket.OnSelectedAction = _cachedAction;
            ticket.PreviewImage.texture = PDDUtilities.LoadTextureFromFile(item.ImagePath);
            ticket.Initialize();
            ticket.CreateButton.onClick.AddListener(this.AssetContainer.Unload);
            yield return new WaitForEndOfFrame();
        }
    }
}
