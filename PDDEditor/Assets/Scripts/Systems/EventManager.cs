using System;
using System.IO;
using UnityEngine.Events;

public class EventManager
{
    public UnityEvent OnSceneLoaded;
    public UnityEvent<PDDItem> OnItemSelected;
    public UnityEvent<Node> OnNodeSelected;
    public UnityEvent<Node> OnNodeSettings;
    public UnityEvent<FileType, Action<FileSystemInfo>> OnFileRequest;

    public EventManager()
    {
        OnSceneLoaded = new UnityEvent();
        OnItemSelected = new UnityEvent<PDDItem>();
        OnNodeSelected = new UnityEvent<Node>();
        OnNodeSettings = new UnityEvent<Node>();
        OnFileRequest = new UnityEvent<FileType, Action<FileSystemInfo>>();
    }
}
