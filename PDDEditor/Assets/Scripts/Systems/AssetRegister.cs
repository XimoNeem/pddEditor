using System.IO;
using UnityEngine;
using PDDEditor.Paths;
using PDDEditor.Types;
using System;

public class AssetRegister : MonoBehaviour
{
    private string _dataPath;

    private void Start()
    {
        _dataPath = Application.persistentDataPath + PDDEditorPaths.AssetsPath;

        PDDUtilities.CreateDirectoryIfNotExists(_dataPath);
        ObjectType[] elements = (ObjectType[])Enum.GetValues(typeof(ObjectType));
        string[] elementStrings = Array.ConvertAll(elements, e => e.ToString());
        foreach (var item in elementStrings)
        {
            PDDUtilities.CreateDirectoryIfNotExists(Path.Combine(_dataPath, item));
        }

        Debug.Log($"Created paths {_dataPath}");
    }
}
