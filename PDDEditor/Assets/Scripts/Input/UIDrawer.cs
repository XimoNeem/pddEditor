using PDDEditor.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;


public class UIDrawer : MonoBehaviour
{
    private LocalAssetProvider _settingWindow;
    private GUIStyle _style = null;
    private Texture2D _background;
    private bool _showHint = false;
    private string _hintText;


    private object[] _cachedValues;

    public List<LocalAssetProvider> _loadedProviders;


    private void OnEnable()
    {
        _loadedProviders = new List<LocalAssetProvider>();
        Context.Instance.EventManager.OnNodeSelected.AddListener(ShowSettingsWindow);
        //Context.Instance.EventManager.OnUtilitySelected.AddListener(ShowSettingsWindow);
    }

    private void OnDisable()
    {
        Context.Instance.EventManager.OnNodeSelected.RemoveListener(ShowSettingsWindow);
        //Context.Instance.EventManager.OnUtilitySelected.RemoveListener(ShowSettingsWindow);
    }

    public void ShowWindow(string name, params object[] values)
    {
        LocalAssetProvider provider = Context.Instance.AssetLoader.LoadAsset(name, InitializeWindow);
        _cachedValues = values;
    }

    private void InitializeWindow(AsyncOperationHandle<GameObject> item)
    {
        if (item.Result.TryGetComponent(out LoadedAsset asset))
        {
            asset.AssetContainer.CachedObject.Initialize(_cachedValues);

        }
        else { Debug.LogError("Loaded assets must be typeof <LoadedAsset>"); }
    }

    public void UnloadAllWindows()
    {
        for (int i = 0; i < _loadedProviders.Count; i++)
        {
            if (_loadedProviders[i] != null)
            {
                try
                {
                    _loadedProviders[i].Unload();
                }
                catch (Exception)
                {
                }
            }
        }

        _loadedProviders.Clear();
    }

    public T GetLoadedWindow<T>() where T : WindowController
    {
        foreach (var provider in _loadedProviders)
        {
            if (provider != null && provider.gameObject.GetComponentInChildren<T>())
            {
                return provider.gameObject.GetComponentInChildren<T>();
            }
        }
        return null;
    }

    public void ShowHint(string text)
    {
        _hintText = text;
        _showHint = true;
    }

    public void HideHint()
    { 
        _showHint = false;
    }

    private void ShowSettingsWindow(PDDNode node)
    {
        if (GetLoadedWindow<ObjectSettings_Window>() == null)
        {
            ShowWindow(PDDEditor.UI.PDDEditorWindows.ObjectSettings, node);
        }

        Context.Instance.EventManager.OnNodeSettings.Invoke(node);
    }

    public void TintImageByTime(Image image, Color tintColor, Color originalColor, float time)
    {
        StartCoroutine(ChangeColor(image, tintColor, originalColor, time));
    }

    public void TintImageByTime(Image image, Color tintColor, Color originalColor)
    {
        StartCoroutine(ChangeColor(image, tintColor, originalColor, 0.5f));
    }

    private IEnumerator ChangeColor(Image image, Color tintColor, Color originalColor, float time)
    {
        float iteration = 0;
        while (iteration < time)
        {
            image.color = Color.Lerp(tintColor, originalColor, iteration / time);
            iteration += Time.deltaTime;
            yield return null;
        }
    }

    public void InitTypesDropdown(TMP_Dropdown dropdown)
    {
        //ObjectType[] elements = (ObjectType[])Enum.GetValues(typeof(ObjectType));
        string[] elementStrings = PDDObjectTypes.GetTypes();

        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(elementStrings));

        if (dropdown.GetComponentInChildren<ScrollRect>())
        {
            dropdown.GetComponentInChildren<ScrollRect>().scrollSensitivity = 30;
        }
    }

    public void SetRawImageFromPath(string imagePath, RawImage previewImage)
    {
        StartCoroutine(LoadTexture(imagePath, previewImage));
    }

    private IEnumerator LoadTexture(string imagePath, RawImage previewImage)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imagePath))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                previewImage.texture = DownloadHandlerTexture.GetContent(uwr);
            }
            else
            {
                Debug.LogError($"Failed to load texture from {imagePath}: {uwr.error}");
            }
        }
    }

    private void OnGUI()
    {
        CreateCursorItems();
        if (!_showHint) { return; }
        if (Context.Instance.InputSystem.InputBlocked) { return; }
        if (Input.GetMouseButton(1)) { return; }

        var mousePosition = Input.mousePosition;

        float width = _hintText.Length * 9.5f;
        float height = 20;
        float x = mousePosition.x - width - 5;
        float y = Screen.height - mousePosition.y - height / 2;
        var rect = new Rect(x, y, width, height);


        GUI.Box(rect, _hintText, _style);
    }

    private void CreateCursorItems()
    {
        if (_background == null) { _background = CreateBackground(2, 2, Color.white); }

        if (_style == null)
        {
            _style = new GUIStyle(GUI.skin.box);
            _style.normal.background = _background;
            _style.normal.textColor = Color.black;
            _style.fontSize = 16;
            _style.alignment = TextAnchor.MiddleCenter;
        }
    }

    private Texture2D CreateBackground(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
