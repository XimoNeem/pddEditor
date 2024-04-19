using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleLoader : MonoBehaviour
{
    public string assetBundleURL; // URL, ������ ����������� AssetBundle

    void Start()
    {
        StartCoroutine(LoadAssetBundle());
    }

    IEnumerator LoadAssetBundle()
    {
        // ��������� AssetBundle �� ���������� URL
        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(assetBundleURL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load AssetBundle: " + www.error);
                yield break;
            }

            // �������� ����������� AssetBundle
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

            // ��������� ������ �� AssetBundle (����� ��� ������� - "PrefabName")
            GameObject[] prefab = bundle.LoadAllAssets<GameObject>();

            // ������� ��������� ������� � �����
            foreach (var item in prefab)
            {
                Instantiate(item, Vector3.zero, Quaternion.identity);
            }

            // ����������� ������� AssetBundle ����� �������������
            bundle.Unload(false);
        }
    }
}
