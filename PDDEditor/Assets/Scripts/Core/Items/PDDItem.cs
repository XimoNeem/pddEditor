using UnityEngine;
using PDDEditor.Types;

public class PDDItem : MonoBehaviour
{
    public string Name;

    public ObjectType Type;
    public ToggleSetting[] ToggleSettings;
    public ColorSetting[] ColorSettings;

    private void Start()
    {
        for (int i = 0; i < ToggleSettings.Length; i++)
        {
            ToggleSettings[i].ID = i;
            ToggleSettings[i].Set();
        }

        for (int i = 0; i < ColorSettings.Length; i++)
        {
            ColorSettings[i].ID = i;
            ColorSettings[i].Set();
        }
    }

    public void Set()
    {
        for (int i = 0; i < ToggleSettings.Length; i++)
        {
            ToggleSettings[i].ID = i;
            ToggleSettings[i].Set();
        }

        for (int i = 0; i < ColorSettings.Length; i++)
        {
            ColorSettings[i].ID = i;
            ColorSettings[i].Set();
        }
    } 
}