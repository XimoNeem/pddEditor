using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using PDDEditor.UI;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class ImportPreview_Window : WindowController
{
    [SerializeField] private Transform _previewParent;
    [SerializeField] private Button _selectButton;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private TMP_InputField _pathInput;

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

        _loadButton.onClick.AddListener(LoadAssetBundle);
        _cancelButton.onClick.AddListener(Cancel);
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
    }

    private void LoadAssetBundle()
    {
        _assetBundlePath = _pathInput.text;

        if (string.IsNullOrEmpty(_assetBundlePath))
        {
            Debug.LogError("Path is empty!");
            return;
        }

        StartCoroutine(Load(_assetBundlePath));
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
            yield return www.SendWebRequest();

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
