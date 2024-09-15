using PDDEditor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTimeSystem : MonoBehaviour
{
    public bool RunTime = false;
    public bool Pause = false;

    private List<PDDNode> _items = new List<PDDNode>();
    private List<UtilityNode> _utils = new List<UtilityNode>();
    private GameObject _mainCamera;

    private void Update()
    {
        if (!RunTime) { return; }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause = !Pause;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetRunTime(false);
        }
    }

    private void FixedUpdate()
    {
        if (!RunTime) { return; }
        if (!Pause)
        {
            foreach (PDDNode node in _items)
            {
                node.Item.SetRuntimeChanges();
            }
            foreach (UtilityNode util in _utils)
            {
                util.Item.SetRuntimeChanges();
            }
        }
    }

    public void SetRunTime(bool state)
    {
        if (Context.Instance.ScreenShoter.renderCamera == null)
        {
            Context.Instance.Logger.LogWarning("Set up camera first!");
            return;
        }
        RunTime = state;

        ToggleMainCamera(!state);
        if (state)
        {
            Context.Instance.UIDrawer.UnloadAllWindows();
            _items = Context.Instance.LevelSystem.GetAllItems();
            _utils = Context.Instance.LevelSystem.GetAllUtils();
        }
        else
        {
            Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.MainTopBar);
            Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.MainButtomBar);
            Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.DebugLayer);
        }
    }

    private void ToggleMainCamera(bool state)
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        _mainCamera.SetActive(state);

        if (Context.Instance.ScreenShoter.renderCamera != null)
        {
            Context.Instance.ScreenShoter.renderCamera.enabled = !state;
        }
    }
}