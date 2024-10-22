﻿using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeFlyCamera : MonoBehaviour
{
    #region UI


    private bool _active => !Context.Instance.InputSystem.InputBlocked;

    [Space]

    [SerializeField]
    private bool _enableRotation = true;

    private float _mouseSense => Context.Instance.EditorBase.EditorSettings.MouseSensivity;

    [Space]

    [SerializeField]
    private float _maxHeight = 100;

    [SerializeField]
    private float _minHeight = 2;

    [Space]

    [SerializeField]
    private bool _enableTranslation = true;

    [SerializeField]
    private float _translationSpeed = 55f;

    [Space]

    [SerializeField]
    private bool _enableMovement = true;

    [SerializeField]
    private float _movementSpeed = 10f;

    [SerializeField]
    private float _boostedSpeed = 50f;

    [Space]

    [SerializeField]
    private bool _enableSpeedAcceleration = true;

    [SerializeField]
    private float _speedAccelerationFactor = 1.5f;

    [Space]

    [SerializeField]
    private KeyCode _initPositonButton = KeyCode.R;

    #endregion UI



    private CursorLockMode _wantedMode;

    private float _currentIncrease = 1;
    private float _currentIncreaseMem = 0;

    private Vector3 _initPosition;
    private Vector3 _initRotation;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_boostedSpeed < _movementSpeed)
            _boostedSpeed = _movementSpeed;
    }
#endif


    private void Start()
    {
        _initPosition = transform.position;
        _initRotation = transform.eulerAngles;
    }

    private void OnEnable()
    {
        if (_active)
            _wantedMode = CursorLockMode.Locked;
    }

    public void SetMoveSpeed(float value)
    {
        _movementSpeed = value;
        _boostedSpeed = 40 + value;
    }

    public void SetRotatable(bool state)
    {
        _enableRotation = state;        
    }
    // Apply requested cursor state
    private void SetCursorState()
    {
        if (Input.GetMouseButton(1))
        {
            _wantedMode = CursorLockMode.Locked;
            _enableRotation = true;
        }
        else
        {
            Cursor.lockState = _wantedMode = CursorLockMode.None;
            _enableRotation = false;
        }

        // Apply cursor state
        Cursor.lockState = _wantedMode;
        // Hide cursor when locking
        Cursor.visible = (CursorLockMode.Locked != _wantedMode);
    }

    private void CalculateCurrentIncrease(bool moving)
    {
        _currentIncrease = Time.deltaTime;

        if (!_enableSpeedAcceleration || _enableSpeedAcceleration && !moving)
        {
            _currentIncreaseMem = 0;
            return;
        }

        _currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1);
        _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
    }

    private void Update()
    {
        if (!_active)
            return;

        SetCursorState();

        if (Cursor.visible)
            return;

        // Translation
        if (_enableTranslation)
        {
            transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * _translationSpeed);
        }

        // Movement
        if (_enableMovement)
        {
            Vector3 deltaPosition = Vector3.zero;
            float currentSpeed = _movementSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
                currentSpeed = _boostedSpeed;

            if (Input.GetKey(KeyCode.W))
            {
                deltaPosition += transform.forward;
            }

            if (Input.GetKey(KeyCode.S))
            {
                deltaPosition -= transform.forward;
            }

            if (Input.GetKey(KeyCode.A))
            {
                deltaPosition -= transform.right;
            }

            if (Input.GetKey(KeyCode.D))
            {
                deltaPosition += transform.right;
            }

            // Calc acceleration
            CalculateCurrentIncrease(deltaPosition != Vector3.zero);

            transform.position += deltaPosition * currentSpeed * _currentIncrease;

            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _minHeight, _maxHeight), transform.position.z);
        }

        // Rotation
        if (_enableRotation)
        {
            // Pitch
            transform.rotation *= Quaternion.AngleAxis(
                -Input.GetAxis("Mouse Y") * _mouseSense,
                Vector3.right
            );

            // Paw
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y + Input.GetAxis("Mouse X") * _mouseSense,
                transform.eulerAngles.z
            );
        }

        // Return to init position
        if (Input.GetKeyDown(_initPositonButton))
        {
            transform.position = _initPosition;
            transform.eulerAngles = _initRotation;
        }
    }
}
