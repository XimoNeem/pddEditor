using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDDEditor.Types;

[AddComponentMenu("PDDEditor/Vehicle")]
public class Vehicle : Entity
{
    [Space()]
    public string Brand;
    public string Name;

    [Space()]
    [Header("Wheels")]
    public Wheel[] Wheels;

    [Space()]
    [Header("Lights")]
    public Light[] Lights;

    [Space()]
    [Header("asset options")]
    public AssetOptions[] Options;

    public MeshRenderer[] ColorBodies;
}

[System.Serializable]
public class Wheel
{
    public Transform WheelTransform;
    public WheelType Type;
}

[System.Serializable]
public class Light
{
    public Transform LightObject;
    public VehiclelightType Type;
    public LightPosition lightPosition;
}

[System.Serializable]
public class AssetOptions
{
    public string Name;
    public int ID;
    public Transform[] Items;
}