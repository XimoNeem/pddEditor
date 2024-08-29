using UnityEngine.UI;
using UnityEngine;
using PDDEditor.UI;
using PDDEditor.SceneManagment;
using System.IO;

public class MainMenu_Window : WindowController
{
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _scenesListButton;
    [SerializeField] private Button _scenesListButton_close;
    [SerializeField] private Button _SettingsButton;
    [SerializeField] private Button _importButton;
    [SerializeField] private Button _assetsManagerButton;
    [SerializeField] private Button _exitButton;

    [SerializeField] private Scene_Ticket _sceneTicket;
    [SerializeField] private Transform _sceneContent;
    [SerializeField] private GameObject _sceneView;

    private void Awake()
    {
        _loadButton.onClick.AddListener(Context.Instance.LevelSystem.CreateNewScene);

        _exitButton.onClick.AddListener( delegate { Application.Quit(); } );
        _importButton.onClick.AddListener( delegate 
        {
            Context.Instance.AssetLoader.LoadAsset(PDDEditorWindows.ImportPreview);
            AssetContainer.Unload();
        } 
        );

        _scenesListButton.onClick.AddListener(ShowScenes);
        _scenesListButton_close.onClick.AddListener(HideScenes);
        _assetsManagerButton.onClick.AddListener(delegate { Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.AssetsManager); } );
    }

    private void ShowScenes()
    {
        _sceneView.SetActive(true);

        var scenes = Context.Instance.LevelSystem.GetAllScenes();

        foreach (var item in scenes)
        {
            Scene_Ticket ticket = Instantiate(_sceneTicket, _sceneContent);
            ticket.Name.text = item.Name;
            ticket.Path.text = item.Path;


            if (File.Exists(item.PreviewURL))
            {
                byte[] previewData = File.ReadAllBytes(item.PreviewURL);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(previewData);
                ticket.Preview.texture = texture;
            }
            else
            {
                Context.Instance.Logger.LogWarning("Preview image not found at path: " + item.PreviewURL);
            }
        }
    }

    private void HideScenes()
    {
        _sceneView.SetActive( false );

        for (int i = 0; i < _sceneContent.childCount; i++)
        {
            Destroy(_sceneContent.GetChild(i).gameObject);
        }
    }
}
