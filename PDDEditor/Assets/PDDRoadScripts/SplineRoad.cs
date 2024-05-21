using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(SplineRoadBase))]

[ExecuteInEditMode()]
public class SplineRoad : MonoBehaviour
{
    private SplineRoadBase m_splineSampler;
    private MeshFilter m_meshFilter;
    private MeshRenderer m_meshRenderer;

    private List<Vector3> m_vertsP1;
    private List<Vector3> m_vertsP2;

    [SerializeField, Min(5)]
    private int resolution = 10;
    [SerializeField, Range(0.01f,1f)]
    private float m_curveStep = 0.1f;
    [SerializeField]
    private float m_width;

    public List<RoadInfo> roadInfos;

    [SerializeField]
    private List<Intersection> intersections;

    public List<Intersection> Intersections => intersections;

    //Draw mesh
    private void Awake()
    {
        m_splineSampler = gameObject.GetComponent<SplineRoadBase>();
        m_meshFilter = gameObject.GetComponent<MeshFilter>();
        m_meshRenderer = gameObject.GetComponent<MeshRenderer>();

        ComponentUtility.MoveComponentUp(this);
        ComponentUtility.MoveComponentUp(this);

        m_width = 1;
    }

    private void Start()
    {
        Rebuild();
    }

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        GetSplineVerts();
    }

    private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3)
    {
        Rebuild();
    }

    public void Rebuild()
    {
        GetSplineVerts();
        BuildMesh();
    }

    private void OnValidate()
    {
        Rebuild();
    }

    private void BuildMesh()
    {
        Mesh m = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int offset = 0;

        int length = m_vertsP2.Count;

        
        float uvOffset = 0f;

        for (int currentSplineIndex = 0; currentSplineIndex < m_splineSampler.NumSplines; currentSplineIndex++)
        {
            // Определяем смещение для текущего сегмента
            int splineOffset = resolution * currentSplineIndex;
            splineOffset += currentSplineIndex;

            // Итерируемся по точкам сегмента
            for (int currentSplinePoint = 1; currentSplinePoint < resolution + 1; currentSplinePoint++)
            {
                int vertoffset = splineOffset + currentSplinePoint;
                Vector3 p1 = m_vertsP1[vertoffset - 1];
                Vector3 p2 = m_vertsP2[vertoffset - 1];
                Vector3 p3 = m_vertsP1[vertoffset];
                Vector3 p4 = m_vertsP2[vertoffset];

                offset = 4 * resolution * currentSplineIndex;
                offset += 4 * (currentSplinePoint - 1);

                int t1 = offset + 0;
                int t2 = offset + 2;
                int t3 = offset + 3;

                int t4 = offset + 3;
                int t5 = offset + 1;
                int t6 = offset + 0;

                verts.AddRange(new List<Vector3> { p1, p2, p3, p4 });
                tris.AddRange(new List<int> { t1, t2, t3, t4, t5, t6 });

                float distance = Vector3.Distance(p1, p3) / 4f;
                float uvDistance = uvOffset + distance;
                uvs.AddRange(new List<Vector2> { new Vector2(uvOffset, 0), new Vector2(uvOffset, 1), new Vector2(uvDistance, 0), new Vector2(uvDistance, 1) });

                uvOffset += distance;
            }
        }

        List<int> trisB = new List<int>();

        int numVerts = verts.Count;
        GetIntersectionVerts(verts, trisB, uvs);

        m.subMeshCount = 2;

        m.SetVertices(verts);

        m.SetTriangles(tris, 0);
        m.SetTriangles(trisB, 1);

        m.SetUVs(0, uvs);


       /*
        
        m.SetUVs(1, uvsB);
*/


        m_meshFilter.mesh = m;

    }

    private void GetSplineVerts()
    {
        m_vertsP1 = new List<Vector3>();
        m_vertsP2 = new List<Vector3>();

        float step = 1f / (float)resolution;
        Vector3 p1;
        Vector3 p2;
        for (int j = 0; j < m_splineSampler.NumSplines; j++)
        {
            for (int i = 0; i < resolution; i++)
            {
                float t = step * i;
                m_splineSampler.SampleSplineWidth(j, t, roadInfos.Count > j ? roadInfos[j].RoadWidth : m_width, out p1, out p2);
                m_vertsP1.Add(p1);
                m_vertsP2.Add(p2);
            }

            m_splineSampler.SampleSplineWidth(j, 1f, roadInfos.Count > j ? roadInfos[j].RoadWidth : m_width, out p1, out p2);
            m_vertsP1.Add(p1);
            m_vertsP2.Add(p2);
        }
    }

    struct IntersectionEdge
    {
        public Vector3 left;
        public Vector3 right;

        public Vector3 Center => (left + right)/2;

        public IntersectionEdge (Vector3 p1, Vector3 p2)
        {
            this.left = p1;
            this.right = p2;
        }
    }

    private void GetIntersectionVerts(List<Vector3> verts, List<int> tris, List<Vector2> uvs)
    {
        int offset = verts.Count;

        //Get intersection verts
        for (int i = 0; i < intersections.Count; i++)
        {
            Intersection intersection = intersections[i];
            int count = 0;
            //List<Vector3> points = new List<Vector3>();
            
            List<IntersectionEdge> junctionEdges = new List<IntersectionEdge>();

            Vector3 center = new Vector3();
            foreach (JunctionInfo junction in intersection.GetJunctions())
            {
                int splineIndex = junction.splineIndex;
                float t = junction.knotIndex == 0 ? 0f : 1f;
                m_splineSampler.SampleSplineWidth(splineIndex, t, roadInfos.Count > splineIndex ? roadInfos[splineIndex].RoadWidth : m_width, out Vector3 p1, out Vector3 p2);
                //If knot index is 0 we're facing away from the junction
                //If we're more than zero we're facing the junction
                if (junction.knotIndex == 0)
                {
                    junctionEdges.Add(new IntersectionEdge(p1, p2));
                }
                else
                {
                    junctionEdges.Add(new IntersectionEdge(p2, p1));

                }
                center += p1;
                center += p2;
                count++;
            }
            center = center / (junctionEdges.Count * 2);
            

            //Sort the junctions based on their direction from the center
            junctionEdges.Sort((x, y) => SortPoints(center, x.Center, y.Center));


            List<Vector3> curvePoints = new List<Vector3>();
            //Add additional points
            Vector3 mid;
            Vector3 c;
            Vector3 b;
            Vector3 a;
            BezierCurve curve;
            for (int j = 1; j <= junctionEdges.Count; j++)
            {
                a = junctionEdges[j - 1].left;
                curvePoints.Add(a);
                b = (j < junctionEdges.Count) ? junctionEdges[j].right : junctionEdges[0].right;
                mid = Vector3.Lerp(a, b, 0.5f);
                Vector3 dir = center - mid;
                mid = mid - dir;
                c = Vector3.Lerp(mid, center, intersection.curves[j-1]);

                curve = new BezierCurve(a, c, b);
                for (float t = 0f; t < 1f; t += m_curveStep)
                {
                    Vector3 pos = CurveUtility.EvaluatePosition(curve, t);
                    curvePoints.Add(pos);
                }

                curvePoints.Add(b);
            }

            curvePoints.Reverse();

            int pointsOffset = verts.Count;

            for (int j = 1; j <= curvePoints.Count; j++)
            {

                Vector3 pointA = curvePoints[j - 1];
                Vector3 pointB;
                if (j == curvePoints.Count)
                {
                    pointB = curvePoints[0];
                }
                else
                {
                    pointB = curvePoints[j];
                }
                
                verts.Add(center);
                verts.Add(pointA);
                verts.Add(pointB);

                tris.Add(pointsOffset + ((j - 1) * 3) + 0);
                tris.Add(pointsOffset + ((j - 1) * 3) + 1);
                tris.Add(pointsOffset + ((j - 1) * 3) + 2);

                uvs.Add(new Vector2(center.z, center.x));
                uvs.Add(new Vector2(pointA.z, pointA.x));
                uvs.Add(new Vector2(pointB.z, pointB.x));

            }
        }
    }

    private int SortPoints(Vector3 center, Vector3 x, Vector3 y)
    {
        Vector3 xDir = x - center;
        Vector3 yDir = y - center;

        float angleA = Vector3.SignedAngle(center.normalized, xDir.normalized, Vector3.up);
        float angleB = Vector3.SignedAngle(center.normalized, yDir.normalized, Vector3.up);

        if (angleA > angleB)
        {
            return -1;
        }
        if (angleA < angleB)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    public void AddIntersection(Intersection intersection)
    {
        if(intersections == null)
        {
            intersections = new List<Intersection>();
        }
        
        intersections.Add(intersection);

        Rebuild();
    }
}

[System.Serializable]
public struct RoadInfo
{
    public float RoadWidth;
}