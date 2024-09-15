using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using PDDEditor.Paths;

public class TexturePicker_Window: WindowController
{
    [SerializeField] private Transform _contentParent;
    [SerializeField] private RawImage _textureItemTemplate;

    [SerializeField] private TMP_Dropdown _directoryDropdown;

    private string _directory;
    private string _fileName;

    private Action<string, string> _callback;

    public override void OnEnable()
    {
        base.OnEnable();
        _directoryDropdown.onValueChanged.AddListener(OnSelectFolder);
    }

    public override void Initialize(params object[] values)
    {
        base.Initialize(values);
        InitDropDown();

        if (values[0].GetType() != typeof(Action<string, string>))
        {
            Debug.LogError("Need object of type <Action<string, string>> for initialization");
            return;
        }

        if (values.Length > 1)
        {
            _directory = (string)values[1];
            for (int i = 0; i < _directoryDropdown.options.Count; i++)
            {
                if (_directoryDropdown.options[i] != null)
                {
                    if (_directoryDropdown.options[i].text == _directory)
                    {
                        _directoryDropdown.SetValueWithoutNotify(i);
                        break;
                    }
                }
            }
        }
        else
        {
            if (_directoryDropdown.options.Count > 0)
            {
                _directory = _directoryDropdown.options[0].text;
            }
        }

        _callback = (Action<string, string>)values[0];
    }

    private void InitDropDown()
    {
        _directoryDropdown.ClearOptions();
        _directoryDropdown.AddOptions(Context.Instance.AssetRegister.GetAllTextureDirectories());
    }

    private void Start()
    {
        RefreshFileList();
    }

    private void OnSelectFolder(int value)
    {
        _directory = _directoryDropdown.captionText.text;
        ClearList();
        RefreshFileList();
    }

    private void OnSelectTexture(string path)
    {
        
    }

    private void RefreshFileList()
    {
        string path = Path.Combine((Application.persistentDataPath + PDDEditorPaths.TexturesPath), _directory);

        DirectoryInfo directory = new DirectoryInfo(path);

        FileInfo[] files = directory.GetFiles();
        DirectoryInfo[] directories = directory.GetDirectories();

        ClearList();


        foreach (FileInfo file in files)
        {
            AddTextureTicket(file);
        }
    }

    private void AddTextureTicket(FileInfo path)
    {
        Texture2D texture = PDDUtilities.LoadTextureFromFile(path.FullName);
        
        if(texture == null) { return; }

        RawImage newImage = Instantiate(_textureItemTemplate, _contentParent);
        newImage.gameObject.SetActive(true);
        newImage.texture = texture;
        newImage.GetComponent<AspectRatioFitter>().aspectRatio = (float)texture.width / (float)
            texture.height;

        string dir = _directoryDropdown.captionText.text;
        string tex = path.Name;

        newImage.GetComponent<Button>().onClick.AddListener(delegate
        {
            _callback.Invoke(dir, tex);
            this.AssetContainer.Unload();
        });
    }

    private void ClearList()
    {
        for (int i = _contentParent.childCount - 1; i >= 0; i--)
        {
            Transform child = _contentParent.GetChild(i);
            Destroy(child.gameObject);
        }
    }
}