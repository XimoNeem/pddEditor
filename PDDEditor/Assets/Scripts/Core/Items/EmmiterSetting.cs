using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EmmiterSetting : IItemSetting
{
    [SerializeField] private string name;
    public string Name { get { return name; } set { name = value; } }
    public int ID { get; set; }

    public EmmiterObject[] EmmiterObjects;
    public MeshRenderer[] EmmiterRenderer;

    public void Set()
    {
        foreach (var item in EmmiterObjects)
        {
            foreach (var renderer in EmmiterRenderer)
            {
                renderer.material.SetInt($"_{item.Color.ToString()}", Convert.ToInt32(item.Value));
            }
        }
    }

    public void SetColor(int index, bool enabled)
    {
        EmmiterObjects[index].Value = enabled;
        Set();
    }
}

[System.Serializable]
public struct EmmiterObject
{
    public string Name;
    public EmmiterColors Color;
    public bool Value;
}
public enum EmmiterColors
{
    R,
    G, 
    B,
    A
}
