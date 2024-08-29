using PDDEditor.UI;
using System;
using UnityEngine;
using PDDEditor.SceneManagment;
using PDDEditor.Paths;
using System.Collections.Generic;
using System.IO;
using PDDEditor.Assets;
using UnityEditor;

public class LevelSystem : MonoBehaviour
{
    private SceneInfo _sceneInfo;
    private LoadingHelper _loader;

    private string _scenesPath;

    [SerializeField] private List<Node> _items = new List<Node>();

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

        _items.Add(node);
    }

    public void DeleteObject(Node node)
    {
        if (_items.Contains(node))
        {
            _items.Remove(node);
            Destroy(node.gameObject);
        }
    }

    public void DublicateObject(Node node)
    {
        if (_items.Contains(node))
        {
            _items.Remove(node);
            Node copy = Instantiate(node);
            _items.Add(copy);
        }
    }

    public void CreateNewScene()
    {
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.PopupWindow, new PopupHeader("Выберите название для сцены:"), new PopupInput("Название сцены", OnSceneNameInput));
    }

    private void OnSceneNameInput(string name)
    {
        foreach (var item in GetAllScenes())
        {
            if (item.Name == name)
            {
                Context.Instance.Logger.Log("Scene name {name} is already exists");
                return;
            }
        }

        Context.Instance.SceneLoader.LoadScene(PDDEditorScenes.Main);
        Context.Instance.UIDrawer.UnloadAllWindows();
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.MainTopBar);
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.MainButtomBar);

        _sceneInfo = new SceneInfo(name);
    }

    public void RequestObject()
    {
        Action<string> action = Context.Instance.AssetRegister.LoadAsset;
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
        if (_sceneInfo == null) { Context.Instance.Logger.LogError("No SceneData privided"); return; }
        SaveScene(_sceneInfo.Name, _items, preview);
    }

    public void SaveScene(string name, List<Node> items, Texture2D preview)
    {
        string scenePath = Path.Combine(_scenesPath, name);

        PDDSceneData sceneData = new PDDSceneData();
        sceneData.Name = name;
        sceneData.ItemsList = new List<PDDNodeData>();

        foreach (var item in _items)
        {
            PDDNodeData data = new PDDNodeData();

            data.Position = item.transform.position;
            data.Rotation = item.transform.eulerAngles;
            data.Scale = item.transform.localScale;
            data.AssetPath = item.Item.AssetPath;
            data.ToggleSettings = item.Item.ToggleSettings;

            sceneData.ItemsList.Add(data);
        }

        if (PDDUtilities.CreateDirectoryIfNotExists(scenePath)) { Debug.Log("Saving"); }
        else { Debug.Log("Rewriting"); }


        byte[] previewBytes = preview.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(scenePath, PDDEditorPaths.PreviewImageName), previewBytes);

        string json = JsonUtility.ToJson(sceneData);

        string sceneFilePath = Path.Combine(scenePath, PDDEditorPaths.SceneDataName);
        File.WriteAllText(sceneFilePath, json);

        Context.Instance.Logger.Log("Scene saved: " + sceneFilePath);
    }

    public void LoadScene(string path)
    {
        if (string.IsNullOrEmpty(path) | !File.Exists(path)) { Context.Instance.Logger.LogError($"Path {path} not exists"); return; }

        PDDSceneData sceneData;

        if (PDDUtilities.TryReadFromFile(path, out string dataJson))
        {
            sceneData = JsonUtility.FromJson<PDDSceneData>(dataJson);
            Context.Instance.Logger.Log($"Scene data file was loaded by path {path}");
        }

        else { return; }

        _loader = new LoadingHelper(sceneData.ItemsList.Count, sceneData);
    }



    private void LoadingIterator()
    {
        
    }
}

public class LoadingHelper
{
    public int NodesCount;
    public int CurrentNode;
    public PDDSceneData SceneData;

    public LoadingHelper(int nodesCount, PDDSceneData data)
    {
        NodesCount = nodesCount;
        CurrentNode = 0;
        SceneData = data;
    }
}

public class SceneInfo
{ 
    public string Name;

    public SceneInfo(string name)
    {
        Name = name;
    }
}