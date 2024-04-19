using PDDEditor.UI;
using System;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public void CreateObject(PDDItem item)
    {
        Node node = Instantiate(Resources.Load<Node>("Node"));

        node.gameObject.name = item.name;

        node.NodeName = item.name;
        node.NodeID = 0000000;
        node.Item = Instantiate(item, node.transform);

        FindObjectOfType<RuntimeHandle.RuntimeTransformHandle>().SetTarget(node.gameObject);
    }

    public void RequestObject()
    {
        Action<PDDItem> action = CreateObject;
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ItemsList, action);
    }

    private void OnEnable()
    {
        Context.Instance.EventManager.OnItemSelected.AddListener(CreateObject);
    }

    private void OnDisable()
    {
        
    }
}