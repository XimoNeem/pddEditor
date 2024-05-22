using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineRoad))]
public class SplineRoadEditor : Editor
{

    private void OnSceneGUI()
    {
        SplineRoad road = (SplineRoad)target;
        for (int i = 0; i < road.Intersections.Count; i++)
        {
            if (Handles.Button(road.Intersections[i].Center, Quaternion.Euler(90f, 0, 0), 0.3f, 0.5f, Handles.CircleHandleCap))
            {
                JunctionBuilderOverlay.instance.ShowIntersection(road.Intersections[i]);
                JunctionBuilderOverlay.instance.OnChangeValueEvent = road.Rebuild;
            }
        }
    }
}
