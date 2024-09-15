using PDDEditor.Paths;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class TextureSetting : IItemSetting
{
    [SerializeField] private string name;
    public string Name { get { return name; } set { name = value; } }
    public int ID { get; set; }

    public MeshRenderer[] RenderObjects;
    public TextureSettingData Value;

    public void Set()
    {
        if (Value == null) { return; }

        foreach (var item in RenderObjects)
        {
            string path = Value.GetTexturePath();
            Texture2D texture = PDDUtilities.LoadTextureFromFile(path);

            Debug.Log(texture);
            Debug.Log(path);

            if (texture != null)
            {
                item.material.mainTexture = texture;
            }
            else
            {
                Context.Instance.Logger.LogError($"No image found by path {path}");
            }
        }
    }
}

[System.Serializable]
public class TextureSettingData
{
    public string TextureDirectory;
    public string TextureName;

    public string GetTexturePath()
    {
        string result = "";

        result = Path.Combine((Application.persistentDataPath + PDDEditorPaths.TexturesPath), TextureDirectory, TextureName);

        return result;
    }
}