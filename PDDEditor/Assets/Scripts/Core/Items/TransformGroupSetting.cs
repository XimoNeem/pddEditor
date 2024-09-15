using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformGroupSetting : IItemSetting
{
    [SerializeField] private string name;
    public string Name { get { return name; } set { name = value; } }

    public int ID { get; set; }

    public List<PDDTransformInfo> Value;

    public void Set()
    {
        if (Value == null) { Value = new List<PDDTransformInfo>(); }
    }
}

[System.Serializable]
public class PDDTransformInfo
{
    public Vector3 Position;
    public Vector3 LeftHandle;
    public Vector3 RightHandle;

    public PDDTransformInfo(Vector3 position, Vector3 leftHandle, Vector3 rightHandle)
    {
        Position = position;
        LeftHandle = leftHandle;
        RightHandle = rightHandle;
    }
}