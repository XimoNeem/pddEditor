using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int ID;
    public Transform PivotNode;
    public void Create() { }

    [ContextMenu("PDDEditor/Create object node")]
    private void CreatePivot()
    {
        if (PivotNode != null) { return; }
        GameObject pivot = new GameObject("ObjectNode");
        pivot.transform.parent = this.transform;

        pivot.transform.position = Vector3.zero;

        PivotNode = pivot.transform;
    }

    [ContextMenu("PDDEditor/Create struct")]
    private void CreateStruct()
    {
        GameObject options = new GameObject("ObjectsOptions");
        options.transform.parent = this.transform;
        options.transform.localPosition = Vector3.zero;

        GameObject option = new GameObject("Option (1)");
        option.transform.parent = options.transform;
        option.transform.localPosition = Vector3.zero;

        GameObject lights = new GameObject("Lights");
        GameObject objects = new GameObject("objects");
        GameObject wheels = new GameObject("wheels");

        GameObject[] items = new GameObject[3] { lights, objects, wheels };


        foreach (var item in items)
        {
            item.transform.parent = option.transform;
            item.transform.localPosition = Vector3.zero;
        }
    }
}
