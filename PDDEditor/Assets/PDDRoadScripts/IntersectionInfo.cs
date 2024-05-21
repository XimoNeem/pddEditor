using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public class Intersection
{
    public List<JunctionInfo> junctions;
    public List<float> curves;


    public void AddJunction(int splineIndex, int knotIndex, Spline spline)
    {
        if (junctions == null)
        {
            junctions = new List<JunctionInfo>();
            curves = new List<float>();
        }

        junctions.Add(new JunctionInfo(splineIndex, knotIndex, spline));
        curves.Add(0.3f);

    }
    public Vector3 Center {
        get
        {
            Vector3 center = new Vector3();
            foreach(JunctionInfo junction in junctions)
            {
                center += (Vector3)junction.knot.Position;
            }
            center = center / junctions.Count;
            
            return center;
        }
    }

    internal IEnumerable<JunctionInfo> GetJunctions()
    {
        return junctions;
    }
}

[System.Serializable]
public struct JunctionInfo
{
    public int splineIndex;
    public int knotIndex;
    public Spline spline;
    public BezierKnot knot => spline.Knots.ToArray()[knotIndex];

    public JunctionInfo(int splineIndex, int knotIndex, Spline spline) : this()
    {
        this.splineIndex = splineIndex;
        this.knotIndex = knotIndex;
        this.spline = spline;
    }
}
