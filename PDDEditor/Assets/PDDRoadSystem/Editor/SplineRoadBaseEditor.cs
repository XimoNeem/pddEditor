using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;

[CustomEditor(typeof(SplineRoadBase))]
public class SplineRoadBaseEditor : Editor
{
    private Texture2D logoTexture;

    private void OnEnable()
    {
        logoTexture = EditorGUIUtility.Load("Assets/PDDRoadSystem/Editor/logo.png") as Texture2D;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SplineRoadBase road = (SplineRoadBase)target;

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        // Отображаем изображение с логотипом
        if (logoTexture != null)
        {
            GUILayout.Label(logoTexture, GUILayout.Width(100), GUILayout.Height(100));
        }

        // Отображаем текст рядом с логотипом
        GUILayout.Label("PDDEditor\nRoad system", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("Инициализировать"))
        {
            road.SplineContainer = road.GetComponent<SplineContainer>();
            road.gameObject.AddComponent<SplineRoad>();
        }

        if (GUILayout.Button("Поправить вращение"))
        {
            road.FixRotation();
        }
    }
}
