using System;
using Unity.VisualScripting;
using UnityEngine;

public class Context : MonoBehaviour
{
    public static Context Instance;

    public UIDrawer UIDrawer;
    public EventManager EventManager;
    public SceneLoader SceneLoader;
    public InputSystem InputSystem;
    public LocalAssetLoader AssetLoader;
    public LevelSystem LevelSystem;
    public ScreenShoter ScreenShoter;
    public DebugSystem Logger;
    public PDDEditorContainer EditorBase;
    public AssetRegister AssetRegister;
    public RunTimeSystem RunTimeSystem;
    public NativeWindowsUtility WindowsUtility;
    public UnityMainThreadDispatcher MainThreadDispatcher;

    //public PDDUtilities Utilities;


    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance == null) { Instance = this; }
        else { Destroy(this); }
    }

    public void Initialize(Action Callback)
    {
        EventManager = new EventManager();
        SceneLoader = new SceneLoader();
        ScreenShoter = new ScreenShoter();
        Logger = new DebugSystem();
        EditorBase = new PDDEditorContainer();

        UIDrawer = this.AddComponent<UIDrawer>();
        InputSystem = this.AddComponent<InputSystem>();
        AssetLoader = this.AddComponent<LocalAssetLoader>();
        LevelSystem = this.AddComponent<LevelSystem>();
        AssetRegister = this.AddComponent<AssetRegister>();
        RunTimeSystem = this.AddComponent<RunTimeSystem>();
        WindowsUtility = this.AddComponent<NativeWindowsUtility>();
        MainThreadDispatcher = this.AddComponent<UnityMainThreadDispatcher>();

        Context.Instance.Logger.Log("Context initialized");

        Callback.Invoke();
    }
}
