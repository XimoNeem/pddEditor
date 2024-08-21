using PDDEditor.UI;
using System;
using UnityEngine;
using PDDEditor.SceneManagment;
using PDDEditor.Paths;
using System.Collections.Generic;
using System.IO;

public class LevelSystem : MonoBehaviour
{
    private string _scenesPath;

    private string _currentName = "testt22222t";
    private List<Node> _items = new List<Node>();

    private void Start()
    {
        _scenesPath = Application.persistentDataPath + PDDEditorPaths.ScenesPath;
        PDDUtilities.CreateDirectoryIfNotExists(_scenesPath);
    }
    public void CreateObject(PDDItem item)
    {
        Node node = Instantiate(Resources.Load<Node>("Node"));

        node.gameObject.name = item.name;

        node.NodeName = item.name;
        node.NodeID = 0000000;
        node.Item = Instantiate(item, node.transform);

        FindObjectOfType<RuntimeHandle.RuntimeTransformHandle>().SetTarget(node.gameObject);
    }

    public void RequestObject()
    {
        Action<PDDItem> action = CreateObject;
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ItemsList, action);
    }

    private void OnEnable()
    {
        Context.Instance.EventManager.OnItemSelected.AddListener(CreateObject);
    }

    private void OnDisable()
    {
        
    }

    public List<PDDScene> GetAllScenes()
    {
        List<PDDScene> result = new List<PDDScene>();

        if (!PDDUtilities.CreateDirectoryIfNotExists(_scenesPath))
        {
            string[] sceneDirectories = Directory.GetDirectories(_scenesPath);

            foreach (string sceneDirectory in sceneDirectories)
            {
                string sceneDataPath = Path.Combine(sceneDirectory, PDDEditorPaths.SceneDataName);

                if (File.Exists(sceneDataPath))
                {
                    string json = File.ReadAllText(sceneDataPath);
                    PDDSceneData sceneData = JsonUtility.FromJson<PDDSceneData>(json);

                    PDDScene scene = new PDDScene
                    {
                        Name = sceneData.Name,
                        Path = sceneDirectory,
                        PreviewURL = Path.Combine(sceneDirectory, PDDEditorPaths.PreviewImageName)
                    };

                    result.Add(scene);
                }
            }
        }
        else
        {
            Debug.LogWarning("Scene directory does not exist.");
        }

        return result;
    }

    public void SaveScene()
    {
        Context.Instance.ScreenShoter.GetPreview(SaveScene, 126, 126);
    }

    public void SaveScene(Texture2D preview)
    {
        SaveScene(_currentName, _items, preview);
    }

    public void SaveScene(string name, List<Node> items, Texture2D preview)
    {
        PDDSceneData sceneData = new PDDSceneData();
        sceneData.Name = name;
        sceneData.ItemsList = items;

        string scenePath = Path.Combine(_scenesPath, name);

        if (PDDUtilities.CreateDirectoryIfNotExists(scenePath))
        {
            Debug.Log("Saving");
        }
        else { Debug.Log("Rewriting"); }


        byte[] previewBytes = preview.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(scenePath, PDDEditorPaths.PreviewImageName), previewBytes);

        string json = JsonUtility.ToJson(sceneData);

        string sceneFilePath = Path.Combine(scenePath, PDDEditorPaths.SceneDataName);
        File.WriteAllText(sceneFilePath, json);

        Context.Instance.Logger.Log("Scene saved: " + sceneFilePath);
    }
}