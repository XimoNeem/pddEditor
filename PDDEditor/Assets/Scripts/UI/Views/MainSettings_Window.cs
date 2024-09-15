using System.Collections;
using UnityEngine;

public class MainSettings_Window : WindowController
{
    [SerializeField] private InputSetting_Ticket _settingTemplate;
    [SerializeField] private Transform _content;

    [SerializeField] private InputAction[] _data;

    private void Start()
    {
        foreach (var item in _data)
        {
            InputSetting_Ticket newTicket = Instantiate(_settingTemplate, _content);
            newTicket.gameObject.SetActive(true);

            newTicket.Init(item);
        }
    }
}