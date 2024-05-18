using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDDEditor;
using PDDEditor.SceneManagment;
using PDDEditor.UI;

public class InputActions : MonoBehaviour
{
    public void ExitToMain()
    {
        Context.Instance.UIDrawer.UnloadAllWindows();
        Context.Instance.SceneLoader.LoadScene(PDDEditorScenes.Menu);
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.MainMenu);
    }
    public void ShowSceneSettings()
    {
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.SceneSettings);
    }
    public void ShowEditorSettings()
    {
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.EditorSettings);
    }

    public void ShowRender()
    {
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ScreenShot);
    }
}
