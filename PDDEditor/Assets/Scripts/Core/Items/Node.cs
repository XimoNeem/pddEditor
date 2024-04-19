using UnityEngine;

public class Node : MonoBehaviour
{
    public string NodeName;
    public int NodeID;
    public PDDItem Item;

    private void OnMouseDown()
    {
        FindObjectOfType<RuntimeHandle.RuntimeTransformHandle>().SetTarget(this.gameObject);

        Context.Instance.EventManager.OnNodeSelected.Invoke(this);
        Item.Set();
    }

    private void OnMouseEnter()
    {
        Context.Instance.UIDrawer.ShowHint(NodeName);
    }

    private void OnMouseExit()
    {
        Context.Instance.UIDrawer.HideHint();
    }

    public void ChangeModel(PDDItem item)
    {
        Destroy(Item.gameObject);
        Item = Instantiate(item, this.transform);
    }
}
