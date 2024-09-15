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
using PDDEditor.Paths;
using PDDEditor.Types;

public class ScreenShot_Window : WindowController
{
    [SerializeField] private RawImage _previewImage;
    [SerializeField] private AspectRatioFitter _previewFitter;
    [SerializeField] private TMP_InputField _widthInput, _heightInput, _pathInput, _nameInput;
    [SerializeField] private Button _renderButton;
    [SerializeField] private Button _pathButton;
    [SerializeField] private Slider _progressSlider;

    [SerializeField] private Transform _overlayItemsContent;
    [SerializeField] private Button _overlayItemTemplate;

    [SerializeField] private int _width = 1920;
    [SerializeField] private int _height = 1080;

    [Space]

    [SerializeField] private Button _selectCameraButton;

    [SerializeField] private Button _moveRightButton;
    [SerializeField] private Button _moveLeftButton;
    [SerializeField] private Button _moveUpButton;
    [SerializeField] private Button _moveDownButton;
    [SerializeField] private Button _moveForwardButton;
    [SerializeField] private Button _moveBackwardButton;

    [SerializeField] private Button _rotateRightButton;
    [SerializeField] private Button _rotateLeftButton;
    [SerializeField] private Button _rotateUpButton;
    [SerializeField] private Button _rotateDownButton;


    private const int _maxWidth = 4000;
    private const int _maxHeight = 2000;

    private string _path;
    private string _overlayImagePath;

    public float moveStep = 0.5f;  // Шаг перемещения камеры
    public float rotateStep = 5.0f; // Шаг вращения камеры
    private Transform parentTransform;

    //private Camera _camera;

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

        _selectCameraButton.onClick.AddListener(SelectCamera);

        if (_moveRightButton != null) _moveRightButton.onClick.AddListener(MoveRight);
        if (_moveLeftButton != null) _moveLeftButton.onClick.AddListener(MoveLeft);
        if (_moveUpButton != null) _moveUpButton.onClick.AddListener(MoveUp);
        if (_moveDownButton != null) _moveDownButton.onClick.AddListener(MoveDown);
        if (_moveForwardButton != null) _moveForwardButton.onClick.AddListener(MoveForward);
        if (_moveBackwardButton != null) _moveBackwardButton.onClick.AddListener(MoveBackward);

        if (_rotateRightButton != null) _rotateRightButton.onClick.AddListener(RotateRight);
        if (_rotateLeftButton != null) _rotateLeftButton.onClick.AddListener(RotateLeft);
        if (_rotateUpButton != null) _rotateUpButton.onClick.AddListener(RotateUp);
        if (_rotateDownButton != null) _rotateDownButton.onClick.AddListener(RotateDown);

        if (_moveRightButton != null) _moveRightButton.onClick.AddListener(RequestPreview);
        if (_moveLeftButton != null) _moveLeftButton.onClick.AddListener(RequestPreview);
        if (_moveUpButton != null) _moveUpButton.onClick.AddListener(RequestPreview);
        if (_moveDownButton != null) _moveDownButton.onClick.AddListener(RequestPreview);
        if (_moveForwardButton != null) _moveForwardButton.onClick.AddListener(RequestPreview);
        if (_moveBackwardButton != null) _moveBackwardButton.onClick.AddListener(RequestPreview);

        if (_rotateRightButton != null) _rotateRightButton.onClick.AddListener(RequestPreview);
        if (_rotateLeftButton != null) _rotateLeftButton.onClick.AddListener(RequestPreview);
        if (_rotateUpButton != null) _rotateUpButton.onClick.AddListener(RequestPreview);
        if (_rotateDownButton != null) _rotateDownButton.onClick.AddListener(RequestPreview);

        Camera renderCamera = Context.Instance?.ScreenShoter?.renderCamera;

        if (renderCamera == null)
        {
            Debug.LogError("Render Camera is not set.");
        }
        else
        {
            parentTransform = renderCamera.transform.parent;
        }

        CreateOverlayItems();
        RequestPreview();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        _widthInput.onEndEdit.RemoveListener(SetWidth);
        _heightInput.onEndEdit.RemoveListener(SetHeight);
    }

    private void SelectCamera()
    {
        Action<UtilityNode> action = OnCamereSelected;
        Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.UtilityPicker, action, UtilityType.Camera);
    }

    private void OnCamereSelected(UtilityNode node)
    {
        Context.Instance.ScreenShoter.renderCamera = node.GetComponentInChildren<Camera>();
        RequestPreview();
        Debug.Log("Done camera");

        Camera renderCamera = Context.Instance?.ScreenShoter?.renderCamera;

        if (renderCamera == null)
        {
            Debug.LogError("Render Camera is not set.");
        }
        else
        {
            parentTransform = renderCamera.transform.parent;
        }
    }

    private void CreateOverlayItems()
    {
        string path = Path.Combine(Application.persistentDataPath, PDDEditorPaths.OverlayImagesPath);

        string[] files = Directory.GetFiles(path, "*.png");

/*        foreach (Transform child in _overlayItemsContent)
        {
            Destroy(child.gameObject);
        }*/

        foreach (string file in files)
        {
            Button overlayItem = Instantiate(_overlayItemTemplate, _overlayItemsContent);

            Texture2D texture = LoadTexture(file);

            if (texture != null)
            {
                overlayItem.GetComponentInChildren<RawImage>().texture = texture;
                overlayItem.gameObject.SetActive(true);

                overlayItem.GetComponentInChildren<AspectRatioFitter>().aspectRatio = (float)texture.width / (float)texture.height;

                overlayItem.onClick.AddListener( delegate
                    {
                        SetOverlayPath(file);
                    });
            }
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

    public void SetOverlayPath(string path)
    {
        _overlayImagePath = path;
        RequestPreview();
    }

    private void RequestPreview()
    {
        Debug.Log(_width);
        Debug.Log(_height);

        Context.Instance.ScreenShoter.GetPreview(SetPreview, _width, _height, _overlayImagePath);
    }

    private void SaveRender()
    {
        if (Context.Instance.ScreenShoter.renderCamera == null)
        {
            Context.Instance.UIDrawer.TintImageByTime(_selectCameraButton.image, Color.red, _selectCameraButton.image.color);
            return;
        }
        Context.Instance.ScreenShoter.SaveRender(OnRenderFinish, _width, _height, _path, _progressSlider, _nameInput.text, _overlayImagePath);
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
        _previewFitter.aspectRatio = (float)_width / (float)_height;

        Debug.Log($"{_width} / {_height} = {_width / _height}");
    }

    // Приватные методы перемещения камеры

    private void MoveRight()
    {
        if (parentTransform == null) return;
        parentTransform.position += parentTransform.right * moveStep;
    }

    private void MoveLeft()
    {
        if (parentTransform == null) return;
        parentTransform.position -= parentTransform.right * moveStep;
    }

    private void MoveUp()
    {
        if (parentTransform == null) return;
        parentTransform.position += parentTransform.up * moveStep;
    }

    private void MoveDown()
    {
        if (parentTransform == null) return;
        parentTransform.position -= parentTransform.up * moveStep;
    }

    private void MoveForward()
    {
        if (parentTransform == null) return;
        parentTransform.position += parentTransform.forward * moveStep;
    }

    private void MoveBackward()
    {
        if (parentTransform == null) return;
        parentTransform.position -= parentTransform.forward * moveStep;
    }

    // Приватные методы вращения родительского объекта

    private void RotateRight()
    {
        if (parentTransform == null) return;
        parentTransform.Rotate(Vector3.up, rotateStep, Space.World);
    }

    private void RotateLeft()
    {
        if (parentTransform == null) return;
        parentTransform.Rotate(Vector3.up, -rotateStep, Space.World);
    }

    private void RotateUp()
    {
        if (parentTransform == null) return;
        parentTransform.Rotate(Vector3.right, -rotateStep, Space.Self);
    }

    private void RotateDown()
    {
        if (parentTransform == null) return;
        parentTransform.Rotate(Vector3.right, rotateStep, Space.Self);
    }
}
