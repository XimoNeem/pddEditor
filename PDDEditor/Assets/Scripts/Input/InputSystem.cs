using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public InputAction[] InputActions;
    private bool _inputBlocked = false;

    public bool InputBlocked { get => _inputBlocked; set => _inputBlocked = value; }

    private PDDNode _selectedNode;
    private PDDNode _clipboardNode;

    public LayerMask SelectableLayers;
    private PDDMovable _hoveredObject;

    private void OnEnable()
    {
        Context.Instance.EventManager.OnNodeSelected.AddListener(SetSelectedNode);
        SelectableLayers = LayerMask.GetMask("Node");
    }

    private void Update()
    {
        HandleSelection();   // ��������� ��������� ��������
        HandleHover();

        if (InputActions == null) return;

        foreach (var inputAction in InputActions)
        {
            if (inputAction.KeyCodes.Length == 0)
            {
                continue;
            }

            bool shouldExecute = false;

            if (inputAction.KeyCodes.Length == 1)
            {
                // ��� ����� ������� �������� �������� ���������� ��� �������
                if (Input.GetKeyDown(inputAction.KeyCodes[0]))
                {
                    shouldExecute = true;
                }
            }
            else if (inputAction.KeyCodes.Length == 2)
            {
                // ���������, ��� ������ ������� ������, � ������ ������ ��� ������
                if (Input.GetKey(inputAction.KeyCodes[0]) && Input.GetKeyDown(inputAction.KeyCodes[1]))
                {
                    shouldExecute = true;
                }
            }
            else if (inputAction.KeyCodes.Length == 3)
            {
                // ���������, ��� ������ ��� ������� ������, � ������ ������ ��� ������
                if (Input.GetKey(inputAction.KeyCodes[0]) && Input.GetKey(inputAction.KeyCodes[1]) && Input.GetKeyDown(inputAction.KeyCodes[2]))
                {
                    shouldExecute = true;
                }
            }
            // �������� ����������� ������ ��� �������� ���������� ������, ���� �����������

            if (shouldExecute)
            {
                inputAction.ExecuteAction();
            }
        }
    }

    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, SelectableLayers))
            {
                PDDMovable movable = hit.collider.GetComponent<PDDMovable>();
                if (movable != null)
                {
                    movable.OnClick();
                }
            }
        }
    }

    private void HandleHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, SelectableLayers))
        {
            PDDMovable movable = hit.collider.GetComponent<PDDMovable>();
            if (movable != null)
            {
                if (_hoveredObject != movable)
                {
                    EndHover(); // ��������� ��������� �� ���������� ������
                    StartHover(movable); // �������� ��������� �� ����� ������
                }
            }
            else
            {
                EndHover(); // ��������� ���������, ���� ��� ���������� PDDMovable
            }
        }
        else
        {
            EndHover(); // ��������� ���������, ���� ������� ������ �� �����
        }
    }

    private void StartHover(PDDMovable movable)
    {
        _hoveredObject = movable;

        _hoveredObject.OnHover();
    }

    private void EndHover()
    {
        if (_hoveredObject != null)
        {
            _hoveredObject.OnHoverEnds();
        }
    }


    public void SetSelectedNode(PDDNode node)
    {
        _selectedNode = node;
    }

    public void CopyNode()
    {
        if (_selectedNode != null)
        {
            _clipboardNode = _selectedNode;
        }
    }

    public void PasteNode()
    {
        if (_clipboardNode != null)
        {
            Context.Instance.LevelSystem.CreateObject(_clipboardNode.Item);
        }
    }

    public void DublicateNode()
    {
        if (_selectedNode != null)
        {
            Context.Instance.LevelSystem.DublicateObject(_selectedNode);
        }
    }

    public void DeleteNode()
    {
        if (_selectedNode != null)
        {
            Context.Instance.LevelSystem.DeleteObject(_selectedNode);
        }
    }
}
