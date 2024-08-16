using System.Collections;
using UnityEngine;

public class PDDEditorContainer
{
    public EditorSettings EditorSettings;
    /*public EditorInput EditorInput;*/

    public PDDEditorContainer()
    {
        if (PDDUtilities.TryReadFromFile(PDDEditor.Paths.PDDEditorPaths.EditorSettings, out string json))
        {
            EditorSettings = JsonUtility.FromJson<EditorSettings>(json);
            Debug.Log("EditorSettings exists");
        }
        else { EditorSettings = new EditorSettings(); Debug.Log("new EditorSettings created"); }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(EditorSettings, true);
        PDDUtilities.WriteTextToFile(PDDEditor.Paths.PDDEditorPaths.EditorSettings, json);
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