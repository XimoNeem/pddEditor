using PDDEditor.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDDUtility : PDDModifiable
{
/*    public string Name;
    public string AssetPath;*/

    public UtilityType Type;

/*    public ToggleSetting[] ToggleSettings;
    public ColorSetting[] ColorSettings;
    public TransformGroupSetting[] PDDTransformInfo;*/

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