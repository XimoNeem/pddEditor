using System;
using System.Collections;
using System.Collections.Generic;
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


    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance == null) { Instance = this; }
        else { Destroy(this); }
    }

    public void Initialize(Action Callback)
    {
        Debug.Log("Context created");

        EventManager = new EventManager();
        SceneLoader = new SceneLoader();
        ScreenShoter = new ScreenShoter();

        UIDrawer = this.AddComponent<UIDrawer>();
        InputSystem = this.AddComponent<InputSystem>();
        AssetLoader = this.AddComponent<LocalAssetLoader>();
        LevelSystem = this.AddComponent<LevelSystem>();

        Debug.Log("Context initialized");

        Callback.Invoke();
    }
}
