using PDDEditor.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UtilityPicker_Window : WindowController
{
    [SerializeField] private TMP_InputField _searchInput;
    [SerializeField] private Transform _contentParent;

    [SerializeField] private Button _fileTicketTemplate;


    private Action<UtilityNode> _callback;
    private UtilityType _type;

    public override void OnEnable()
    {
        base.OnEnable();

        _searchInput.onValueChanged.AddListener(OnSearch);
    }

    public override void Initialize(params object[] values)
    {
        /*base.Initialize(values);*/

        base.Initialize(values);

        if (values.Length < 2 || values[0].GetType() != typeof(Action<UtilityNode>))
        {
            Debug.LogError("Initialization requires at least 2 parameters: Action<UtilityNode> and UtilityType.");
            return;
        }

        _callback = (Action<UtilityNode>)values[0];

        if (values[1].GetType() != typeof(UtilityType))
        {
            return;
        }


        _type = (UtilityType)values[1];
        Debug.Log($"UtilityType set to {_type}");

        //awddddddddddddd

        if (values[0].GetType() != typeof(Action<UtilityNode>)) { Debug.LogError("Need object of type <Action<UtilityNode>> for initialization"); return; }

        _callback = (Action<UtilityNode>)values[0];

        _type = (UtilityType)values[1];

        RefreshUtilsList();
    }

    private void Start()
    {

    }

    private void OnSearch(string value)
    {
        RefreshUtilsList();
    }

    private void RefreshUtilsList()
    {
        ClearList();
        List<UtilityNode> showList = new List<UtilityNode>();

        foreach (UtilityNode item in Context.Instance.LevelSystem.GetAllUtils())
        {
            if(item.Item.GetType() != typeof(PDDUtility)) { continue;}
            PDDUtility util = (PDDUtility)item.Item;
            if (_type == util.Type)
            {
                Debug.Log(_type);
                if (_searchInput.text != "")
                {
                    Debug.Log(_searchInput.text);
                    if (item.NodeName.ToLower().StartsWith(_searchInput.text.ToLower()))
                    {
                        showList.Add(item);
                    }
                }
                else
                {
                    Debug.Log("added");
                    showList.Add(item);
                }
            }
        }

        foreach (UtilityNode item in showList)
        {
            AddUtilTicket(item);
        }
    }

    private void AddUtilTicket(UtilityNode item)
    {
        Button newItem = Instantiate(_fileTicketTemplate, _contentParent);
        newItem.gameObject.SetActive(true);
        newItem.GetComponentInChildren<TMP_Text>().text = item.NodeName;

        newItem.onClick.AddListener(delegate { _callback.Invoke(item); this.AssetContainer.Unload(); });
    }

    private void ClearList()
    {
        for (int i = _contentParent.childCount - 1; i >= 0; i--)
        {
            Transform child = _contentParent.GetChild(i);
            Destroy(child.gameObject);
        }
    }
}