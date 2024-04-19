using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleLoader : MonoBehaviour
{
    public string assetBundleURL; // URL, откуда загружается AssetBundle

    void Start()
    {
        StartCoroutine(LoadAssetBundle());
    }

    IEnumerator LoadAssetBundle()
    {
        // Загружаем AssetBundle из указанного URL
        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(assetBundleURL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load AssetBundle: " + www.error);
                yield break;
            }

            // Получаем загруженный AssetBundle
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

            // Извлекаем префаб из AssetBundle (здесь имя префаба - "PrefabName")
            GameObject[] prefab = bundle.LoadAllAssets<GameObject>();

            // Создаем экземпляр префаба в сцене
            foreach (var item in prefab)
            {
                Instantiate(item, Vector3.zero, Quaternion.identity);
            }

            // Освобождаем ресурсы AssetBundle после использования
            bundle.Unload(false);
        }
    }
}
