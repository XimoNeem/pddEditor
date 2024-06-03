using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode()]
public class SplineRoadBase : MonoBehaviour
{
    [HideInInspector] public SplineContainer SplineContainer;


    public SplineContainer Container => SplineContainer;
    public int NumSplines => SplineContainer.Splines.Count;

    float3 m_position;
    float3 m_foward;
    float3 m_upVector;

    Vector3 p1;
    Vector3 p2;

    public void SampleSplineWidth(int splineIndex, float t, float width, out Vector3 p1, out Vector3 p2)
    {
        SplineContainer.Evaluate(splineIndex, t, out float3 position, out float3 forward, out float3 upVector);

        float3 right = Vector3.Cross(forward, upVector).normalized;
        p1 = position + (right * width);
        p2 = position + (-right * width);
    }

    public void SampleSplineBorder(int splineIndex, float t, float width, out Vector3 point, out Vector3 rightDirection)
    {
        SplineContainer.Evaluate(splineIndex, t, out float3 position, out float3 frwrd, out float3 upVector);

        float3 right = Vector3.Cross(frwrd, upVector).normalized;
        point = position + (right * width);
        rightDirection = right;
    }

    public void FixRotation()
    {
        foreach (Spline item in SplineContainer.Splines)
        {
            for (int i = 0; i < item.Count; i++)
            {
                BezierKnot knot = item[i];

                knot.Rotation = Quaternion.identity;
                item.SetKnot(i, knot);
            }
        }
    }
}
