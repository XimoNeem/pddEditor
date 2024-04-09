using UnityEngine;
using PDDEditor.UI;

public class Test : MonoBehaviour
{
    public string Tes = "sfgdhf";
    private void OnMouseEnter()
    {
        Context.Instance.UIDrawer.ShowHint(Tes);
    }

    private void OnMouseExit()
    {
        Context.Instance.UIDrawer.HideHint();
        Context.Instance.EventManager.OnSceneLoaded.Invoke();
    }

    private void OnMouseDown()
    {
        Context.Instance.AssetLoader.LoadAsset(PDDEditorWindows.ObjectSettings);
    }
}
