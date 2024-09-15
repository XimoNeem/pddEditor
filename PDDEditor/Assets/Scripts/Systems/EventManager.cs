using System;
using System.IO;
using UnityEngine.Events;

public class EventManager
{
    public UnityEvent OnSceneLoaded;
    public UnityEvent<PDDItem> OnItemSelected;
    public UnityEvent<PDDNode> OnNodeSelected;
/*    public UnityEvent<UtilityNode> OnUtilitySelected;*/
    public UnityEvent<PDDNode> OnNodeSettings;
    public UnityEvent<PDDNode> OnNodeRemove;
    public UnityEvent<PDDNode> OnNodeCreated;
    public UnityEvent<FileType, Action<FileSystemInfo>> OnFileRequest;

    public EventManager()
    {
        OnSceneLoaded = new UnityEvent();
        OnItemSelected = new UnityEvent<PDDItem>();
        OnNodeSelected = new UnityEvent<PDDNode>();
/*        OnUtilitySelected = new UnityEvent<UtilityNode>();*/
        OnNodeSettings = new UnityEvent<PDDNode>();
        OnFileRequest = new UnityEvent<FileType, Action<FileSystemInfo>>();
        OnNodeRemove = new UnityEvent<PDDNode>();
        OnNodeCreated = new UnityEvent<PDDNode>();
    }
}
