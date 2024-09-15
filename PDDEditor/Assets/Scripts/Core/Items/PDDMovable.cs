using System.Collections;
using UnityEngine;

public class PDDMovable : MonoBehaviour
{
    public virtual void OnMove()
    {
    }

    public virtual void OnMoveEnds()
    {
    }

    public virtual void OnClick()
    {
        FindObjectOfType<RuntimeHandle.RuntimeTransformHandle>().SetTarget(this);
    }

    public virtual void OnHover()
    {
        
    }

    public virtual void OnHoverEnds()
    {

    }

/*    private void OnMouseDown()
    {
        OnClick();
    }

    private void OnMouseEnter()
    {
        OnHover();
    }

    private void OnMouseExit()
    {
        OnHoverEnds();
    }*/
}