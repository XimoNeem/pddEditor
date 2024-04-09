using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuDrop : TMP_Dropdown
{
    private string _name = "Item";

    protected override GameObject CreateDropdownList(GameObject template)
    {
        GameObject result = base.CreateDropdownList(template);
        result.name = "sgdhtfgjhgffdsdafsgdhj";
        return result;
    }
    protected override DropdownItem CreateItem(DropdownItem itemTemplate)
    {
        DropdownItem item = base.CreateItem(itemTemplate);
        item.GetComponent<RectTransform>().sizeDelta = new Vector2 (0, 80);
        item.name = "sefgdrhftgjhkl";

        return item;
    }
    protected override void Start()
    {
        base.Start();
        captionText.text = _name;
    }
    public override void Select()
    {
        base.Select();
        captionText.text = _name;
    }
}
