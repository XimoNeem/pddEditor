using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
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
    private Action<FileSystemInfo> _callback;

    public override void OnEnable()
    {
        base.OnEnable();
        _upButton.onClick.AddListener(MoveUp);
        _selectButton.onClick.AddListener(SelectFolder);
    }

    public void Initialize(FileType type, Action<FileSystemInfo> callback)
    {
        _callback = callback;
        switch (type) 
        {
            case FileType.Dir:
                _showFiles = false;
                break;
            case FileType.All:
                _showFiles = true;
                break;
        }
    }

    private void Start()
    {
        _path = Application.dataPath;
        RefreshFileList(_path);
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
            AddFileTicket(file);
        }
    }

    private void AddFolderTicket(DirectoryInfo dir)
    {
        File_Ticket newItem = Instantiate(_directoryTicket, _contentParent);
        newItem.Name.text = dir.Name;

        newItem.Button.onClick.AddListener( delegate { RefreshFileList(dir.FullName); } );
    }
    private void AddFileTicket(FileInfo file)
    {
        File_Ticket newItem = Instantiate(_fileTicket, _contentParent);
        newItem.Name.text = file.Name;
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
    All
}