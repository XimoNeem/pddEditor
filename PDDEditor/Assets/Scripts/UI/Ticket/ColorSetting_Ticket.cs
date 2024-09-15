using UnityEngine.UI;
using TMPro;
using UnityEngine;
using PDDEditor.UI;
using System;
using System.Data.Common;

public class ColorSetting_Ticket : Ticket
{
    public TMP_Text NameText;
    public Button ColorButton;

    public PDDModifiable Item;
    public int ColorSettingID;



    public void RequestColor()
    {
        Action<Color> action = OnColorSelected;
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ColorPicker, action);
    }

    public void OnColorSelected(Color color)
    {
        ColorButton.image.color = color;
        Item.ColorSettings[ColorSettingID].SetColor(color);
    }
}
