using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using PDDEditor.UI;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using Unity.VisualScripting;
using PDDEditor.Paths;

public class ImportPreview_Window : WindowController
{
    [SerializeField] private Transform _previewParent;
    [SerializeField] private Button _selectButton;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _createTypeButton;
    [SerializeField] private Button _toTexturesButton;

    [SerializeField] private TMP_InputField _pathInput;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_Dropdown _typeDropdown;

    private string _assetBundlePath;
    private GameObject _loadedPrefab;

    public override void Initialize(params object[] values)
    {
        base.Initialize(values);

        _pathInput.text = "";
    }

    public override void OnEnable()
    {
        _previewParent = new GameObject().transform;
        base.OnEnable();

        Action<FileSystemInfo> action = OnFileSelected;

        _selectButton.onClick.AddListener(delegate { Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.FilePicker, action, FileType.PDDAsset); } );

        _pathInput.onEndEdit.AddListener(OnPathInput);
        _cancelButton.onClick.AddListener(Cancel);
        _createTypeButton.onClick.AddListener(CreateNewType);
        _loadButton.onClick.AddListener(Import);

        _toTexturesButton.onClick.AddListener(delegate { Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.TexturesPreview); this.AssetContainer.Unload(); });

        Context.Instance.UIDrawer.InitTypesDropdown(_typeDropdown);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        _loadButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        if (_loadedPrefab != null) { Destroy(_loadedPrefab); }
        Destroy(_previewParent.gameObject);
    }

    private void OnFileSelected(FileSystemInfo file)
    {
        _pathInput.text = file.FullName;
        LoadAssetBundle();
    }

    private void CreateNewType()
    {
        PopupHeader header = new PopupHeader("¬ведите название новой группы объектов");
        PopupInput input = new PopupInput("¬ведите название группы", OnTypeInput);
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.PopupWindow, header, input);
    }

    private void OnTypeInput(string type)
    {
        Context.Instance.EditorBase.EditorTypes.CreateNewType(type);
        Context.Instance.UIDrawer.InitTypesDropdown(_typeDropdown);
    }

    private void OnPathInput(string path)
    {
        _assetBundlePath = path;
        LoadAssetBundle();
    }

    private void LoadAssetBundle()
    {
        _assetBundlePath = _pathInput.text;

        if (string.IsNullOrEmpty(_assetBundlePath) | !File.Exists(_assetBundlePath))
        {
            Context.Instance.UIDrawer.TintImageByTime(_pathInput.GetComponent<Image>(), Color.red, Color.white);
            Context.Instance.Logger.LogError("Path is empty!");
            return;
        }

        _nameInput.text = Path.GetFileName(_assetBundlePath.Replace(".pddasset", ""));

        StartCoroutine(Load(_assetBundlePath));
    }

    private void Import()
    {
        if (string.IsNullOrEmpty(_assetBundlePath) | !File.Exists(_assetBundlePath))
        {
            Context.Instance.UIDrawer.TintImageByTime(_pathInput.GetComponent<Image>(), Color.red, Color.white);
            Context.Instance.Logger.LogError("Path is empty!");
            return;
        }

        if (string.IsNullOrEmpty(_nameInput.text))
        {
            Context.Instance.UIDrawer.TintImageByTime(_nameInput.GetComponent<Image>(), Color.red, Color.white);
            Context.Instance.Logger.LogError("Wrong name");
            return;
        }

        string newPath = Path.Combine(Application.persistentDataPath + PDDEditorPaths.AssetsPath, _typeDropdown.captionText.text, _nameInput.text);
        Debug.Log(newPath);
        if (File.Exists(newPath))
        {
            Context.Instance.Logger.LogError("This file is already exists");
            Context.Instance.UIDrawer.TintImageByTime(_nameInput.GetComponent<Image>(), Color.red, Color.white);
            return;
        }

        if (Context.Instance.AssetRegister.Import(_assetBundlePath, _typeDropdown.captionText.text, _nameInput.text, GameObject.Find("PreviewCamera").GetComponent<Camera>()))
        {
            Context.Instance.UIDrawer.GetLoadedWindow<ImportPreview_Window>().AssetContainer.Unload();
            Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ImportPreview);
        }
    }

    private void ShowPrefabPreview(GameObject prefab)
    {
        if (_loadedPrefab != null)
        {
            Destroy(_loadedPrefab);
        }

        _loadedPrefab = Instantiate(prefab, _previewParent);
        _loadedPrefab.transform.localPosition = Vector3.zero;
    }

    IEnumerator Load(string path)
    {
        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(path))
        {
            Debug.Log(path);
            Debug.Log(www.downloadProgress);
            yield return www.SendWebRequest();
            Debug.Log(www.downloadProgress);
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load AssetBundle: " + www.error);
                yield break;
            }
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            GameObject[] prefab = bundle.LoadAllAssets<GameObject>();
            foreach (var item in prefab)
            {
                ShowPrefabPreview(item);
            }
            bundle.Unload(false);
        }
    }

    private void Cancel()
    {
        Context.Instance.AssetLoader.LoadAsset(PDDEditorWindows.MainMenu);
        AssetContainer.Unload();
    }
}
