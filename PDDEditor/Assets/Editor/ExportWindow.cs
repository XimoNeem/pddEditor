using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEditor;
using UnityEngine;
using System.IO;
using static Unity.VisualScripting.Member;

public class ExportWindow : EditorWindow 
{
    private string _exportPath;
    private string _exportFileName;

    private GameObject _exportPrefab;


    [MenuItem("PDDEditor/ExportAsset")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ExportWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        EditorGUILayout.ObjectField(_exportPrefab, typeof(Entity), true);

        GUILayout.Label("Path");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField(_exportPath);
        if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
        {
            _exportPath = EditorUtility.SaveFolderPanel("Path to Save Images", _exportPath, Application.dataPath);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("File Name");
        EditorGUILayout.TextField(_exportFileName);

        EditorGUILayout.Space(10);


        GUILayout.Label("Export Settings", EditorStyles.boldLabel);

        if (GUILayout.Button("Export", GUILayout.MinHeight(50)))
        {
            BuildPipeline.BuildAssetBundles(_exportPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
        }
        if (GUILayout.Button("Get assets names"))
        {
            var names = AssetDatabase.GetAllAssetBundleNames();
            foreach (var name in names)
                Debug.Log("AssetBundle: " + name);
        }
    }
}
