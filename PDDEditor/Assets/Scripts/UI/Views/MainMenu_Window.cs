using UnityEngine.UI;
using UnityEngine;
using PDDEditor.UI;
using PDDEditor.SceneManagment;

public class MainMenu_Window : WindowController
{
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _scenesListButton;
    [SerializeField] private Button _SettingsButton;
    [SerializeField] private Button _importButton;
    [SerializeField] private Button _exitButton;

    private void Awake()
    {
        _loadButton.onClick.AddListener
        (
            delegate 
            { 
                Context.Instance.SceneLoader.LoadScene(PDDEditorScenes.Main);
                AssetContainer.Unload();
                Context.Instance.AssetLoader.LoadAsset(PDDEditorWindows.MainTopBar);
                Context.Instance.AssetLoader.LoadAsset(PDDEditorWindows.MainButtomBar);
            }
        );

        _exitButton.onClick.AddListener( delegate { Application.Quit(); } );
        _importButton.onClick.AddListener( delegate 
        {
            Context.Instance.AssetLoader.LoadAsset(PDDEditorWindows.ImportPreview);
            AssetContainer.Unload();
        } 
        );
    }
}
