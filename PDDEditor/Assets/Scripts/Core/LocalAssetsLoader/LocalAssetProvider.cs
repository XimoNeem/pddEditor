using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class LocalAssetProvider : MonoBehaviour
{
    private string _key;
    private Action<AsyncOperationHandle<GameObject>> _callback;
    private AsyncOperationHandle<GameObject> _operation;
    public LoadedAsset CachedObject;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        FindObjectOfType<UIDrawer>()._loadedProviders.Add(this); //TODO переписать
    }

    public void LoadAsset(string key, Action<AsyncOperationHandle<GameObject>> callback)
    {
        _key = key;
        _callback = callback;
        StartCoroutine(Load());
    }

    public void Unload()
    {
        //FindObjectOfType<UIDrawer>()._loadedProviders.Remove(this); //TODO переписать

        Debug.Log($"Unload {this.name}");
        Addressables.Release(_operation);
        Destroy(this.gameObject);
    }

    private IEnumerator Load()
    {
        _operation = Addressables.InstantiateAsync(_key, this.transform);
        yield return _operation;

        if (_operation.Status == AsyncOperationStatus.Succeeded)
        {
            if (_operation.Result.TryGetComponent(out LoadedAsset asset))
            {
                asset.AssetContainer = this;
                CachedObject = asset;

                if (_callback != null) { _callback.Invoke(_operation); }

            }
            else 
            {
                Debug.LogError("Loaded assets must be typeof <LoadedAsset>");
                yield break;
            }
        }
    }
}