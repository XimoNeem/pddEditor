using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Asset_Ticket : Ticket
{
    public TMP_InputField NameText;
    public RawImage PreviewImage;
    public Button DeleteButton;
    public Button EditButton;

    public string AssetPath;
}
