using UnityEngine.UI;
using UnityEngine;
using System.Diagnostics;


public class WindowController : LoadedAsset
{
    [SerializeField] private bool _blockInput;
    [SerializeField] private Button _blocker;

    public virtual void OnEnable()
    {
        Context.Instance.InputSystem.InputBlocked = _blockInput;
        if (_blocker != null) {_blocker.onClick.AddListener(delegate { AssetContainer.Unload(); });}
    }

    public virtual void OnDisable()
    {
        Context.Instance.InputSystem.InputBlocked = false;
    }

    public override void Initialize(params object[] values)
    {
        base.Initialize(values);
    }
}