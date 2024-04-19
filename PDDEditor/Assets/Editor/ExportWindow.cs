using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using System.IO;
using UnityEditor;
using UnityEngine;

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

        _exportPrefab = EditorGUILayout.ObjectField("Prefab", _exportPrefab, typeof(GameObject), true) as GameObject;

        GUILayout.Label("Path");
        EditorGUILayout.BeginHorizontal();
        _exportPath = EditorGUILayout.TextField(_exportPath);
        if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
        {
            _exportPath = EditorUtility.SaveFolderPanel("Path to Save AssetBundle", _exportPath, Application.dataPath);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("File Name");
        _exportFileName = EditorGUILayout.TextField(_exportFileName);

        EditorGUILayout.Space(10);

        GUILayout.Label("Export Settings", EditorStyles.boldLabel);

        if (GUILayout.Button("Export", GUILayout.MinHeight(50)))
        {
            if (_exportPrefab != null && !string.IsNullOrEmpty(_exportPath) && !string.IsNullOrEmpty(_exportFileName))
            {
                AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_exportPrefab));
                importer.SetAssetBundleNameAndVariant(_exportFileName, "");

                string fullExportPath = Path.Combine(_exportPath, _exportFileName);

                // ”казываем полный путь к файлу AssetBundle
  

                // Ёкспортируем AssetBundle только дл€ выбранного префаба
                BuildPipeline.BuildAssetBundles(Path.GetDirectoryName(fullExportPath), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

                string manifestPath = fullExportPath + ".manifest";
                if (File.Exists(manifestPath))
                {
                    File.Delete(manifestPath);
                }

                string[] folders = _exportPath.Split('/');
                string folderName = folders[folders.Length - 1];
                string dirPath = Path.Combine(_exportPath, folderName);
                if (File.Exists(dirPath))
                {
                    File.Delete(dirPath);
                    File.Delete(dirPath + ".manifest");
                }

                importer.SetAssetBundleNameAndVariant("", "");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.RemoveUnusedAssetBundleNames();
            }
            else
            {
                Debug.LogError("Prefab, export path, and export file name must be set!");
            }
        }
    }
}
