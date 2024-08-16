using System;
using Unity.VisualScripting;
using UnityEngine;

public class Context : MonoBehaviour
{
    public static Context Instance;

    public AssetRegister AssetRegister;
    public UIDrawer UIDrawer;
    public EventManager EventManager;
    public SceneLoader SceneLoader;
    public InputSystem InputSystem;
    public LocalAssetLoader AssetLoader;
    public LevelSystem LevelSystem;
    public ScreenShoter ScreenShoter;
    public DebugSystem Logger;
/*    public PDDUtilities Utilities;*/
    public PDDEditorContainer EditorBase;


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
/*        Utilities = new PDDUtilities();*/
        EditorBase = new PDDEditorContainer();

        UIDrawer = this.AddComponent<UIDrawer>();
        InputSystem = this.AddComponent<InputSystem>();
        AssetLoader = this.AddComponent<LocalAssetLoader>();
        LevelSystem = this.AddComponent<LevelSystem>();
        AssetRegister = this.AddComponent<AssetRegister>();

        Context.Instance.Logger.Log("Context initialized");

        Callback.Invoke();
    }
}
