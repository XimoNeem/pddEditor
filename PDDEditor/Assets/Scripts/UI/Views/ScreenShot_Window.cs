using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.IO;
using PDDEditor.UI;
using System;

public class ScreenShot_Window : WindowController
{
    [SerializeField] private RawImage _previewImage;
    [SerializeField] private AspectRatioFitter _previewFitter;
    [SerializeField] private TMP_InputField _widthInput, _heightInput, _pathInput, _nameInput;
    [SerializeField] private Button _renderButton;
    [SerializeField] private Button _pathButton;
    [SerializeField] private Slider _progressSlider;

    [SerializeField] private int _width = 1920;
    [SerializeField] private int _height = 1080;

    private const int _maxWidth = 4000;
    private const int _maxHeight = 2000;

    private string _path;

    public override void OnEnable()
    {
        base.OnEnable();

        _widthInput.text = _width.ToString();
        _heightInput.text = _height.ToString();

        _widthInput.onEndEdit.AddListener(SetWidth);
        _heightInput.onEndEdit.AddListener(SetHeight);
        _renderButton.onClick.AddListener(SaveRender);
        _pathButton.onClick.AddListener(SelectPath);

        _path = Application.dataPath;
        _pathInput.text = _path;

        RequestPreview();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        _widthInput.onEndEdit.RemoveListener(SetWidth);
        _heightInput.onEndEdit.RemoveListener(SetHeight);
    }

    private void RequestPreview()
    {
        Debug.Log(_width);
        Debug.Log(_height);

        Context.Instance.ScreenShoter.GetPreview(SetPreview, _width, _height);
    }

    private void SaveRender()
    {
        Context.Instance.ScreenShoter.SaveRender(OnRenderFinish, _width, _height, _path, _progressSlider, _nameInput.text);
    }

    private void OnRenderFinish(Texture2D tex)
    {
        
    }

    private void SelectPath()
    {
        Action<FileSystemInfo> action = SetPath;
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.FilePicker, action, FileType.Dir);
    }

    private void SetPath(string path)
    {
        
    }

    private void SetPath(FileSystemInfo path) 
    {
        _path = path.FullName;
        _pathInput.text = _path;
    }

    private void SetWidth(string value)
    {
        try
        {
            _width = int.Parse(value);
            _width = Mathf.Clamp(_width, 0, _maxWidth);
            _widthInput.text = _width.ToString();
            RequestPreview();
        }
        catch (System.Exception)
        {
            _widthInput.text = _width.ToString();
            throw;
        }
    }

    private void SetHeight(string value)
    {
        try
        {
            _height = int.Parse(value);
            _height = Mathf.Clamp(_height, 0, _maxHeight);
            _heightInput.text = _height.ToString();
            RequestPreview();
        }
        catch (System.Exception)
        {
            _heightInput.text = _height.ToString();
            throw;
        }
    }

    private void SetPreview(Texture2D preview) 
    {
        _previewImage.texture = preview;
        _previewFitter.aspectRatio = preview.width / preview.height;
    }
}
