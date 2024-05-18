using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using PDDEditor.UI;
using Unity.VisualScripting;

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
    [SerializeField] private ColorSetting_Ticket _colorTemplate;
    [SerializeField] private EmmiterSetting_Ticket _emmiterTemplate;

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
            Destroy(item.gameObject);
        }

        createdTickets.Clear();
    }

    private void CreateViewItem()
    {
        ClearSettings();

        for (int i = 0; i < _currentNode.Item.ToggleSettings.Length; i++)
        {
            ToggleSetting_Ticket ticket = Instantiate(_toggleTemplate, _content);
            ticket.NameText.text = _currentNode.Item.ToggleSettings[i].Name;
            ticket.Toggle.isOn = _currentNode.Item.ToggleSettings[i].Value;
            int id = _currentNode.Item.ToggleSettings[i].ID;

            ticket.Toggle.onValueChanged.AddListener(delegate
            {
                _currentNode.Item.ToggleSettings[id].Value = ticket.Toggle.isOn;
                _currentNode.Item.ToggleSettings[id].Set();
            });

            createdTickets.Add(ticket);
        }

        for (int i = 0; i < _currentNode.Item.ColorSettings.Length; i++)
        {
            ColorSetting_Ticket ticket = Instantiate(_colorTemplate, _content);
            ticket.NameText.text = _currentNode.Item.ColorSettings[i].Name;
            ticket.ColorButton.image.color = _currentNode.Item.ColorSettings[i].Value;
            int id = _currentNode.Item.ColorSettings[i].ID;
            ticket.Item = _currentNode.Item;
            ticket.ColorSettingID = id;


            ticket.ColorButton.onClick.AddListener(delegate
                    {
                        ticket.RequestColor();
                    }
                );

            createdTickets.Add(ticket);
        }

        for (int i = 0; i < _currentNode.Item.EmmiterSettings.Length; i++)
        {
            EmmiterSetting_Ticket ticket = Instantiate(_emmiterTemplate, _content);
            ticket.NameText.text = _currentNode.Item.EmmiterSettings[i].Name;

            for (int x = 0; x < _currentNode.Item.EmmiterSettings[i].EmmiterObjects.Length; x++)
            {
                GameObject newTicket = Instantiate(ticket.ItemTemplate, ticket.ItemsParent);
                newTicket.SetActive(true);
                TMP_Text text = newTicket.GetComponentInChildren<TMP_Text>();
                Toggle toggle = newTicket.GetComponentInChildren<Toggle>();

                text.text = _currentNode.Item.EmmiterSettings[i].EmmiterObjects[x].Name;
                toggle.isOn = _currentNode.Item.EmmiterSettings[i].EmmiterObjects[x].Value;

                int id = _currentNode.Item.EmmiterSettings[i].ID;
                int objectId = x;

                toggle.onValueChanged.AddListener(delegate {
                    _currentNode.Item.EmmiterSettings[id].EmmiterObjects[objectId].Value = toggle.isOn;
                    _currentNode.Item.Set();
                });
            }

            createdTickets.Add(ticket);
        }
    }
}
