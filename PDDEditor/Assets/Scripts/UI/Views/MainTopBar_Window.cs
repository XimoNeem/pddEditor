using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

public class MainTopBar_Window : WindowController
{
    [SerializeField] private Button _sectionTemplate;
    [SerializeField] private Button _buttonTemplate;

    [SerializeField] private Transform _buttonParent;
    [SerializeField] private InputAction[] _data;

    private Dictionary<string, Transform> _sectionParents = new Dictionary<string, Transform>();

    void Start()
    {
        foreach (InputAction objData in _data)
        {
            CreateButton(objData);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) { return; }
            foreach (var item in _sectionParents.Values)
            {
                HideList(item);
            }
        }
    }

    void CreateButton(InputAction objData)
    {
        string[] pathParts = objData.ButtonPath.Split('/');

        string sectionName = pathParts[0];
        string buttonName = pathParts[1];

        if (!_sectionParents.ContainsKey(sectionName))
        {
            Button section = Instantiate(_sectionTemplate, _buttonParent);
            section.gameObject.SetActive(true);
            _sectionParents.Add(sectionName, section.transform);
            section.GetComponentInChildren<TMP_Text>().text = sectionName;

            section.onClick.AddListener( delegate { ShowList(section.transform); } );
        }

        Button newButton = Instantiate(_buttonTemplate, _sectionParents[sectionName]);
        newButton.GetComponentInChildren<TMP_Text>().text = buttonName;

        Button buttonComponent = newButton.GetComponent<Button>();
        buttonComponent.onClick.AddListener(() => objData.ExecuteAction());
        buttonComponent.onClick.AddListener(delegate { HideList(buttonComponent.transform.parent); });

        newButton.name = objData.ButtonPath;
    }

    private void HideList(Transform section)
    {
        for (int i = 0; i < section.transform.childCount; i++)
        {
            if (!section.transform.GetChild(i).GetComponent<TMP_Text>())
            {
                section.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void ShowList(Transform section)
    {
        foreach (var item in _sectionParents.Values)
        {
            HideList(item);
        }
        for (int i = 0; i < section.childCount; i++)
        {
            if (!section.GetChild(i).GetComponent<TMP_Text>())
            {
                section.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}