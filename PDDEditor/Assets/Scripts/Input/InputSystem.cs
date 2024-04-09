using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    private bool _inputBlocked = false;

    public bool InputBlocked { get => _inputBlocked; set => _inputBlocked = value; }

    private void Update()
    {
        /*foreach (var item in InputItems)
        {
            bool active = true;
            foreach (var key in item.Keys) 
            {
                if (!Input.GetKey(key)) { active = false; }
                if (active) { Debug.Log(item.Name); }
            }
        }*/
    }
}

public class InputItem : ScriptableObject
{ 
    public string Name;
}
