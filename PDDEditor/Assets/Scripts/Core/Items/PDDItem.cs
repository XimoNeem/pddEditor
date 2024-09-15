using UnityEngine;
using PDDEditor.Types;

public class PDDItem : PDDModifiable
{
/*    public string Name;
    [HideInInspector] public string AssetPath;*/

    public ObjectType Type;

    /*public PDDBaseSettings PDDBaseSettings;*/

/*    public ToggleSetting[] ToggleSettings;
    public ColorSetting[] ColorSettings;
    public EmmiterSetting[] EmmiterSettings;*/


    private void Start()
    {
        Set();
    }

    public override void Set()
    {
        base.Set();
    }

    public override void ApplyBaseSettings()
    {
        base.ApplyBaseSettings();
    }
}