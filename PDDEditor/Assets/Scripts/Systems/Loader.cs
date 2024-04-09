using Unity.VisualScripting;
using UnityEngine;
using PDDEditor.UI;
using PDDEditor.SceneManagment;

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
        Context.Instance.AssetLoader.LoadAsset(PDDEditorWindows.MainMenu);
        

        Debug.Log("Loader destroyed");
    }
}
