using UnityEngine.UI;
using UnityEngine;
using PDDEditor.UI;

public class BottomBar_Window : WindowController
{
    [SerializeField] private Button _itemsListButton;
    [SerializeField] private Button _screenShotButton;

    private void Awake()
    {
        _itemsListButton.onClick.AddListener(delegate { Context.Instance.LevelSystem.RequestObject(); });
        _screenShotButton.onClick.AddListener(delegate { Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ScreenShot); });
    }
}
