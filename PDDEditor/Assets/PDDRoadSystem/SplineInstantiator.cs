using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

[SelectionBase]
[ExecuteInEditMode]
[RequireComponent(typeof(SplineContainer))]
public class SplineMeshGenerator : MonoBehaviour
{
    [SerializeField] [Min(0.05f)] private float spacing = 1.0f;
    [SerializeField] private Vector3 _rotationOffset = Vector3.zero;
    [SerializeField] private Vector3 _positionOffset = Vector3.zero;
    [SerializeField] [Range(-100, 100)] private float _splineOffset = 0;
    [SerializeField] private bool _mirror = false;
    [SerializeField] private bool _connectEdges = false;
    [SerializeField] private bool _reverseNormals = false;
    [SerializeField] private SplineElement[] prefabs;

    private SplineContainer spline;
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        spline = GetComponent<SplineContainer>();
        GenerateMeshAlongSpline();

        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
    }

    private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3)
    {
        GenerateMeshAlongSpline();
    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    [ContextMenu("Generate Mesh Along Spline")]
    private void GenerateMeshAlongSpline()
    {
        if (spline == null || prefabs.Length == 0)
        {
            Debug.LogError("Spline or prefabs array is not set.");
            return;
        }

        float totalLength = spline.Spline.GetLength();
        float currentDistance = 0.0f;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        Vector3[] previousRightEdge = null;

        while (currentDistance < totalLength)
        {
            spline.Spline.Evaluate(currentDistance / totalLength, out float3 position, out float3 forward, out float3 up);

            float3 right = Vector3.Cross(forward, up).normalized;
            position += (right * _splineOffset);

            Quaternion rotation = Quaternion.LookRotation(forward, up) * Quaternion.Euler(_rotationOffset);

            foreach (var prefab in prefabs)
            {
                Mesh mesh = prefab.Prefab.GetComponent<MeshFilter>().sharedMesh;
                Vector3[] prefabVertices = mesh.vertices;
                int[] prefabTriangles = mesh.triangles;
                Vector2[] prefabUVs = mesh.uv;
                Vector3[] prefabNormals = mesh.normals;

                List<Vector3> worldVertices = new List<Vector3>(new Vector3[prefabVertices.Length]);
                Vector3[] worldNormals = new Vector3[prefabVertices.Length];
                for (int i = 0; i < prefabVertices.Length; i++)
                {
                    Vector3 vertex = prefabVertices[i];
                    if (_mirror)
                    {
                        worldVertices[i] = (Vector3)position + _positionOffset + rotation * vertex;
                        worldVertices[i] = new Vector3((position.x + _positionOffset.x) - (rotation * vertex).x, worldVertices[i].y, worldVertices[i].z);
                    }
                    else
                    {
                        worldVertices[i] = (Vector3)position + _positionOffset + rotation * vertex;
                    }
                    worldNormals[i] = rotation * prefabNormals[i];


                    // Reverse normals if the option is enabled
                    if (_reverseNormals)
                    {
                        worldNormals[i] = -worldNormals[i];
                    }
                }

                (Vector3[] leftEdge, Vector3[] rightEdge) = SplitEdges(worldVertices, prefabTriangles, rotation, prefabUVs);

                if (previousRightEdge != null && _connectEdges)
                {
                    worldVertices = AlignEdges(previousRightEdge, leftEdge, worldVertices, triangles, uv, prefabTriangles);
                }

                for (int i = 0; i < worldVertices.Count; i++)
                {
                    if (prefab.Remap) { prefabUVs[i] = new Vector2(worldVertices[i].x, worldVertices[i].z); }
                }

                int vertexOffset = vertices.Count;
                vertices.AddRange(worldVertices);
                normals.AddRange(worldNormals);

                for (int i = 0; i < prefabTriangles.Length; i++)
                {
                    int triangle = prefabTriangles[i] + vertexOffset;
                    triangles.Add(triangle);
                }

                uv.AddRange(prefabUVs);

                previousRightEdge = rightEdge;

                currentDistance += prefab.Length;
            }

            currentDistance += spacing;
        }

        Mesh combinedMesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uv.ToArray(),
            normals = normals.ToArray()
        };

        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();

        if (_meshRenderer != null)
        {
            _meshRenderer.GetComponent<MeshFilter>().mesh = combinedMesh;
            _meshRenderer.GetComponent<MeshRenderer>().material = prefabs[0].Prefab.GetComponent<MeshRenderer>().sharedMaterial;
        }
        else
        {
            GameObject meshObject = new GameObject("CombinedMesh");

            meshObject.AddComponent<MeshFilter>().mesh = combinedMesh;
            meshObject.AddComponent<MeshRenderer>().material = prefabs[0].Prefab.GetComponent<MeshRenderer>().sharedMaterial;
            meshObject.transform.parent = transform;
            meshObject.transform.localPosition = Vector3.zero;

            _meshRenderer = meshObject.GetComponent<MeshRenderer>();
        }
    }

    private (Vector3[] leftEdge, Vector3[] rightEdge) SplitEdges(List<Vector3> vertices, int[] triangles, Quaternion rotation, Vector2[] uvs, bool sortByX = false)
    {
        Vector3 center = CalculateCenter(vertices, triangles);
        List<(Vector3 vertex, Vector2 uv)> leftEdge = new List<(Vector3, Vector2)>();
        List<(Vector3 vertex, Vector2 uv)> rightEdge = new List<(Vector3, Vector2)>();

        foreach (var vertex in vertices)
        {
            Vector3 localVertex = Quaternion.Inverse(rotation) * (vertex - center);

            if (sortByX)
            {
                if (IsToTheRightOfCenter(Vector3.right, localVertex))
                {
                    rightEdge.Add((vertex, uvs[vertices.IndexOf(vertex)]));
                }
                else
                {
                    leftEdge.Add((vertex, uvs[vertices.IndexOf(vertex)]));
                }
            }
            else
            {
                if (IsToTheRightOfCenter(Vector3.forward, localVertex))
                {
                    rightEdge.Add((vertex, uvs[vertices.IndexOf(vertex)]));
                }
                else
                {
                    leftEdge.Add((vertex, uvs[vertices.IndexOf(vertex)]));
                }
            }
        }

        leftEdge.Sort((a, b) => a.uv.y.CompareTo(b.uv.y));
        rightEdge.Sort((a, b) => a.uv.y.CompareTo(b.uv.y));


        return (leftEdge.Select(e => e.vertex).ToArray(), rightEdge.Select(e => e.vertex).ToArray());
    }

    private bool IsToTheRightOfCenter(Vector3 axis, Vector3 vertex)
    {
        return Vector3.Dot(axis, vertex) > 0;
    }

    private Vector3 CalculateCenter(List<Vector3> vertices, int[] triangles)
    {
        Vector3 sum = Vector3.zero;
        foreach (var vertex in vertices)
        {
            sum += vertex;
        }

        Vector3 result = sum / vertices.Count;

        Debug.DrawLine(result, result + Vector3.up * 0.1f, Color.green, 0.1f);
        return result;
    }

    private List<Vector3> AlignEdges(Vector3[] previousRightEdge, Vector3[] currentRightEdge, List<Vector3> vertices, List<int> triangles, List<Vector2> uv, int[] prefabTriangles)
    {
        for (int i = 0; i < currentRightEdge.Length; i++)
        {
            if (vertices.Contains(currentRightEdge[i]) && (previousRightEdge.Length - 1) >= i)
            {
                vertices[vertices.IndexOf(currentRightEdge[i])] = previousRightEdge[i];

            }
        }

        return vertices;
    }

    private void OnValidate()
    {
        GenerateMeshAlongSpline();
    }
}

[System.Serializable]
public struct SplineElement
{
    public GameObject Prefab;
    [Min(0.05f)] public float Length;
    public bool SplitOrientation;
    public bool Remap;
}
