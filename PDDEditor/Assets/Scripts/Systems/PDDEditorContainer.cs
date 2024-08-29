using System;
using System.Collections;
using System.IO;
using UnityEngine;
using PDDEditor.Types;
using PDDEditor.Paths;

public class PDDEditorContainer
{
    public EditorSettings EditorSettings;
    public UsersObjectTypes EditorTypes;
    /*public EditorInput EditorInput;*/

    public PDDEditorContainer()
    {
        string settingsPath = Path.Combine(Application.persistentDataPath, PDDEditor.Paths.PDDEditorPaths.EditorSettings);
        string userTypesPath = Path.Combine(Application.persistentDataPath, PDDEditor.Paths.PDDEditorPaths.EditorTypes);

        if (PDDUtilities.TryReadFromFile(settingsPath, out string settingJson))
        {
            EditorSettings = JsonUtility.FromJson<EditorSettings>(settingJson);
            Context.Instance.Logger.Log("EditorSettings was serialized");
        }
        else { EditorSettings = new EditorSettings(); Context.Instance.Logger.Log("new EditorSettings created"); }

        if (PDDUtilities.TryReadFromFile(userTypesPath, out string typesJson))
        {
            EditorTypes = JsonUtility.FromJson<UsersObjectTypes>(typesJson);
            Context.Instance.Logger.Log("UsersObjectTypes was serialized");
        }
        else { EditorTypes = new UsersObjectTypes(); Context.Instance.Logger.Log("new UsersObjectTypes created"); }
    }

    public void Save()
    {
        string settingsJson = JsonUtility.ToJson(EditorSettings, true);
        string settingsPath = Path.Combine(Application.persistentDataPath, PDDEditor.Paths.PDDEditorPaths.EditorSettings);
        PDDUtilities.WriteTextToFile(settingsPath, settingsJson);

        string typesJson = JsonUtility.ToJson(EditorTypes, true);
        string typesPath = Path.Combine(Application.persistentDataPath, PDDEditor.Paths.PDDEditorPaths.EditorTypes);
        PDDUtilities.WriteTextToFile(typesPath, typesJson);
    }
}

[System.Serializable]
public class EditorSettings 
{
    public float MouseSensivity = 1f;
}

[System.Serializable]
public class EditorInput
{
    
}

[System.Serializable]
public class UsersObjectTypes
{
    public string[] Types;

    public string[] GetTypes()
    {
        return Types;
    }

    public void CreateNewType(string type)
    {
        if (Array.Exists(Types, t => t.Equals(type)))
        {
            Context.Instance.Logger.LogError($"Type '{type}' already exists in the array");
            return;
        }

        string _dataPath = Application.persistentDataPath + PDDEditorPaths.AssetsPath;

        if (PDDUtilities.CreateDirectoryIfNotExists(Path.Combine(_dataPath, type)))
        {
            Array.Resize(ref Types, Types.Length + 1);
            Types[Types.Length - 1] = type;

            Context.Instance.EditorBase.Save();
            Context.Instance.Logger.Log($"Type '{type}' added");
        }
    }


    public UsersObjectTypes()
    {
        ObjectType[] elements = (ObjectType[])Enum.GetValues(typeof(ObjectType));
        Types = Array.ConvertAll(elements, e => e.ToString());
    }
}