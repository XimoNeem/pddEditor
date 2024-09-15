using Unity.VisualScripting;
using UnityEngine;
using PDDEditor.UI;
using PDDEditor.SceneManagment;
using System.Runtime.ConstrainedExecution;

public class Loader : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        this.AddComponent<Context>().Initialize(OnInitialized);
    }

    private void OnInitialized()
    {
        Destroy(this);

        Context.Instance.SceneLoader.LoadScene(PDDEditorScenes.Menu);

        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.DebugLayer);
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.MainMenu);
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ActivationWindow);


        Context.Instance.Logger.Log("Loader destroyed");
    }
}
