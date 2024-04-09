using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LocalAssetLoader : MonoBehaviour
{
    public LocalAssetProvider LoadAsset(string key)
    {
        LocalAssetProvider provider = new GameObject("AssetContainer").AddComponent<LocalAssetProvider>();
        provider.LoadAsset(key, null);

        return provider;
    }
    public LocalAssetProvider LoadAsset(string key, Action<AsyncOperationHandle<GameObject>> callback)
    {
        LocalAssetProvider provider = new GameObject("AssetContainer").AddComponent<LocalAssetProvider>();
        provider.LoadAsset(key, callback);

        return provider;
    }
}
