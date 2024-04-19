using PDDEditor.Types;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct ToggleSetting : IItemSetting
{
    [SerializeField] private string name;
    public string Name { get { return name; } set { name = value; } }

    public int ID { get; set; }

    public GameObject[] ToggledObjects;
    public bool Value;

    public void Set()
    {
        foreach (var item in ToggledObjects)
        {
            item.SetActive(Value);
        }
    }
}