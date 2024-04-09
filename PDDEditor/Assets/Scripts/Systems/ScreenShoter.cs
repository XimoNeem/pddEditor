using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShoter
{
    private Texture2D _finalTexture;
    public Camera renderCamera => Camera.main;
    public float sizeMultiplayer = 0.9f;

    public void GetPreview(Action<Texture2D> callBack, int width, int height)
    {
        Render(false, false, "", "", width, height, callBack);
    }

    public void SaveRender(Action<Texture2D> callBack, int width, int height, string path, Slider slider, string name)
    {
        Render(true, true, name, path, width, height, callBack);
    }

    private void Render(bool finalRender, bool save, string name, string path, int width, int height, Action<Texture2D> callBack)
    {
        if (finalRender) { QualitySettings.SetQualityLevel(5); }

        RenderTexture newTex = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
        newTex.Create();

        renderCamera.targetTexture = newTex;
        renderCamera.Render();


        _finalTexture = toTexture2D(newTex);

        renderCamera.targetTexture = null;

        if (save)
        {
            SaveImage(name, path, callBack);
        }
        else
        {
            callBack.Invoke(_finalTexture);
        }
    }


    public void SaveImage(string name, string path, Action<Texture2D> callBack)
    {
        byte[] imageBytes;

        imageBytes = _finalTexture.EncodeToPNG();

        int byteValue = imageBytes.Length;
        string filePath = Path.Combine(path, name + ".png");

        File.WriteAllBytes(filePath, imageBytes);

        callBack.Invoke(_finalTexture);
    }

    public Texture2D toTexture2D(RenderTexture texture)
    {
        Texture2D tex = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
        RenderTexture.active = texture;
        tex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
