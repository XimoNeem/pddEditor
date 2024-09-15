using System.Collections;
using UnityEngine;

public class PDDNode : PDDMovable
{
    public string NodeName;
    public string NodeID;
    public PDDModifiable Item;

    public override void OnClick()
    {
        base.OnClick();

        Context.Instance.EventManager.OnNodeSelected.Invoke(this);
        Item.Set();
    }

    public override void OnHover()
    {
        base.OnHover();
        Context.Instance.UIDrawer.ShowHint(NodeName);
    }

    public override void OnHoverEnds()
    {
        base.OnHoverEnds();
        Context.Instance.UIDrawer.HideHint();
    }

    public virtual void DrawIngameGizmo()
    {

    }
}