using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Activation_Window : WindowController
{
    [SerializeField] private TMP_InputField _keyInput;
    [SerializeField] private Button _activateButton;
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Image _activateImage;

    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;

    private string _key;
    private Color _inputStartColor;

    public override void OnEnable()
    {
        base.OnEnable();

        _activateButton.onClick.AddListener(TryActivate);
        _inputStartColor = _keyInput.image.color;
        _keyInput.onValueChanged.AddListener(SetKey);

        if (PlayerPrefs.HasKey("pddActivation"))
        {
            if (PlayerPrefs.GetString("pddActivation") == "TX9XD-98N7V-6WMQ6")
            {
                StartCoroutine(Activate());
            }
        }
    }

    private void SetKey(string key)
    {
        _key = key.ToLower();
    }

    private void TryActivate()
    {
        if (_key == "TX9XD-98N7V-6WMQ6".ToLower())
        {
            StartCoroutine(Activate());
            PlayerPrefs.SetString("pddActivation", "TX9XD-98N7V-6WMQ6");
            PlayerPrefs.Save();
        }
        else
        {
            Context.Instance.UIDrawer.TintImageByTime(_keyInput.image, Color.red, _inputStartColor);
            _messageText.text = "Неверный код активации";
            _messageText.color = _inactiveColor;
        }
    }

    private IEnumerator Activate()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.fixedDeltaTime;
            _activateImage.fillAmount = t;

            yield return new WaitForFixedUpdate();
        }

        _messageText.text = "Программа успешно активирована";
        _messageText.color = _activeColor;

        yield return new WaitForSeconds(1.5f);
        this.AssetContainer.Unload();
    }
}