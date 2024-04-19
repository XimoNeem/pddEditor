using UnityEngine;
using PDDEditor.UI;
using System.Diagnostics.Tracing;

[System.Serializable]
public class Test : MonoBehaviour
{
    public ColorSetting color;

    private void FixedUpdate()
    {
        color.Value = Color.red;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.G)) { color.Value = Color.blue; }
    }
}
