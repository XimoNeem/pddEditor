using UnityEngine.UI;
using UnityEngine;
using PDDEditor.UI;
using PDDEditor.SceneManagment;

public class MainMenu_Window : WindowController
{
    [SerializeField] private Button _loadButton;

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
    }
}
