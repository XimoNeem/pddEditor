using System.IO;
using UnityEngine;
using PDDEditor.Paths;
using PDDEditor.Types;
using System;
using PDDEditor.Assets;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class AssetRegister : MonoBehaviour
{
    private string _dataPath;
    private Action<PDDItem> _cachedAction;

    private void Start()
    {
        _dataPath = Application.persistentDataPath + PDDEditorPaths.AssetsPath;

        PDDUtilities.CreateDirectoryIfNotExists(_dataPath);
        //ObjectType[] elements = (ObjectType[])Enum.GetValues(typeof(ObjectType));
        string[] elementStrings = PDDObjectTypes.GetTypes();
        foreach (var item in elementStrings)
        {
            PDDUtilities.CreateDirectoryIfNotExists(Path.Combine(_dataPath, item));
        }

        Debug.Log($"Created paths {_dataPath}");
    }

    public bool Import(string originalPath, string type, string name, Camera previewCamera)
    {
        string newPath = Path.Combine(_dataPath, type, name);
        if (PDDUtilities.CopyFile(originalPath, newPath))
        {
            Context.Instance.Logger.Log($"Asset {name} imported");

            // Создаем превью для ассета
            string previewPath = GetPreviewPath(newPath);
            CreatePreview(previewCamera, previewPath);

            return true;
        }
        else
        {
            return false;
        }
    }

    public PDDAssetData[] GetAssets(string type)
    {
        string assetsPath = Path.Combine(_dataPath, type);

        if (!Directory.Exists(assetsPath))
        {
            Context.Instance.Logger.LogError($"Path {assetsPath} does not exist.");
            return Array.Empty<PDDAssetData>();
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(assetsPath);
        FileInfo[] files = directoryInfo.GetFiles();
        List<PDDAssetData> assets = new List<PDDAssetData>();

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].FullName.EndsWith(".png")) { continue; }
            assets.Add(new PDDAssetData
            (
                files[i].FullName,
                GetPreviewPath(files[i].FullName),
                Path.GetFileNameWithoutExtension(files[i].FullName)
            ));
        }

        return assets.ToArray();
    }

    public void DeleteAsset(string path)
    {
        bool fileDeleted = PDDUtilities.DeleteFile(path);

        string previewPath = GetPreviewPath(path);
        bool previewDeleted = PDDUtilities.DeleteFile(previewPath);

        if (fileDeleted && previewDeleted)
        {
            Context.Instance.Logger.Log($"Deleted asset and its preview at {path}");
        }
        else if (fileDeleted)
        {
            Context.Instance.Logger.LogWarning($"Deleted asset, but preview was not found or could not be deleted at {previewPath}");
        }
        else
        {
            Context.Instance.Logger.LogError($"Failed to delete asset at {path}");
        }
    }

    private string GetPreviewPath(string assetPath)
    {
        string directory = Path.GetDirectoryName(assetPath);
        string filenameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
        return Path.Combine(directory, filenameWithoutExtension + ".png");
    }

    public void LoadAsset(string path)
    {
        _cachedAction = Context.Instance.LevelSystem.CreateObject;
        StartCoroutine(Load(path));
    }

    private IEnumerator Load(string path)
    {
        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(path))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load AssetBundle: " + www.error);
                yield break;
            }
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            GameObject[] prefab = bundle.LoadAllAssets<GameObject>();
            foreach (var item in prefab)
            {
                if (item.TryGetComponent<PDDItem>(out PDDItem pddItem))
                {
                    pddItem.AssetPath = path;
                    _cachedAction(pddItem);
                }
                else { Context.Instance.Logger.LogError("Imported asset should have <PDDItem> component"); }
            }
            bundle.Unload(false);
        }
    }

    private void CreatePreview(Camera camera, string previewPath)
    {
        if (camera == null)
        {
            Context.Instance.Logger.LogError("Preview camera is not assigned.");
            return;
        }

        RenderTexture originalTexure = camera.targetTexture;

        // Настройка рендера камеры
        RenderTexture renderTexture = new RenderTexture(256, 256, 16);
        camera.targetTexture = renderTexture;
        camera.Render();

        // Создание текстуры для сохранения снимка
        //camera.enabled = true;
        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        // Сохранение снимка в файл
        byte[] bytes = screenshot.EncodeToPNG();
        try
        {
            File.WriteAllBytes(previewPath, bytes);
            Context.Instance.Logger.Log($"Preview created at {previewPath}");
        }
        catch (Exception ex)
        {
            Context.Instance.Logger.LogError("Error creating preview: " + ex.Message);
        }

        // Освобождение ресурсов
        camera.targetTexture = originalTexure;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(screenshot);
        //camera.enabled = false;
    }
}

