using UnityEngine.UI;
using UnityEngine;
using PDDEditor.UI;
using PDDEditor.SceneManagment;
using System.IO;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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

    // Фильтры
    [SerializeField] private TMP_InputField _searchInput;
    [SerializeField] private Toggle _nameFilter;
    [SerializeField] private Toggle _creatingDateFilter;
    [SerializeField] private Toggle _editDateFilter;

    [SerializeField] private bool _nameFilterState = true;
    [SerializeField] private bool _creatingDateFilterState = false;
    [SerializeField] private bool _editDateFilterState = false;

    private void Awake()
    {
        _loadButton.onClick.AddListener(Context.Instance.LevelSystem.CreateNewScene);
        _exitButton.onClick.AddListener(delegate { Application.Quit(); });
        _importButton.onClick.AddListener(delegate
        {
            Context.Instance.AssetLoader.LoadAsset(PDDEditorWindows.ImportPreview);
            AssetContainer.Unload();
        });

        _scenesListButton.onClick.AddListener(ShowScenes);
        _scenesListButton_close.onClick.AddListener(HideScenes);
        _assetsManagerButton.onClick.AddListener(delegate { Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.AssetsManager); });
        _SettingsButton.onClick.AddListener(delegate { Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.MainSettingsWindow); });

        // Подписка на изменение состояния фильтров
        _nameFilter.onValueChanged.AddListener(delegate { _nameFilterState = true; });
        _nameFilter.onValueChanged.AddListener(delegate { _creatingDateFilterState = false; });
        _nameFilter.onValueChanged.AddListener(delegate { _editDateFilterState = false; });

        _creatingDateFilter.onValueChanged.AddListener(delegate { _nameFilterState = false; });
        _creatingDateFilter.onValueChanged.AddListener(delegate { _creatingDateFilterState = true; });
        _creatingDateFilter.onValueChanged.AddListener(delegate { _editDateFilterState = false; });

        _editDateFilter.onValueChanged.AddListener(delegate { _nameFilterState = false; });
        _editDateFilter.onValueChanged.AddListener(delegate { _creatingDateFilterState = false; });
        _editDateFilter.onValueChanged.AddListener(delegate { _editDateFilterState = true; });

        _nameFilter.onValueChanged.AddListener(OnToggle);
        _creatingDateFilter.onValueChanged.AddListener(OnToggle);
        _editDateFilter.onValueChanged.AddListener(OnToggle);

        _searchInput.onValueChanged.AddListener(_ => ShowScenes());  // Для обновления при изменении поиска
    }

    private void OnToggle(bool state)
    {
        ShowScenes();
    }

    private void ShowScenes()
    {
        _sceneView.SetActive(true);

        List<PDDScene> scenes = Context.Instance.LevelSystem.GetAllScenes();

        // Применение фильтров
        scenes = FilterScenes(scenes);

        // Очистка текущего списка перед отображением
        ClearSceneList();

        // Отображение отфильтрованных сцен
        foreach (PDDScene item in scenes)
        {
            Scene_Ticket ticket = Instantiate(_sceneTicket, _sceneContent);
            ticket.Name.text = item.Name;
            ticket.Path.text = item.Path;
            ticket.OpenButton.onClick.AddListener
            (
                delegate
                {
                    Context.Instance.LevelSystem.LoadScene(item.Path);
                }
            );
            ticket.CreatonDate.text = $"Создано: {item.CreatingDate}";
            ticket.EditDate.text = $"Изменено: {item.LastEditDate}";

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
        _sceneView.SetActive(false);
        ClearSceneList();
    }

    private void ClearSceneList()
    {
        for (int i = 0; i < _sceneContent.childCount; i++)
        {
            Destroy(_sceneContent.GetChild(i).gameObject);
        }
    }

    private List<PDDScene> FilterScenes(List<PDDScene> scenes)
    {
        // Фильтрация по имени (поиск по строке)
        if (!string.IsNullOrEmpty(_searchInput.text))
        {
            scenes = scenes.Where(scene => scene.Name.ToLower().Contains(_searchInput.text.ToLower())).ToList();
        }

        if (_nameFilterState)
        {
            if (_nameFilter.isOn)
            {
                scenes = scenes.OrderBy(scene => scene.Name).ToList();
            }
            else
            {
                scenes = scenes.OrderByDescending(scene => scene.Name).ToList();
            }
        }

        if (_creatingDateFilterState)
        {
            if (_creatingDateFilter.isOn)
            {
                scenes = scenes.OrderBy(scene => scene.CreatingDate).ToList();
            }
            else
            {
                scenes = scenes.OrderByDescending(scene => scene.CreatingDate).ToList();
            }
        }

        if (_editDateFilterState)
        {
            if (_editDateFilter.isOn)
            {
                scenes = scenes.OrderBy(scene => scene.LastEditDate).ToList();
            }
            else
            {
                scenes = scenes.OrderByDescending(scene => scene.LastEditDate).ToList();
            }
        }

        return scenes;
    }
}
