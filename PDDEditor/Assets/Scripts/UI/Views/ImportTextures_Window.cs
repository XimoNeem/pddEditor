using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using PDDEditor.UI;
using System.Collections.Generic;

public class ImportTextures_Window : WindowController
{
    [SerializeField] private Transform _textureItemsContent;
    [SerializeField] private RawImage _textureItemTemplate;

    [SerializeField] private Button _selectButton;
    [SerializeField] private Button _createDirectory;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _assetsButton;
    [SerializeField] private TMP_InputField _pathInput;
    [SerializeField] private TMP_Dropdown _typeDropdown;

    private string _textureFolderPath;
    private List<string> _texturesToImport = new List<string>();

    public override void Initialize(params object[] values)
    {
        base.Initialize(values);
        _pathInput.text = "";
    }

    public override void OnEnable()
    {
        base.OnEnable();

        Action<FileSystemInfo> action = OnFolderSelected;

        _selectButton.onClick.AddListener(delegate { Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.FilePicker, action, FileType.Dir); });

        _pathInput.onEndEdit.AddListener(OnPathInput);
        _cancelButton.onClick.AddListener(Cancel);
        _assetsButton.onClick.AddListener(delegate { Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ImportPreview); this.AssetContainer.Unload(); });
        _loadButton.onClick.AddListener(ImportTextures);

        _createDirectory.onClick.AddListener(CreateNewDirectory);
        InitDropDown();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        _loadButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();
    }

    private void OnFolderSelected(FileSystemInfo folder)
    {
        _pathInput.text = folder.FullName;
        _textureFolderPath = folder.FullName;

        PreviewTextures();
    }

    private void InitDropDown()
    {
        _typeDropdown.ClearOptions();
        List<String> names = new List<String>();
        foreach (var item in Context.Instance.AssetRegister.GetAllTextureDirectories())
        {
            names.Add(Path.GetFileName(item));
        }
        _typeDropdown.AddOptions(names);
    }

    private void OnPathInput(string path)
    {
        _textureFolderPath = path;

        PreviewTextures();
    }

    private void PreviewTextures()
    {
        if (string.IsNullOrEmpty(_textureFolderPath) || !Directory.Exists(_textureFolderPath))
        {
            Context.Instance.UIDrawer.TintImageByTime(_pathInput.GetComponent<Image>(), Color.red, _pathInput.image.color);
            Context.Instance.Logger.LogError("Path is empty or invalid!");
            return;
        }

        string[] textureFiles = Directory.GetFiles(_textureFolderPath, "*.png");

        foreach (var filePath in textureFiles)
        {
            _texturesToImport.Add(filePath);
            StartCoroutine(LoadTexture(filePath));
        }
    }

    private void ImportTextures()
    {
        if (string.IsNullOrEmpty(_typeDropdown.captionText.text))
        {
            Context.Instance.UIDrawer.TintImageByTime(_typeDropdown.image, Color.red, _typeDropdown.image.color);
            return;
        }
        if (string.IsNullOrEmpty(_pathInput.text))
        {
            Context.Instance.UIDrawer.TintImageByTime(_pathInput.image, Color.red, _typeDropdown.image.color);
            return;
        }

        if (Context.Instance.AssetRegister.ImportTextures(_texturesToImport.ToArray(), _typeDropdown.captionText.text))
        {
            this.AssetContainer.Unload();
            Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.TexturesPreview);
        }
    }

    private void CreateNewDirectory()
    {
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.PopupWindow, new PopupHeader("Введите имя нового каталога"), new PopupInput("Имя каталога", OnDirectoryInput));
    }

    private void OnDirectoryInput(string name)
    {
        if (!Context.Instance.AssetRegister.CreateTextureDirectory(name))
        {
            Context.Instance.UIDrawer.TintImageByTime(_createDirectory.image, Color.red, _createDirectory.image.color);
        }
        else
        {
            Context.Instance.UIDrawer.TintImageByTime(_createDirectory.image, Color.green, _createDirectory.image.color);
            InitDropDown();
        } 
    }

    IEnumerator LoadTexture(string path)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load texture: " + www.error);
                yield break;
            }

            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            RawImage newButton = Instantiate(_textureItemTemplate, _textureItemsContent);
            newButton.texture = texture;
            newButton.gameObject.GetComponentInChildren<Button>().onClick.AddListener( 
                delegate {
                    DeleteTexture(path);
                    Destroy(newButton.gameObject);
                });
            newButton.gameObject.SetActive(true);
        }
    }

    private void DeleteTexture(string path)
    {
        _texturesToImport.Remove(path);
    }

    private void Cancel()
    {
        Context.Instance.AssetLoader.LoadAsset(PDDEditorWindows.MainMenu);
        AssetContainer.Unload();
    }
}
