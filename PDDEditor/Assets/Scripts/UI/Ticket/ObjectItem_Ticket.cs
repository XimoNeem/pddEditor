using System;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class ObjectItem_Ticket : Ticket
{
    public string AssetPath;
    public string Name;
    public string Description;
    public Texture2D Preview;
    public Action<string> OnSelectedAction;

    public RawImage PreviewImage;
    public Button CreateButton;
    public TMP_Text NameText;

    public override void Initialize()
    {
        NameText.text = Name;
    }

    private void OnEnable()
    {
        CreateButton.onClick.AddListener(OnSelect);
    }

    private void OnDisable()
    {
        CreateButton.onClick.RemoveAllListeners();
    }

    private void OnSelect()
    {
        if (OnSelectedAction != null)
        {
            OnSelectedAction.Invoke(AssetPath);
        }
    }
}
