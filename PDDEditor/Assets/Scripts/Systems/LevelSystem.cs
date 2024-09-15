using PDDEditor.UI;
using System;
using UnityEngine;
using PDDEditor.SceneManagment;
using PDDEditor.Paths;
using System.Collections.Generic;
using System.IO;
using PDDEditor.Assets;
using System.Collections;

public class LevelSystem : MonoBehaviour
{
    public SceneInfo SceneInfo;
    private LoadingHelper _loader;

    private string _scenesPath;
    private Node _nodeTemplate;

    [SerializeField] private List<PDDNode> _items = new List<PDDNode>();
    [SerializeField] private List<PDDNode> _utilities = new List<PDDNode>();

    private void Start()
    {
        _scenesPath = Application.persistentDataPath + PDDEditorPaths.ScenesPath;
        PDDUtilities.CreateDirectoryIfNotExists(_scenesPath);

        _nodeTemplate = Instantiate(Resources.Load<Node>("Node"));
    }
    public void CreateObject(PDDModifiable item, string name = "", string nodeId = "")
    {
        Node result;

        CreateObject(item, out result, name, nodeId);
    }

    public List<UtilityNode> GetAllUtils()
    {
        List<UtilityNode> result = new List<UtilityNode>();
        foreach (var item in _utilities)
        {
            if(item == null) continue;  
            Debug.Log(item);
            Debug.Log(item.GetType());
            if (item.GetType() == typeof(UtilityNode) || item.GetType() == typeof(PDDSpline))
            {
                result.Add((UtilityNode) item);
            }
        }

        return result;
    }

    public PDDNode GetUtilByID(string nodeID)
    {
        PDDNode result = null;
        foreach (var item in _utilities)
        {
            if(item == null) continue;
            if (item.NodeID == nodeID)
            {
                return item; 
            }
        }

        return result;
    }

    public List<PDDNode> GetAllItems()
    {
        return _items;
    }

    public void CreateObject(PDDModifiable item)
    {
        Node result;

        CreateObject(item, out result);
    }

    public void CreateObject(PDDModifiable item, out Node result, string name = "", string nodeId = "")
    {
        Node node = Instantiate(Resources.Load<Node>("Node"));

        node.gameObject.name = item.name;

        if (string.IsNullOrEmpty(name)) { Debug.Log("new name"); node.NodeName = item.name; }
        else { node.NodeName = name; Debug.Log("old name"); }

        if (string.IsNullOrEmpty(nodeId)) { Debug.Log("new ID"); node.NodeID = Guid.NewGuid().ToString(); }
        else { node.NodeID = nodeId; Debug.Log("old name"); }

        node.Item = Instantiate(item, node.transform);

        FindObjectOfType<RuntimeHandle.RuntimeTransformHandle>().SetTarget(node);

        _items.Add(node);

        Context.Instance.EventManager.OnNodeCreated.Invoke(node);

        result = node;
    }

    public UtilityNode CreateUtility(PDDUtility utility, string name = "", string nodeId = "")
    {
        UtilityNode node = Resources.Load<UtilityNode>("UtilityNode");

        return CreateUtility(utility, node, name, nodeId);
    }

    public UtilityNode CreateUtility(PDDUtility utility, UtilityNode currentNode, string name = "", string nodeId = "")
    {
        UtilityNode node = Instantiate(currentNode);
        node.gameObject.name = utility.name;

        if (string.IsNullOrEmpty(name)) { Debug.Log("new name"); node.NodeName = utility.name; }
        else { node.NodeName = name; Debug.Log("old name"); }

        if (string.IsNullOrEmpty(nodeId)) { Debug.Log("new ID"); node.NodeID = Guid.NewGuid().ToString(); }
        else { node.NodeID = nodeId; Debug.Log("old name"); }
        node.Item = Instantiate(utility, node.transform);

        FindObjectOfType<RuntimeHandle.RuntimeTransformHandle>().SetTarget(node);

        _utilities.Add(node);
        node.DrawIngameGizmo();

        Context.Instance.EventManager.OnNodeCreated.Invoke(node);

        return node;
    }

    public void DeleteObject(PDDNode node)
    {
        if (_items.Contains(node))
        {
            _items.Remove(node);
            Context.Instance.EventManager.OnNodeRemove.Invoke(node);
            Destroy(node.gameObject);
        }
    }

    public void DublicateObject(PDDNode node)
    {
        if (_items.Contains(node))
        {
            _items.Remove(node);
            PDDNode copy = Instantiate(node);
            copy.NodeID = Guid.NewGuid().ToString();
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

        LoadUILayout();

        SceneInfo = new SceneInfo(name, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        _items.Clear();
    }

    private void LoadUILayout()
    {
        Context.Instance.SceneLoader.LoadScene(PDDEditorScenes.Main);
        Context.Instance.UIDrawer.UnloadAllWindows();
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.MainTopBar);
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.Hierarchy);
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.MainButtomBar);
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.DebugLayer);
    }

    public void RequestObject()
    {
        Action<string> action = Context.Instance.AssetRegister.LoadAsset;
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ItemsList, action);
    }

    private void OnEnable()
    {
        //Context.Instance.EventManager.OnItemSelected.AddListener(CreateObject);
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
                        PreviewURL = Path.Combine(sceneDirectory, PDDEditorPaths.PreviewImageName),
                        CreatingDate = sceneData.CreatingDate,
                        LastEditDate = sceneData.LastEditDate
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
        Context.Instance.ScreenShoter.GetPreview(SaveScene, 126, 126, "");
    }

    public void SaveScene(Texture2D preview)
    {
        if (SceneInfo == null) { Context.Instance.Logger.LogError("No SceneData privided"); return; }
        SaveScene(SceneInfo.Name, SceneInfo.CreationDate, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), preview);
    }

    public void SaveScene(string name, string creatingDate, string editDate, Texture2D preview)
    {
        string scenePath = Path.Combine(_scenesPath, name);

        PDDSceneData sceneData = new PDDSceneData();
        sceneData.Name = name;
        sceneData.ItemsList = new List<PDDNodeData>();
        sceneData.UtilsList = new List<PDDUtilData>();
        sceneData.CreatingDate = creatingDate;
        sceneData.LastEditDate = editDate;

        foreach (var util in _utilities)
        {
            if (util == null)
            {
                Context.Instance.Logger.LogWarning($"Utility {util} skipped while saving");
                _utilities.Remove(util);
                continue;
            }

            PDDUtilData utilData = new PDDUtilData();

            utilData.Name = util.NodeName;
            utilData.NodeID = util.NodeID;
            utilData.Position = util.transform.position;
            utilData.Rotation = util.transform.eulerAngles;
            utilData.Scale = util.transform.localScale;
            utilData.AssetPath = util.Item.AssetPath;

            utilData.BaseSettings = util.Item.PDDBaseSettings;
            utilData.TransformGroups = util.Item.PDDTransformInfo;
            utilData.ColorSettings = util.Item.ColorSettings;

            sceneData.UtilsList.Add(utilData);
        }

        foreach (var item in _items)
        {
            if (item == null)
            {
                Context.Instance.Logger.LogWarning($"Object {item} skipped while saving");
                _items.Remove(item);
                continue;
            }

            PDDNodeData data = new PDDNodeData();

            data.Name = item.NodeName;
            data.NodeID = item.NodeID;
            data.Position = item.transform.position;
            data.Rotation = item.transform.eulerAngles;
            data.Scale = item.transform.localScale;
            data.AssetPath = item.Item.AssetPath;

            data.BaseSettings = item.Item.PDDBaseSettings;
            data.ToggleSettings = item.Item.ToggleSettings;
            data.ColorSettings = item.Item.ColorSettings;
            data.EmmiterSettings = item.Item.EmmiterSettings;
            data.TextureSettings = item.Item.TextureSettings;
            data.TextSettings = item.Item.TextSettings;


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
        string dataPath = Path.Combine(path, PDDEditorPaths.SceneDataName);

        if (string.IsNullOrEmpty(dataPath) | !File.Exists(dataPath)) { Context.Instance.Logger.LogError($"Path {path} not exists"); return; }

        PDDSceneData sceneData;

        if (PDDUtilities.TryReadFromFile(dataPath, out string dataJson))
        {
            sceneData = JsonUtility.FromJson<PDDSceneData>(dataJson);
            Context.Instance.Logger.Log($"Scene data file was loaded by path {dataPath}");
        }

        else { return; }

        _loader = new LoadingHelper(sceneData.ItemsList.Count, sceneData.UtilsList.Count, sceneData);

        SceneInfo = new SceneInfo(sceneData.Name, sceneData.CreatingDate);
        _items.Clear();

        LoadUILayout();

        StartCoroutine(LoadUtility());
    }

    private IEnumerator LoadUtility()
    {
        if (_loader.UtilsCount == 0)
        {
            IterateLoad();
        }
        else
        {
            PDDUtilData data = _loader.SceneData.UtilsList[_loader.CurrentUtil];

            UtilityNode node = null;
            PDDUtility util = null;
            UtilityNode newNode = null;

            ResourceRequest request = Resources.LoadAsync<PDDUtility>(data.AssetPath);

            yield return request;

            Debug.Log(request.progress);
            if (request.asset != null) { util = request.asset as PDDUtility; Debug.Log(util); }
            else { Debug.LogError("Failed to load resource: " + data.AssetPath); }

            string nodePath = "";

            if (data.AssetPath == "PDDSpline") { nodePath = "SplineNode"; }
            else { nodePath = "UtilityNode"; }

            request = Resources.LoadAsync<UtilityNode>(nodePath);

            yield return request;
            Debug.Log(request.progress);
            if (request.asset != null) { node = request.asset as UtilityNode; Debug.Log(node); }
            else { Debug.LogError("Failed to load resource: " + data.AssetPath); }

            if (node != null & util != null) { newNode = CreateUtility(util, node, data.Name, data.NodeID); }
            else { Debug.LogError("Error loading"); }
            newNode.transform.position = data.Position;
            newNode.transform.eulerAngles = data.Rotation;
            newNode.transform.localScale = data.Scale;

            newNode.Item.PDDBaseSettings = data.BaseSettings;

            for (int i = 0; i < newNode.Item.ColorSettings.Length; i++)
            {
                try { newNode.Item.ColorSettings[i].Value = data.ColorSettings[i].Value; }
                catch (Exception) { Context.Instance.Logger.LogError($"Error while parsing {_loader.CurrentUtil} object settings"); throw; }
            }
            for (int i = 0; i < newNode.Item.PDDTransformInfo.Length; i++)
            {
                try { newNode.Item.PDDTransformInfo[i].Value = data.TransformGroups[i].Value; }
                catch (Exception) { Context.Instance.Logger.LogError($"Error while parsing {_loader.CurrentUtil} object settings"); throw; }
            }
            newNode.Item.Set();
            newNode.DrawIngameGizmo();
            newNode.Item.ApplyBaseSettings();

            yield return new WaitForEndOfFrame();

            _loader.CurrentUtil++;

            if (_loader.CurrentUtil < _loader.UtilsCount)
            {
                StartCoroutine(LoadUtility());
            }
            else { IterateLoad(); }
        }
    }

    private void IterateLoad()
    {
        if (_loader.NodesCount > 0)
        {
            Context.Instance.AssetRegister.LoadAsset(_loader.SceneData.ItemsList[_loader.CurrentNode].AssetPath, InstansiatePDDItem);
        }
    }

    private void InstansiatePDDItem(PDDItem item)
    {
        Node node;
            
        CreateObject(item, out node, _loader.SceneData.ItemsList[_loader.CurrentNode].Name, _loader.SceneData.ItemsList[_loader.CurrentNode].NodeID);

        if (node == null) { return; }

        node.transform.position = _loader.SceneData.ItemsList[_loader.CurrentNode].Position;
        node.transform.eulerAngles = _loader.SceneData.ItemsList[_loader.CurrentNode].Rotation;
        node.transform.localScale = _loader.SceneData.ItemsList[_loader.CurrentNode].Scale;

        node.Item.PDDBaseSettings = _loader.SceneData.ItemsList[_loader.CurrentNode].BaseSettings;

        for (int i = 0; i < node.Item.ToggleSettings.Length; i++)
        {
            try { node.Item.ToggleSettings[i].Value = _loader.SceneData.ItemsList[_loader.CurrentNode].ToggleSettings[i].Value; }
            catch (Exception)
            { Context.Instance.Logger.LogError($"Error while parsing {_loader.CurrentNode} object settings"); throw; }
        }

        for (int i = 0; i < node.Item.ColorSettings.Length; i++)
        {
            try { node.Item.ColorSettings[i].Value = _loader.SceneData.ItemsList[_loader.CurrentNode].ColorSettings[i].Value; }
            catch (Exception) { Context.Instance.Logger.LogError($"Error while parsing {_loader.CurrentNode} object settings"); throw; }
        }

        for (int i = 0; i < node.Item.EmmiterSettings.Length; i++)
        {
            try { node.Item.EmmiterSettings[i].EmmiterObjects = _loader.SceneData.ItemsList[_loader.CurrentNode].EmmiterSettings[i].EmmiterObjects; }
            catch (Exception) { Context.Instance.Logger.LogError($"Error while parsing {_loader.CurrentNode} object settings"); throw; }
        }

        for (int i = 0; i < node.Item.TextureSettings.Length; i++)
        {
            try { node.Item.TextureSettings[i].Value = _loader.SceneData.ItemsList[_loader.CurrentNode].TextureSettings[i].Value; }
            catch (Exception) { Context.Instance.Logger.LogError($"Error while parsing {_loader.CurrentNode} object settings"); throw; }
        }

        for (int i = 0; i < node.Item.TextSettings.Length; i++)
        {
            try { node.Item.TextSettings[i].Value = _loader.SceneData.ItemsList[_loader.CurrentNode].TextSettings[i].Value; }
            catch (Exception) { Context.Instance.Logger.LogError($"Error while parsing {_loader.CurrentNode} object settings"); throw; }
        }

        node.Item.Set();
        node.Item.ApplyBaseSettings();

        Context.Instance.Logger.Log($"Loaded {_loader.CurrentNode} object");

        _loader.CurrentNode++;
        if (_loader.CurrentNode < _loader.NodesCount)
        {
            IterateLoad();
        }
        else { Context.Instance.Logger.Log("Scene loaded"); _loader = null; }
    }
}

public class LoadingHelper
{
    public int NodesCount;
    public int UtilsCount;
    public int CurrentNode;
    public int CurrentUtil;
    public PDDSceneData SceneData;

    public LoadingHelper(int nodesCount, int utilsCount, PDDSceneData data)
    {
        NodesCount = nodesCount;
        UtilsCount = utilsCount;
        CurrentNode = 0;
        CurrentUtil = 0;
        SceneData = data;
    }
}

public class SceneInfo
{ 
    public string Name;
    public string CreationDate;

    public SceneInfo(string name, string creationDate)
    {
        Name = name;
        CreationDate = creationDate;
    }
}