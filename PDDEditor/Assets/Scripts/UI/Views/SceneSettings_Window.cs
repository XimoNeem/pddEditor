using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class SceneSettings_Window : WindowController
{
    [SerializeField] private Transform _sunlight;
    [SerializeField] private Slider _sunRotationSlider;
    [SerializeField] private Slider _temperatureSlider;
    [SerializeField] private Slider _cloudDensitySlider;
    [SerializeField] private Slider _fogDensitySlider;
    [SerializeField] private Slider _lightIntensitySlider;
    [SerializeField] private Slider _fogIntensitySlider;

    private Volume _volume;

    public override void OnEnable()
    {
        base.OnEnable();

        _sunlight = GameObject.FindGameObjectWithTag("Sunlight").transform;
        _volume = GameObject.FindGameObjectWithTag("SceneVolume").GetComponent<Volume>();

        UpdateSunRotationSlider(_sunlight.eulerAngles.x);
        UpdateCloudDensitySlider(_volume.profile.TryGet(out VolumetricClouds clouds) ? clouds.densityMultiplier.value : 0);
        UpdateFogDensitySlider(_volume.profile.TryGet(out Fog fog) ? fog.baseHeight.value : 0);
        UpdateLightIntensitySlider(_sunlight.GetComponent<HDAdditionalLightData>().intensity);
        UpdateFogIntensitySlider(_volume.profile.TryGet(out Fog fog2) ? fog2.globalLightProbeDimmer.value : 0);


        _sunRotationSlider.onValueChanged.AddListener(UpdateSunRotation);
        _temperatureSlider.onValueChanged.AddListener(UpdateSunTemperature);
        _cloudDensitySlider.onValueChanged.AddListener(UpdateCloudDensity);
        _fogDensitySlider.onValueChanged.AddListener(UpdateFogDensity);
        _lightIntensitySlider.onValueChanged.AddListener(UpdateLightIntensity);
        _fogIntensitySlider.onValueChanged.AddListener(UpdateFogIntensity);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        _sunRotationSlider.onValueChanged.RemoveListener(UpdateSunRotation);
        _temperatureSlider.onValueChanged.RemoveListener(UpdateSunTemperature);
        _cloudDensitySlider.onValueChanged.RemoveListener(UpdateCloudDensity);
        _fogDensitySlider.onValueChanged.RemoveListener(UpdateFogDensity);
        _lightIntensitySlider.onValueChanged.RemoveListener(UpdateLightIntensity);
        _fogIntensitySlider.onValueChanged.RemoveListener(UpdateFogIntensity);
    }

    private void UpdateSunRotation(float value)
    {
        _sunlight.transform.eulerAngles = new Vector3(value, _sunlight.transform.eulerAngles.y, _sunlight.transform.eulerAngles.z);
    }

    private void UpdateSunTemperature(float value)
    {
        _sunlight.GetComponent<HDAdditionalLightData>().SetColor(Mathf.CorrelatedColorTemperatureToRGB(value));
    }

    private void UpdateCloudDensity(float value)
    {
        _volume.profile.TryGet(out VolumetricClouds clouds);
        clouds.densityMultiplier.value = value;
    }

    private void UpdateFogDensity(float value)
    {
        _volume.profile.TryGet(out Fog fog);
        fog.baseHeight.value = value;
    }

    private void UpdateLightIntensity(float value)
    {
        _sunlight.GetComponent<HDAdditionalLightData>().intensity = value;
    }

    private void UpdateFogIntensity(float value)
    {
        _volume.profile.TryGet(out Fog fog);
        fog.globalLightProbeDimmer.value = value;
    }

    private void UpdateSunRotationSlider(float value)
    {
        _sunRotationSlider.value = value;
    }

    private void UpdateSunTemperatureSlider(float value)
    {
        _temperatureSlider.value = value;
    }

    private void UpdateCloudDensitySlider(float value)
    {
        _cloudDensitySlider.value = value;
    }

    private void UpdateFogDensitySlider(float value)
    {
        _fogDensitySlider.value = value;
    }

    private void UpdateLightIntensitySlider(float value)
    {
        _lightIntensitySlider.value = value;
    }

    private void UpdateFogIntensitySlider(float value)
    {
        _fogIntensitySlider.value = value;
    }
}
