using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[SelectionBase]
[ExecuteInEditMode]
[RequireComponent(typeof(SplineContainer))]
public class SplineMarksGenerator : MonoBehaviour
{
    [SerializeField] private float _spacing = 1.0f;
    [SerializeField] private float _splineOffset = 0;
    [SerializeField] private Vector3 _positionOffset = Vector3.zero;
    [SerializeField] private float _width = 1.0f;
    [SerializeField] private float _uvScale = 1.0f;
    [SerializeField] private float _firstSegmentLength = 5.0f;
    [SerializeField] private float _lastSegmentLength = 5.0f;
    [SerializeField] private Material firstSegmentMaterial;
    [SerializeField] private Material middleSegmentMaterial;
    [SerializeField] private Material lastSegmentMaterial;

    private SplineContainer _spline;
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _spline = GetComponent<SplineContainer>();
        GenerateMeshAlongSpline();
    }

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3)
    {
        GenerateMeshAlongSpline();
    }

    private void GenerateMeshAlongSpline()
    {
        if (_spline == null)
        {
            Debug.LogError("Spline is not set.");
            return;
        }

        float totalLength = _spline.Spline.GetLength();
        float currentDistance = 0.0f;

        List<Vector3> vertices = new List<Vector3>();
        List<int> firstTriangles = new List<int>();
        List<int> middleTriangles = new List<int>();
        List<int> lastTriangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        int totalSegments = Mathf.FloorToInt(totalLength / _spacing);
        int firstSegmentCount = Mathf.CeilToInt(_firstSegmentLength / _spacing);
        int lastSegmentCount = Mathf.CeilToInt(_lastSegmentLength / _spacing);

        for (int i = 0; i < totalSegments; i++)
        {
            _spline.Spline.Evaluate(currentDistance / totalLength, out float3 position, out float3 forward, out float3 up);

            float3 right = Vector3.Cross(forward, up).normalized;
            position += (right * _splineOffset);

            Quaternion rotation = Quaternion.LookRotation(forward, up);

            // Calculate vertices for the current segment
            Vector3 v0 = (Vector3)position - (Vector3)right * _width / 2 + _positionOffset;
            Vector3 v1 = (Vector3)position + (Vector3)right * _width / 2 + _positionOffset;

            // Evaluate next position for quad
            float nextDistance = currentDistance + _spacing;
            if (nextDistance > totalLength)
                nextDistance = totalLength;

            _spline.Spline.Evaluate(nextDistance / totalLength, out float3 nextPosition, out float3 nextForward, out float3 nextUp);
            float3 nextRight = Vector3.Cross(nextForward, nextUp).normalized;
            nextPosition += (nextRight * _splineOffset);

            Vector3 v2 = (Vector3)nextPosition + (Vector3)nextRight * _width / 2 + _positionOffset;
            Vector3 v3 = (Vector3)nextPosition - (Vector3)nextRight * _width / 2 + _positionOffset;

            // Add vertices
            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);

            // Add triangles
            int vertIndex = vertices.Count - 4;
            List<int> targetTriangles;
            if (i < firstSegmentCount)
                targetTriangles = firstTriangles;
            else if (i >= totalSegments - lastSegmentCount)
                targetTriangles = lastTriangles;
            else
                targetTriangles = middleTriangles;

            targetTriangles.Add(vertIndex);
            targetTriangles.Add(vertIndex + 1);
            targetTriangles.Add(vertIndex + 2);

            targetTriangles.Add(vertIndex);
            targetTriangles.Add(vertIndex + 2);
            targetTriangles.Add(vertIndex + 3);

            // Add UVs (assuming you want a simple UV mapping)
            uv.Add(new Vector2(0, currentDistance * _uvScale));
            uv.Add(new Vector2(1, currentDistance * _uvScale));
            uv.Add(new Vector2(1, nextDistance * _uvScale));
            uv.Add(new Vector2(0, nextDistance * _uvScale));

            // Add normals (assuming you want flat shading)
            normals.Add(up);
            normals.Add(up);
            normals.Add(up);
            normals.Add(up);

            // Move forward along the spline
            currentDistance = nextDistance;
        }

        // Create a new mesh and assign it to the MeshFilter or other renderer component
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.subMeshCount = 3;
        mesh.SetTriangles(firstTriangles, 0);
        mesh.SetTriangles(middleTriangles, 1);
        mesh.SetTriangles(lastTriangles, 2);
        mesh.SetUVs(0, uv);
        mesh.SetNormals(normals);

        // Ensure the mesh renderer exists
        if (_meshRenderer == null)
            if (TryGetComponent<MeshRenderer>(out MeshRenderer renderer)) { _meshRenderer = renderer; }
            else { _meshRenderer = this.gameObject.AddComponent<MeshRenderer>(); }

        // Ensure the mesh filter exists
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        // Assign the generated mesh to the mesh filter
        meshFilter.mesh = mesh;

        // Apply materials
        _meshRenderer.materials = new Material[] { firstSegmentMaterial, middleSegmentMaterial, lastSegmentMaterial };
    }

    private void OnValidate()
    {
        GenerateMeshAlongSpline();
    }
}
