using System.Collections;
using System.IO;
using UnityEngine;

public class PDDEditorContainer
{
    public EditorSettings EditorSettings;
    /*public EditorInput EditorInput;*/

    public PDDEditorContainer()
    {
        string settingsPath = Path.Combine(Application.persistentDataPath, PDDEditor.Paths.PDDEditorPaths.EditorSettings);
        if (PDDUtilities.TryReadFromFile(settingsPath, out string json))
        {
            EditorSettings = JsonUtility.FromJson<EditorSettings>(json);
            Debug.Log("EditorSettings exists");
        }
        else { EditorSettings = new EditorSettings(); Debug.Log("new EditorSettings created"); }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(EditorSettings, true);
        string settingsPath = Path.Combine(Application.persistentDataPath, PDDEditor.Paths.PDDEditorPaths.EditorSettings);
        PDDUtilities.WriteTextToFile(settingsPath, json);
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