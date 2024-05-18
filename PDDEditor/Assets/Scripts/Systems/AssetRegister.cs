using System.IO;
using UnityEngine;
using PDDEditor.Items;
using PDDEditor.Types;
using System;

public class AssetRegister : MonoBehaviour
{
    private string _dataPath;

    private void Start()
    {
        _dataPath = Application.dataPath + PDDItems.AssetsPath;

        if (!Directory.Exists(_dataPath))
        {
             Directory.CreateDirectory(_dataPath);
        }
        ObjectType[] elements = (ObjectType[])Enum.GetValues(typeof(ObjectType));
        string[] elementStrings = Array.ConvertAll(elements, e => e.ToString());
        foreach (var item in elementStrings)
        {
            string path = Path.Combine(_dataPath, item);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
