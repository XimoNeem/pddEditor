using PDDEditor.UI;
using System;
using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextureSetting_Ticket : Ticket
{
    public TMP_Text NameText;
    public Button Button;

    public PDDModifiable Item;
    public int TextureSettingID;



    public void RequestTexture()
    {
        Action<string, string> action = OnTextureSelected;
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.TexturePicker, action);
    }

    public void OnTextureSelected(string directory, string textureName)
    {
        Debug.Log(directory);
        Debug.Log(textureName);

        Debug.Log(TextureSettingID);
        Debug.Log(Item);

        Item.TextureSettings[TextureSettingID].Value.TextureDirectory = directory;
        Item.TextureSettings[TextureSettingID].Value.TextureName = textureName;

        Item.TextureSettings[TextureSettingID].Set();
    }
}