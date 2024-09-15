using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPicker_Window : WindowController
{
    [SerializeField] private Button _pickButton;
    [SerializeField] private Slider _redSlider;   // Ползунок для красного канала
    [SerializeField] private Slider _greenSlider; // Ползунок для зеленого канала
    [SerializeField] private Slider _blueSlider;  // Ползунок для синего канала
    [SerializeField] private Slider _alphaSlider; // Ползунок для альфа канала
    [SerializeField] private Image _gradientImage; // Изображение для градиента
    [SerializeField] private Image _colorDisplay; // Отображение выбранного цвета

    private Color _color;
    private Action<Color> _callBack;

    public override void Initialize(params object[] values)
    {
        base.Initialize(values);

        if (values[0].GetType() != typeof(Action<Color>))
        {
            Debug.LogError("Need object of type <Action<Color>> for initialization");
            return;
        }

        _callBack = (Action<Color>)values[0];
    }

    public override void OnEnable()
    {
        base.OnEnable();

        _pickButton.onClick.AddListener(PickColor);
        _redSlider.onValueChanged.AddListener(UpdateColorFromSliders);
        _greenSlider.onValueChanged.AddListener(UpdateColorFromSliders);
        _blueSlider.onValueChanged.AddListener(UpdateColorFromSliders);
        _alphaSlider.onValueChanged.AddListener(UpdateColorFromSliders);

        // Начальное значение
        SetInitialColor(new Color(1, 1, 1, 1)); // Начальный цвет - белый с полной непрозрачностью
    }

    public override void OnDisable()
    {
        base.OnDisable();

        _pickButton.onClick.RemoveListener(PickColor);
        _redSlider.onValueChanged.RemoveListener(UpdateColorFromSliders);
        _greenSlider.onValueChanged.RemoveListener(UpdateColorFromSliders);
        _blueSlider.onValueChanged.RemoveListener(UpdateColorFromSliders);
        _alphaSlider.onValueChanged.RemoveListener(UpdateColorFromSliders);
    }

    private void SetInitialColor(Color initialColor)
    {
        _redSlider.value = initialColor.r;
        _greenSlider.value = initialColor.g;
        _blueSlider.value = initialColor.b;
        _alphaSlider.value = initialColor.a;

        SelectColor(initialColor);
    }

    private void UpdateColorFromSliders(float value)
    {
        _color = new Color(_redSlider.value, _greenSlider.value, _blueSlider.value, _alphaSlider.value);
        _colorDisplay.color = _color; // Обновляем отображение выбранного цвета
    }

    private void SelectColor(Color color)
    {
        _color = color;
        _colorDisplay.color = _color; // Обновляем отображение выбранного цвета
        UpdateSliders();
    }

    private void UpdateSliders()
    {
        _redSlider.SetValueWithoutNotify(_color.r);   // Обновляем значение ползунка без вызова дополнительных событий
        _greenSlider.SetValueWithoutNotify(_color.g); // для предотвращения зацикливания обновлений
        _blueSlider.SetValueWithoutNotify(_color.b);
        _alphaSlider.SetValueWithoutNotify(_color.a);
    }

    private void PickColor()
    {
        _callBack?.Invoke(_color);
        this.AssetContainer.Unload();
    }

    public void OnGradientClickWrapper(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;

        // Преобразование позиции клика в локальные координаты градиента
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_gradientImage.rectTransform, pointerData.position, pointerData.pressEventCamera, out localPosition);

        OnGradientClick(localPosition);
    }

    // Метод для обработки кликов по градиенту
    public void OnGradientClick(Vector2 position)
    {
        // Получаем текстуру, на которой нужно выбирать цвет
        Texture2D texture = _gradientImage.sprite.texture;

        // Преобразуем позицию клика относительно изображения в локальные координаты текстуры
        Vector2 normalizedPosition = new Vector2(
            (position.x - _gradientImage.rectTransform.rect.x) / _gradientImage.rectTransform.rect.width,
            (position.y - _gradientImage.rectTransform.rect.y) / _gradientImage.rectTransform.rect.height
        );

        // Переводим нормализованные координаты в координаты пикселей текстуры
        Vector2 texturePosition = new Vector2(
            normalizedPosition.x * texture.width,
            normalizedPosition.y * texture.height
        );

        // Отражаем Y-координату, поскольку система координат UI и текстуры различаются (верхний левый угол против нижнего левого)
        texturePosition.y = texture.height - texturePosition.y;

        // Защита от выхода за пределы текстуры
        texturePosition.x = Mathf.Clamp(texturePosition.x, 0, texture.width - 1);
        texturePosition.y = Mathf.Clamp(texturePosition.y, 0, texture.height - 1);

        // Получаем цвет из текстуры
        Color gradientColor = texture.GetPixel((int)texturePosition.x, (int)texturePosition.y);
        gradientColor.a = _alphaSlider.value; // Применяем текущую альфу
        SelectColor(gradientColor);
    }
}
