using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Rendering;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using PDDEditor.UI;

public class ObjectSettings_Window : WindowController
{
    [SerializeField] private Transform _content;
    [SerializeField] private TMP_InputField _objectName;
    [SerializeField] private TMP_InputField _objectPositionX, _objectPositionY, _objectPositionZ;
    [SerializeField] private TMP_InputField _objectRotationX, _objectRotationY, _objectRotationZ;
    [SerializeField] private TMP_InputField _objectScaleX, _objectScaleY, _objectScaleZ;
    [SerializeField] private TMP_Text _modelName;
    [SerializeField] private Button _changeModelButton;

    [SerializeField] private ToggleSetting_Ticket _toggleTemplate;

    private List<Ticket> createdTickets;

    private Node _currentNode;

    private GameObject[] _viewItems;

    private void Awake()
    {
        createdTickets = new List<Ticket>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Context.Instance.EventManager.OnNodeSettings.AddListener(Initialize);
        _changeModelButton.onClick.AddListener(RequestChangeModel);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Context.Instance.EventManager.OnNodeSettings.RemoveListener(Initialize);
    }

    public override void Initialize(params object[] values)
    {
        base.Initialize(values);

        if (values[0].GetType() != typeof(Node)) { Debug.LogError("Need object of type <Node> for initialization"); return; }

        Debug.Log("Init with params");
        Initialize((Node)values[0]);

    }

    private void Initialize(Node node)
    {
        _currentNode = node;

        _objectName.text = node.name;
        _objectPositionX.text = node.transform.position.x.ToString();
        _objectPositionY.text = node.transform.position.y.ToString();
        _objectPositionZ.text = node.transform.position.z.ToString();

        _objectRotationX.text = node.transform.rotation.x.ToString();
        _objectRotationY.text = node.transform.rotation.y.ToString();
        _objectRotationZ.text = node.transform.rotation.z.ToString();

        _objectScaleX.text = node.transform.localScale.x.ToString();
        _objectScaleY.text = node.transform.localScale.y.ToString();
        _objectScaleZ.text = node.transform.localScale.z.ToString();

        CreateViewItem();
    }

    private void RequestChangeModel()
    {
        Action<PDDItem> action = ChangeModel;
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ItemsList, action);
        CreateViewItem();
    }

    private void ChangeModel(PDDItem item)
    {
        _currentNode.ChangeModel(item);
        Initialize(_currentNode);
    }

    private void ClearSettings()
    {
        foreach (var item in createdTickets)
        {
            Debug.Log(item);
            Destroy(item.gameObject);
        }

        createdTickets.Clear();
    }

    private void CreateViewItem()
    {
        ClearSettings();

        for (int i = 0; i < _currentNode.Item.Settings.Length; i++)
        {
            ToggleSetting_Ticket ticket = Instantiate(_toggleTemplate, _content);
            ticket.NameText.text = _currentNode.Item.Settings[i].Name;
            ticket.Toggle.isOn = _currentNode.Item.Settings[i].Value;
            int id = _currentNode.Item.Settings[i].ID;

            ticket.Toggle.onValueChanged.AddListener(delegate
            {
                _currentNode.Item.Settings[id].Value = ticket.Toggle.isOn;
                _currentNode.Item.Settings[id].Set();
            });

            createdTickets.Add(ticket);
        }
    }
}
