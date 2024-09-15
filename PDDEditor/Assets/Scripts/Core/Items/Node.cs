using UnityEngine;

public class Node : PDDNode
{
/*    public string NodeName;
    public string NodeID;
    public PDDModifiable Item;
*/
    public override void OnClick()
    {
        base.OnClick();

        /*Context.Instance.EventManager.OnNodeSelected.Invoke(this);
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

    public void ChangeModel(PDDItem item)
    {
        Destroy(Item.gameObject);
        Item = Instantiate(item, this.transform);
    }
}
