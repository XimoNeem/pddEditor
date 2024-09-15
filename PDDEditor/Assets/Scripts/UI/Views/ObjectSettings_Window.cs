using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using PDDEditor.UI;
using PDDEditor.Types;

public class ObjectSettings_Window : WindowController
{
    [SerializeField] private Transform _content;
    [SerializeField] private TMP_InputField _objectName;
    [SerializeField] private TMP_InputField _objectPositionX, _objectPositionY, _objectPositionZ;
    [SerializeField] private TMP_InputField _objectRotationX, _objectRotationY, _objectRotationZ;
    [SerializeField] private TMP_InputField _objectScaleX, _objectScaleY, _objectScaleZ;
    [SerializeField] private TMP_Text _modelName;
    [SerializeField] private Button _changeModelButton;
    [SerializeField] private Button _rescaleButton;


    [Space]
    [SerializeField] private Toggle _lockedToggle;
    [SerializeField] private Toggle _animatedToggle;
    [SerializeField] private Toggle _movableToggle;
    [SerializeField] private Toggle _moveLoopToggle;
    [SerializeField] private TMP_InputField _animationSpeedInput;
    [SerializeField] private TMP_InputField _moveSpeedInput;
    [SerializeField] private Slider _splinePositionSlider;
    [SerializeField] private Button _linkedSplineButton;
    [Space]

    [SerializeField] private ToggleSetting_Ticket _toggleTemplate;
    [SerializeField] private ColorSetting_Ticket _colorTemplate;
    [SerializeField] private EmmiterSetting_Ticket _emmiterTemplate;
    [SerializeField] private TextureSetting_Ticket _textureTemplate;
    [SerializeField] private TextSetting_Ticket _textTemplate;

    private List<Ticket> createdTickets;

    private PDDNode _currentNode;

    private GameObject[] _viewItems;

    private void Awake()
    {
        createdTickets = new List<Ticket>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Context.Instance.EventManager.OnNodeSettings.AddListener(InitSettings);
        _changeModelButton.onClick.AddListener(RequestChangeModel);

        _lockedToggle.onValueChanged.AddListener(SetlockedToggle);
        _animatedToggle.onValueChanged.AddListener(SetAnimatedToggle);
        _movableToggle.onValueChanged.AddListener(SetMovableToggle);
        _moveLoopToggle.onValueChanged.AddListener(SetMoveLoopToggle);
        _animationSpeedInput.onEndEdit.AddListener(SetAnimationSpeedInput);
        _moveSpeedInput.onEndEdit.AddListener(SetMoveSpeedInput);
        _linkedSplineButton.onClick.AddListener(SetLinkedSplineButton);
        _rescaleButton.onClick.AddListener(Rescale);
        _splinePositionSlider.onValueChanged.AddListener(SetSplinePosition);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Context.Instance.EventManager.OnNodeSettings.RemoveListener(InitSettings);
    }

    public override void Initialize(params object[] values)
    {
        base.Initialize(values);

        if (values[0].GetType() == typeof(PDDNode) | values[0].GetType() == typeof(Node) | values[0].GetType() == typeof(UtilityNode))
        {

            InitSettings((PDDNode)values[0]);
        }
        else
        { 
            Context.Instance.Logger.LogError("Need object of type <PDDModifiable> for initialization"); 
            this.AssetContainer.Unload(); return; 
        }

    }

    public void InitSettings(PDDNode node)
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

        _lockedToggle.SetIsOnWithoutNotify(node.Item.PDDBaseSettings.Locked);
        _animatedToggle.SetIsOnWithoutNotify(node.Item.PDDBaseSettings.Animated);
        _movableToggle.SetIsOnWithoutNotify(node.Item.PDDBaseSettings.Movable);
        _moveLoopToggle.SetIsOnWithoutNotify(node.Item.PDDBaseSettings.MoveLoop);
        _animationSpeedInput.SetTextWithoutNotify(node.Item.PDDBaseSettings.AnimationSpeed.ToString());
        _moveSpeedInput.SetTextWithoutNotify(node.Item.PDDBaseSettings.AnimationSpeed.ToString());
        _linkedSplineButton.GetComponentInChildren<TMP_Text>().text = node.Item.PDDBaseSettings.LinkedSpline.ToString();
        _splinePositionSlider.SetValueWithoutNotify(node.Item.PDDBaseSettings.SplinePosition);

        CreateViewItem();
    }

    private void SetlockedToggle(bool value)
    {
        _currentNode.Item.PDDBaseSettings.Locked = value;
        _currentNode.Item.ApplyBaseSettings();
    }
    private void SetAnimatedToggle(bool value)
    {
        _currentNode.Item.PDDBaseSettings.Animated = value;
        _currentNode.Item.ApplyBaseSettings();
    }
    private void SetMovableToggle(bool value)
    {
        _currentNode.Item.PDDBaseSettings.Movable = value;
        _currentNode.Item.ApplyBaseSettings();
    }
    private void SetMoveLoopToggle(bool value)
    {
        _currentNode.Item.PDDBaseSettings.MoveLoop = value;
        _currentNode.Item.ApplyBaseSettings();
    }
    private void SetAnimationSpeedInput(string value)
    {
        _currentNode.Item.PDDBaseSettings.AnimationSpeed = float.Parse(value);
        _currentNode.Item.ApplyBaseSettings();
    }
    private void SetMoveSpeedInput(string value)
    {
        _currentNode.Item.PDDBaseSettings.MoveSpeed = float.Parse(value);
        _currentNode.Item.ApplyBaseSettings();
    }
    private void SetLinkedSplineButton()
    {
        Action<UtilityNode> action = OnSplineSelected;
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.UtilityPicker, action, UtilityType.Spline);
    }

    private void SetSplinePosition(float value)
    {
        _currentNode.Item.PDDBaseSettings.SplinePosition = value;
        _currentNode.Item.ApplyBaseSettings();
    }

    private void OnSplineSelected(UtilityNode spline)
    {
        _linkedSplineButton.GetComponentInChildren<TMP_Text>().text = spline.NodeName;
        _currentNode.Item.PDDBaseSettings.LinkedSpline = spline.NodeID;
        _currentNode.Item.ApplyBaseSettings();
    }


    private void Rescale()
    {
        _currentNode.transform.localScale = Vector3.one;
    }

    private void RequestChangeModel()
    {
        Action<PDDItem> action = ChangeModel;
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ItemsList, action);
        CreateViewItem();
    }

    private void ChangeModel(PDDItem item)
    {
        try
        {
            Node node = (Node)_currentNode;
            node.ChangeModel(item);
            Initialize(_currentNode);
        }
        catch (Exception)
        {

            throw;
        }
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

        for (int i = 0; i < _currentNode.Item.TextureSettings.Length; i++)
        {
            TextureSetting_Ticket ticket = Instantiate(_textureTemplate, _content);
            ticket.NameText.text = _currentNode.Item.TextureSettings[i].Name;
            int id = _currentNode.Item.TextureSettings[i].ID;
            ticket.Item = _currentNode.Item;
            ticket.TextureSettingID = id;


            ticket.Button.onClick.AddListener(delegate { ticket.RequestTexture(); });

            createdTickets.Add(ticket);
        }

        for (int i = 0; i < _currentNode.Item.TextSettings.Length; i++)
        {
            TextSetting_Ticket ticket = Instantiate(_textTemplate, _content);
            ticket.NameText.text = _currentNode.Item.TextSettings[i].Name;
            ticket.TextInput.SetTextWithoutNotify(_currentNode.Item.TextSettings[i].Value);
            int id = _currentNode.Item.TextSettings[i].ID;


            ticket.TextInput.onValueChanged.AddListener(delegate {
                _currentNode.Item.TextSettings[id].Value = ticket.TextInput.text;
                _currentNode.Item.Set();
            });

            createdTickets.Add(ticket);
        }
    }
}
