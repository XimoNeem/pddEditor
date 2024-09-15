using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.IO;
using System;

public class FilePicker_Window : WindowController
{
    [SerializeField] private Button _upButton;
    [SerializeField] private Button _selectButton;
    [SerializeField] private TMP_InputField _pathInput;
    [SerializeField] private Transform _contentParent;

    [SerializeField] private File_Ticket _fileTicket;
    [SerializeField] private File_Ticket _directoryTicket;

    private string _path;
    private DirectoryInfo _currentDirectory;

    private bool _showFiles;
    private string _fileType;
    private Action<FileSystemInfo> _callback;

    public override void OnEnable()
    {
        base.OnEnable();
        _upButton.onClick.AddListener(MoveUp);
        _selectButton.onClick.AddListener(SelectFolder);
    }

    public override void Initialize(params object[] values)
    {
        base.Initialize(values);

        if (values[0].GetType() != typeof(Action<FileSystemInfo>))
        {
            Debug.LogError("Need object of type <Action<Color>> for initialization");
            return;
        }

        _callback = (Action<FileSystemInfo>)values[0];
        FileType type = (FileType)values[1];

        switch (type)
        {
            case FileType.Dir:
                _showFiles = false;
                break;
            case FileType.All:
                _showFiles = true;
                break;
            case FileType.PDDAsset:
                _showFiles = true;
                _fileType = "pddasset";
                break;
        }

        _path = Application.dataPath;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        Action<FileSystemInfo> callback = OnInput;
        NativeWindowsUtility.ShowFileBrowser(_fileType, !_showFiles, callback);

        foreach (var item in this.GetComponentsInChildren<Transform>())
        {
            item.gameObject.SetActive(false);
        }

        return;
#endif
    }

    private void Start()
    {
        _path = Application.dataPath;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        return;
#endif
        RefreshFileList(_path); // Для редактора и других платформ
    }

    private void OnInput(FileSystemInfo fileSystemInfo)
    {
        Context.Instance.Logger.LogWarning(fileSystemInfo.FullName);
        _callback.Invoke(fileSystemInfo);

    }

    private void SelectFolder()
    {
        _callback.Invoke(_currentDirectory);
        AssetContainer.Unload();
    }

    private void RefreshFileList(string path)
    {
        _path = path;
        _pathInput.text = _path;

        DirectoryInfo directory = new DirectoryInfo(_path);
        _currentDirectory = directory;

        FileInfo[] files = directory.GetFiles();
        DirectoryInfo[] directories = directory.GetDirectories();

        ClearList();

        foreach (DirectoryInfo dir in directories)
        {
            AddFolderTicket(dir);
        }

        if (!_showFiles) { return; }

        foreach (FileInfo file in files)
        {
            if (string.IsNullOrEmpty(_fileType) || file.Name.EndsWith(_fileType))
            {
                AddFileTicket(file);
            }
        }
    }

    private void AddFolderTicket(DirectoryInfo dir)
    {
        File_Ticket newItem = Instantiate(_directoryTicket, _contentParent);
        newItem.Name.text = dir.Name;

        newItem.Button.onClick.AddListener(delegate { RefreshFileList(dir.FullName); });
    }

    private void AddFileTicket(FileInfo file)
    {
        File_Ticket newItem = Instantiate(_fileTicket, _contentParent);
        newItem.Name.text = file.Name;

        if (_showFiles)
        {
            newItem.Button.onClick.AddListener(delegate { _callback.Invoke(file); });
            newItem.Button.onClick.AddListener(delegate { AssetContainer.Unload(); });
        }
    }

    private void MoveUp()
    {
        RefreshFileList(_currentDirectory.Parent.FullName);
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

public enum FileType
{
    Dir,
    All,
    PDDAsset
}
