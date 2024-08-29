using PDDEditor.Assets;
using PDDEditor.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssetsManager_Window : WindowController
{
    [SerializeField] private Transform _itemsContent;
    [SerializeField] private TMP_Dropdown _typeDropdown;

    [SerializeField] private Asset_Ticket _itemTemplate;

    public override void OnEnable()
    {
        base.OnEnable();

        Context.Instance.UIDrawer.InitTypesDropdown(_typeDropdown);
        _typeDropdown.onValueChanged.AddListener(LoadAssets);

        LoadAssets(0);
    }

    private void LoadAssets(int value)
    {
        ClearAssetsList();
        PDDAssetData[] data = Context.Instance.AssetRegister.GetAssets(_typeDropdown.captionText.text);

        foreach (var asset in data)
        {
            Debug.Log(asset.Path);
            Asset_Ticket ticket = Instantiate(_itemTemplate, _itemsContent);

            ticket.NameText.text = asset.Name;
            ticket.AssetPath = asset.Path;
            Context.Instance.UIDrawer.SetRawImageFromPath(asset.ImagePath, ticket.PreviewImage);

            ticket.DeleteButton.onClick.AddListener(delegate { OnDeleteAsset(asset.Path); });
        }
    }

    private void ClearAssetsList()
    {
        for (int i = 0; i < _itemsContent.transform.childCount; i++)
        {
            Destroy(_itemsContent.GetChild(i).gameObject);
        }
    }


    private void OnDeleteAsset(string path)
    {
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.PopupWindow, new PopupHeader("Удалить этот объект?"), new PopupButton("Да", Color.Lerp(Color.white, Color.red, 0.5f), delegate { Context.Instance.AssetRegister.DeleteAsset(path); LoadAssets(0); }), new PopupButton("Нет", Color.Lerp(Color.white, Color.black, 0.1f), delegate { }));
    }
}
