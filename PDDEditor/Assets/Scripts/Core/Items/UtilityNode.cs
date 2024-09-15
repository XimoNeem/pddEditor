using System.Collections;
using UnityEngine;

public class UtilityNode : PDDNode
{
/*    public string NodeName;
    public string NodeID;
    public PDDUtility Item;*/

    public override void OnClick()
    {
        base.OnClick();

        /*Context.Instance.EventManager.OnUtilitySelected.Invoke(this);
        Item.Set();*/
    }

    public override void OnHover()
    {
        base.OnHover();
        //Context.Instance.UIDrawer.ShowHint(NodeName);
    }

    public override void OnHoverEnds()
    {
        base.OnHoverEnds();
        //Context.Instance.UIDrawer.HideHint();
    }

    public override void DrawIngameGizmo()
    {

    }
}