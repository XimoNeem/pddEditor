using UnityEngine;
using PDDEditor.Types;

public class PDDItem : MonoBehaviour
{
    public string Name;
    [HideInInspector] public string AssetPath;

    public ObjectType Type;

    public ToggleSetting[] ToggleSettings;
    public ColorSetting[] ColorSettings;
    public EmmiterSetting[] EmmiterSettings;

    private void Start()
    {
        Set();
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

        for (int i = 0; i < EmmiterSettings.Length; i++)
        {
            EmmiterSettings[i].ID = i;
            EmmiterSettings[i].Set();
        }
    } 
}