using System.Collections;
using UnityEngine;

public class PDDSplineHandle : PDDMovable
{
    public PDDSpline ParentSpline;
    public int Index;

    public PDDSplineHandleController LeftHandle;
    public PDDSplineHandleController RightHandle;

    public void Initialize(PDDSpline parentSpline, int index, Vector3 leftPosition, Vector3 rightPosition)
    {
        ParentSpline = parentSpline;
        Index = index;

        PDDSplineHandleController template = Resources.Load<PDDSplineHandleController>("SplineHandleController");

        LeftHandle = Instantiate(template);
        RightHandle = Instantiate(template);

        LeftHandle.transform.position = leftPosition;
        RightHandle.transform.position = rightPosition;

        LeftHandle.transform.parent = this.transform;
        RightHandle.transform.parent = this.transform;

        Debug.DrawLine(leftPosition, leftPosition + Vector3.up, Color.blue, 5);
        Debug.DrawLine(rightPosition, rightPosition + Vector3.up, Color.blue, 5);

        LeftHandle.Initialize(this, leftPosition);
        RightHandle.Initialize(this, rightPosition);
    }

    public void OnMoveHandles()
    {
        ParentSpline.MoveHandle(Index, LeftHandle.transform.position, RightHandle.transform.position);
    }

    public override void OnMove()
    {
        base.OnMove();

        ParentSpline.MovePoint(Index, this.transform.position);
        ParentSpline.MoveHandle(Index, LeftHandle.transform.position, RightHandle.transform.position);

        LeftHandle.SetLine();
        RightHandle.SetLine();
    }

    public override void OnMoveEnds()
    {
        base.OnMoveEnds();
    }

    public override void OnClick()
    {
        base.OnClick();
    }

    public override void OnHover()
    {
        base.OnHover();
    }

    public override void OnHoverEnds()
    {
        base.OnHoverEnds();
    }
}