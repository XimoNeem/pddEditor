using System.Collections;
using UnityEngine;

public class PDDSplineHandleController : PDDMovable
{
    public PDDSplineHandle Parent;

    private LineRenderer _lineRenderer;

    public void Initialize(PDDSplineHandle parent, Vector3 position)
    {
        Parent = parent;
        _lineRenderer = GetComponentInChildren<LineRenderer>();

        this.transform.position = position;

        SetLine();
    }

    private void Start()
    {
        SetLine();
    }

    public override void OnClick()
    {
        base.OnClick();
    }

    public override void OnMove()
    {

        base.OnMove();

        SetLine();
        Parent.OnMoveHandles();
    }

    public void SetLine()
    {
        if (_lineRenderer != null)
        {
            _lineRenderer.SetPosition(0, this.transform.position);
            _lineRenderer.SetPosition(1, Parent.transform.position);
        }
    }
}