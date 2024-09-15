using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hierarchy_Window : WindowController
{
    public Button _objectTemplate;
    public Transform _content;

    private Dictionary<string, Button> _items = new Dictionary<string, Button>();

    public override void OnEnable()
    {
        base.OnEnable();

        Context.Instance.EventManager.OnNodeRemove.AddListener(RemoveItem);
        Context.Instance.EventManager.OnNodeCreated.AddListener(AddItem);
        _objectTemplate.gameObject.SetActive(false);
    }

    public void AddItem(PDDNode item)
    {
        Debug.Log(item);
        Button newItem = Instantiate(_objectTemplate, _content);
        newItem.gameObject.SetActive(true);
        newItem.GetComponentInChildren<TMP_Text>().text = item.name;

        _items.Add(item.NodeID, newItem);

        newItem.onClick.AddListener(delegate { Context.Instance.EventManager.OnNodeSelected.Invoke(item);  } );
    }

    public void RemoveItem(PDDNode item)
    {
        try
        {
            Destroy(_items[item.NodeID].gameObject);
        }
        catch (System.Exception)
        {

            throw;
        }
    }
}