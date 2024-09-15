using UnityEngine.UI;
using UnityEngine;
using PDDEditor.UI;
using UnityEngine.Rendering.HighDefinition;

public class BottomBar_Window : WindowController
{
    [SerializeField] private Button _itemsListButton;
    [SerializeField] private Button _screenShotButton;
    [SerializeField] private Toggle _fxButton;

    private void Awake()
    {
        _itemsListButton.onClick.AddListener(delegate { Context.Instance.LevelSystem.RequestObject(); });
        _screenShotButton.onClick.AddListener(delegate { Context.Instance.UIDrawer.ShowWindow(PDDEditorWindows.ScreenShot); });
        _fxButton.onValueChanged.AddListener(
                delegate
                {
                    Camera targetCamera = GameObject.Find("MainCamera")?.GetComponent<Camera>();

                    if (targetCamera != null)
                    {
                        HDAdditionalCameraData hdCameraData = targetCamera.GetComponent<HDAdditionalCameraData>();

                        if (hdCameraData != null)
                        {
                            hdCameraData.antialiasing = _fxButton.isOn ? HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing : HDAdditionalCameraData.AntialiasingMode.None;

                            if (_fxButton.isOn)
                            {
                                hdCameraData.volumeLayerMask = -1;
                            }
                            else
                            {
                                hdCameraData.volumeLayerMask = 0;
                                GameObject.FindGameObjectWithTag("Sunlight").GetComponent<HDAdditionalLightData>().intensity = 5f;
                            }

                            hdCameraData.dithering = _fxButton.isOn;

                        }
                        else
                        {
                            Context.Instance.Logger.LogWarning($"HDAdditionalCameraData not found on camera");
                        }
                    }
                    else
                    {
                        Context.Instance.Logger.LogError($"Camera with name not found on the scene.");
                    }
                }
            );
    }
}
