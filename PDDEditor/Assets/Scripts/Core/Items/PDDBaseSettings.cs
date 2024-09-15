using System.Collections;
using UnityEngine;

[System.Serializable]
public class PDDBaseSettings
{
    public bool Locked = false;
    public bool Animated = false;
    public bool Movable = false;
    public bool MoveLoop = false;

    public float AnimationSpeed = 1f;
    public float MoveSpeed = 1f;
    public float SplinePosition = 0.5f;

    public string LinkedSpline = "";
}