using UnityEngine;

[System.Serializable]
public struct ColorSetting: IItemSetting
{
    [SerializeField] private string name;
    public string Name { get { return name; } set { name = value; } }

    public int ID { get; set; }

    public Color Value;

    public MeshRenderer[] ToggledObjects;
    

    public void Set()
    {
        foreach (var item in ToggledObjects)
        {
            foreach (var material in item.materials)
            {
                material.color = Value;
            }
        }
    }

    public void SetColor(Color color)
    {
        Value = color;
        Set();
    }
}