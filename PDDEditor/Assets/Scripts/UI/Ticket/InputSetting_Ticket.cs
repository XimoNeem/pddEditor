using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class InputSetting_Ticket : Ticket
{
    public Button ApplyButton;

    public TMP_Text NameText;

    public Button KeyInputTemplate;
    public Transform Content;
    public Button AddInputButton;
    public Button RemoveInputButton;

    private List<KeyCode> Keys = new List<KeyCode>();
    private InputAction CurrentAction;

    private int CurrentIndex;
    public bool ListenForInput = false;

    public void Init(InputAction action)
    {
        CurrentAction = action;

        for (int i = 0; i < CurrentAction.KeyCodes.Length; i++)
        {
            Keys.Add(CurrentAction.KeyCodes[i]);

            Button inputKey = Instantiate(KeyInputTemplate, Content);
            inputKey.gameObject.SetActive(true);
            inputKey.GetComponentInChildren<TMP_Text>().text = CurrentAction.KeyCodes[i].ToString();
            int keyId = i;
            inputKey.onClick.AddListener(delegate { SetCurrentEditKey(keyId); });
            NameText.text = CurrentAction.ButtonPath;
        }
    }

    private void Start()
    {
        AddInputButton.onClick.AddListener(AddInputKey);
        RemoveInputButton.onClick.AddListener(RemoveInputKey);
        ApplyButton?.onClick.AddListener(ApplyChanges);
    }

    private void SetCurrentEditKey(int value)
    {
        CurrentIndex = value;
        ListenForInput = true;

        Button button = Content.GetChild(value).GetComponent<Button>();
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        buttonText.text = "";
    }

    private void AddInputKey()
    {
        Button inputKey = Instantiate(KeyInputTemplate, Content);
        inputKey.gameObject.SetActive(true);
        inputKey.GetComponentInChildren<TMP_Text>().text = "";
        int keyId = Keys.Count;
        inputKey.onClick.AddListener(delegate { SetCurrentEditKey(keyId); });
        Keys.Add(KeyCode.None); // Добавляем пустой элемент в список
        ApplyChanges();
    }

    private void RemoveInputKey()
    {
        if (Keys.Count < 2)
        {
            return;
        }

        Destroy(Content.transform.GetChild(Content.transform.childCount - 1).gameObject);
        Keys.RemoveAt(Keys.Count - 1);
        ApplyChanges();
    }

    private void Update()
    {
        if (ListenForInput)
        {
            // Проверяем нажатие любой клавиши
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    Keys[CurrentIndex] = keyCode;
                    ListenForInput = false;
                    UpdateKeyDisplay(CurrentIndex, keyCode); // Обновление отображения на UI
                    break;
                }
            }
        }
    }

    private void UpdateKeyDisplay(int index, KeyCode keyCode)
    {
        // Обновление текста кнопки для отображения новой клавиши
        Button button = Content.GetChild(index).GetComponent<Button>();
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = keyCode.ToString();
        }
        ApplyChanges();
    }

    private void ApplyChanges()
    {
        CurrentAction.KeyCodes = Keys.ToArray(); // Обновляем скриптабл объект с новыми значениями
        Debug.Log("Key bindings updated.");
    }
}
