using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDDEditor;
using PDDEditor.SceneManagment;
using PDDEditor.UI;
using System;
using Unity.VisualScripting;

public class InputActions : MonoBehaviour
{
    public void ExitToMain()
    {
        Context.Instance.LevelSystem.SaveScene();
        Context.Instance.UIDrawer.UnloadAllWindows();
        Context.Instance.SceneLoader.LoadScene(PDDEditorScenes.Menu);
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.MainMenu);
    }
    public void SaveScene()
    {
        Context.Instance.LevelSystem.SaveScene();
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

    public void CopyObject()
    {
        Context.Instance.InputSystem.CopyNode();
    }

    public void PasteObject()
    {
        Context.Instance.InputSystem.PasteNode();
    }

    public void DeleteObject()
    {
        Context.Instance.InputSystem.DeleteNode();
    }

    public void DublicateObject()
    {
        Context.Instance.InputSystem.DublicateNode();
    }

    public void CreateObject()
    {
        Action<string> action = Context.Instance.AssetRegister.LoadAsset;
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ItemsList, action);
    }

    public void CreateSpline()
    {
        Context.Instance.LevelSystem.CreateUtility(Resources.Load<PDDUtility>("PDDSpline"), Resources.Load<PDDSpline>("SplineNode"));
    }

    public void CreateCamera()
    {
        Context.Instance.LevelSystem.CreateUtility(Resources.Load<PDDUtility>("PDDCamera"));
    }

    public void StartRunTime()
    {
        Context.Instance.RunTimeSystem.SetRunTime(true);
    }
    public void ShowInfo()
    {
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.InfoWindow);
    }

    public void ShowMeasure()
    {
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.MeasureWindow);
    }

    public void ShowList()
    {
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ItemsList);
    }
}
