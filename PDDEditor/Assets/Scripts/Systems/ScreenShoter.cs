using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShoter
{
    private Texture2D _finalTexture;
    public Camera renderCamera;
    public float sizeMultiplayer = 0.9f;

    public void GetPreview(Action<Texture2D> callBack, int width, int height, string overlayPath)
    {
        Render(false, false, "", "", width, height, callBack, overlayPath);
    }

    public void GetPreview(Action<Texture2D> callBack, int width, int height, string overlayPath, Camera camera)
    {
        if (camera == null)
        {
            renderCamera = camera;
        }
        Render(false, false, "", "", width, height, callBack, overlayPath);
    }

    public void SaveRender(Action<Texture2D> callBack, int width, int height, string path, Slider slider, string name, string overlayPath)
    {
        Render(true, true, name, path, width, height, callBack, overlayPath);
    }

    private void Render(bool finalRender, bool save, string name, string path, int width, int height, Action<Texture2D> callBack, string overlayURL)
    {
        if (finalRender)
        {
            QualitySettings.SetQualityLevel(5);
        }

        RenderTexture newTex = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
        newTex.Create();

        if (renderCamera == null)
        {
            renderCamera = GameObject.Find("RenderCamera").GetComponent<Camera>();
        }

        Debug.Log(renderCamera);

        renderCamera.targetTexture = newTex;
        renderCamera.Render();

        _finalTexture = toTexture2D(newTex);

        renderCamera.targetTexture = null;
        renderCamera.enabled = false;

        if (!string.IsNullOrEmpty(overlayURL))
        {
            Texture2D overlayTexture = LoadTexture(overlayURL);
            if (overlayTexture != null)
            {
                ApplyOverlay(_finalTexture, overlayTexture);
            }
        }

        if (save)
        {
            SaveImage(name, path, callBack);
        }
        else
        {
            callBack.Invoke(_finalTexture);
        }
    }

    private Texture2D LoadTexture(string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                return texture;
            }
        }
        return null;
    }

    private void ApplyOverlay(Texture2D baseTexture, Texture2D overlayTexture)
    {
        int baseWidth = baseTexture.width;
        int baseHeight = baseTexture.height;

        for (int x = 0; x < baseWidth; x++)
        {
            for (int y = 0; y < baseHeight; y++)
            {
                float u = (float)x / baseWidth;
                float v = (float)y / baseHeight;

                int overlayX = Mathf.FloorToInt(u * overlayTexture.width);
                int overlayY = Mathf.FloorToInt(v * overlayTexture.height);

                Color overlayPixel = overlayTexture.GetPixel(overlayX, overlayY);

                Color basePixel = baseTexture.GetPixel(x, y);

                Color finalPixel = Color.Lerp(basePixel, overlayPixel, overlayPixel.a);
                baseTexture.SetPixel(x, y, finalPixel);
            }
        }

        baseTexture.Apply();
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
