using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InputItem", menuName = "PDDEditor/Input")]
public class InputAction : ScriptableObject
{
    [System.Serializable] public class ActionEvent : UnityEvent { }
    public ActionEvent action = new ActionEvent();

    public string ButtonPath;
    public KeyCode[] KeyCodes;

    public void ExecuteAction()
    {
        if (action != null)
        {
            action.Invoke();
        }
    }
}
