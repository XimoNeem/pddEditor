using UnityEngine;
using PDDEditor.Types;

public class PDDItem : MonoBehaviour
{
    public string Name;

    public ObjectType Type;
    public ToggleSetting[] Settings;

    private void Start()
    {
        for (int i = 0; i < Settings.Length; i++)
        {
            Settings[i].ID = i;
            Settings[i].Set();
        }
    }
}