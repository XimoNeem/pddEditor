using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PDDSpline : UtilityNode
{
    private LineRenderer _renderer;
    private List<Vector3> positions = new List<Vector3>();
    private List<Vector3> leftHandles = new List<Vector3>();
    private List<Vector3> rightHandles = new List<Vector3>();
    private List<Vector3> interpolatedPositions = new List<Vector3>();

    private List<PDDSplineHandle> handles = new List<PDDSplineHandle>();

    private void Start()
    {
        _renderer = GetComponent<LineRenderer>();
        UpdateSplineData();

        InitSpline();

        InterpolateSpline(); // Интерполируем точки для получения гладкого сплайна
        DrawSpline();
    }

    public override void DrawIngameGizmo()
    {
        base.DrawIngameGizmo();
    }

    private void InitSpline()
    {
        PDDSplineHandle template = Resources.Load<PDDSplineHandle>("SplineHandle");

        for (int i = 0; i < positions.Count; i++)
        {
            PDDSplineHandle newHandle = Instantiate(template);

            newHandle.transform.position = positions[i];
            newHandle.Initialize(this, i, leftHandles[i], rightHandles[i]);
            handles.Add(newHandle);
        }
    }

    private void UpdateSettings()
    {
        for (int i = 0; i < Item.PDDTransformInfo[0].Value.Count; i++)
        {
            Item.PDDTransformInfo[0].Value[i].Position = positions[i];
            Item.PDDTransformInfo[0].Value[i].LeftHandle = leftHandles[i];
            Item.PDDTransformInfo[0].Value[i].RightHandle = rightHandles[i];
        }
    }

    public override void OnMove()
    {
        base.OnMove();

        InterpolateSpline();
        DrawSpline();

        UpdateSettings();
    }

    public void MovePoint(int index, Vector3 position)
    {
        positions[index] = position;
        InterpolateSpline();
        DrawSpline();

        UpdateSettings();
    }

    public void MoveHandle(int index, Vector3 leftHandle, Vector3 rightHandle)
    {
        leftHandles[index] = leftHandle;
        rightHandles[index] = rightHandle;
        InterpolateSpline();
        DrawSpline();

        UpdateSettings();
    }

    public void AddNewPoint()
    {
        (Vector3 position, Vector3 direction) = EvaluateSpline(0.95f);

        position += direction * 5;

        PDDSplineHandle newHandle = Instantiate(Resources.Load<PDDSplineHandle>("SplineHandle"));
        newHandle.Initialize(this, positions.Count, direction * -1, direction);

        newHandle.transform.position = position;

        positions.Add(position);
        leftHandles.Add(direction * -1);
        rightHandles.Add(direction);

        Item.PDDTransformInfo[0].Value.Add(new PDDTransformInfo(position, direction * -1, direction));

        InterpolateSpline();
        DrawSpline();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Plus)) { AddNewPoint(); }
    }

    private void UpdateSplineData()
    {
        List<PDDTransformInfo> data = Item.PDDTransformInfo[0].Value;

        positions.Clear();
        leftHandles.Clear();
        rightHandles.Clear();

        foreach (PDDTransformInfo info in data)
        {
            positions.Add(info.Position);
            leftHandles.Add(info.LeftHandle);
            rightHandles.Add(info.RightHandle);
        }
    }

    private void InterpolateSpline()
    {
        interpolatedPositions.Clear();

        int resolution = 10; // Количество точек между двумя узлами для интерполяции

        for (int i = 0; i < positions.Count - 1; i++)
        {
            Vector3 p0 = positions[i];
            Vector3 p1 = rightHandles[i];
            Vector3 p2 = leftHandles[i + 1];
            Vector3 p3 = positions[i + 1];

            /*Vector3 p0 = positions[i] - handles[i].transform.position;
            Vector3 p1 = positions[i] + rightHandles[i] - handles[i].transform.position * handleMultiplier;
            Vector3 p2 = positions[i + 1] + leftHandles[i + 1] - handles[i].transform.position * handleMultiplier;
            Vector3 p3 = positions[i + 1] - handles[i].transform.position;*/

            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)resolution;
                interpolatedPositions.Add(CubicBezier(p0, p1, p2, p3, t));
            }
        }

        interpolatedPositions.Add(positions[positions.Count - 1]);
    }

    public void DrawSpline()
    {
        _renderer.positionCount = interpolatedPositions.Count;

        _renderer.startColor = Item.ColorSettings[0].Value;
        _renderer.endColor = Item.ColorSettings[0].Value;
        _renderer.material.color = Item.ColorSettings[0].Value;

        _renderer.SetPositions(interpolatedPositions.ToArray());
    }

    public (Vector3 position, Vector3 direction) EvaluateSpline(float t)
    {
        // Если t меньше 0 или больше 1, то мы ограничиваем значение в пределах [0, 1]
        t = Mathf.Clamp01(t);

        // Определяем количество сегментов (один сегмент - это расстояние между двумя узлами)
        int segmentCount = positions.Count - 1;

        // Определяем на каком сегменте находится параметр t
        // Например, если t = 0.5 и у нас 10 сегментов, то мы будем на 5-м сегменте
        int segmentIndex = Mathf.FloorToInt(t * segmentCount);

        // Сколько t приходится на один сегмент
        float segmentT = 1f / segmentCount;

        // Определяем локальное значение t для сегмента
        float localT = (t - segmentIndex * segmentT) / segmentT;

        // Получаем точки для кубической кривой Безье этого сегмента
        Vector3 p0 = positions[segmentIndex];
        Vector3 p1 = rightHandles[segmentIndex];
        Vector3 p2 = leftHandles[segmentIndex + 1];
        Vector3 p3 = positions[segmentIndex + 1];

        // Вычисляем позицию на сплайне
        Vector3 position = CubicBezier(p0, p1, p2, p3, localT);

        // Для направления используем производную кубической кривой Безье
        Vector3 direction = CubicBezierDerivative(p0, p1, p2, p3, localT).normalized;

        return (position, direction);
    }


    private Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0;
        point += 3 * uu * t * p1;
        point += 3 * u * tt * p2;
        point += ttt * p3;

        return point;
    }

    private Vector3 CubicBezierDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = 3 * uu * (p1 - p0);
        point += 6 * u * t * (p2 - p1);
        point += 3 * tt * (p3 - p2);

        return point;
    }
}
