using System.Collections;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TextSetting : IItemSetting
{

    [SerializeField] private string name;
    public string Name { get { return name; } set { name = value; } }
    public int ID { get; set; }

    public TMP_Text[] TextRenderers;
    public string Value;

    public void Set()
    {
        foreach (var renderer in TextRenderers)
        {
            renderer.text = Value;
        }
    }
}