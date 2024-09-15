using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Measure_Window : WindowController
{
    [SerializeField] private TMP_Text _measureText; // Поле для отображения текста измеренного расстояния
    [SerializeField] private LineRenderer _lineRenderer; // Ссылка на LineRenderer

    private Vector3 _firstPoint; // Первая точка рейкаста
    private Vector3 _secondPoint; // Вторая точка рейкаста
    private bool _isFirstPointSet = false; // Флаг для проверки, установлена ли первая точка

    private void Start()
    {
        _lineRenderer = this.AddComponent<LineRenderer>();

        _lineRenderer.positionCount = 2; // Устанавливаем количество точек линии
        _lineRenderer.startWidth = 0.05f; // Устанавливаем начальную толщину линии
        _lineRenderer.endWidth = 0.05f; // Устанавливаем конечную толщину линии
        _lineRenderer.enabled = false; // Изначально отключаем LineRenderer
    }

    private void Update()
    {
        // Проверка нажатия левой кнопки мыши
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Проверяем, попадает ли рейкаст в объект в сцене
            if (Physics.Raycast(ray, out hit))
            {
                if (!_isFirstPointSet)
                {
                    // Если первая точка не установлена, устанавливаем её
                    _firstPoint = hit.point;
                    _isFirstPointSet = true;
                    _lineRenderer.enabled = true; // Включаем LineRenderer
                    _lineRenderer.SetPosition(0, _firstPoint); // Устанавливаем первую точку в LineRenderer
                }
                else
                {
                    // Если первая точка уже установлена, устанавливаем вторую точку
                    _secondPoint = hit.point;
                    _isFirstPointSet = false; // Сбрасываем флаг для повторного измерения
                    _lineRenderer.SetPosition(1, _secondPoint); // Устанавливаем вторую точку в LineRenderer

                    // Вычисляем и отображаем расстояние
                    float distance = Vector3.Distance(_firstPoint, _secondPoint);
                    _measureText.text = $"Distance: {distance:F2} units";
                }
            }
        }

        // Если первая точка установлена, обновляем вторую точку по текущей позиции рейкаста
        if (_isFirstPointSet)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                _secondPoint = hit.point;
                _lineRenderer.SetPosition(1, _secondPoint);

                // Обновляем отображаемое расстояние в реальном времени
                float distance = Vector3.Distance(_firstPoint, _secondPoint);
                _measureText.text = $"{distance:F2} m";
            }
        }
    }
}
