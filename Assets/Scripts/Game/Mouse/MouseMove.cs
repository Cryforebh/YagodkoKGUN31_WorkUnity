using UnityEngine;
using Zenject;

public class MouseMove : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private float _sensitivity = 2f;
    [SerializeField] private float _smoothTime = 0.3f;
    [SerializeField] private float _maxSpeed = 20f;
    [SerializeField] private float _timeOutOnReturn = 3f;
    [SerializeField] private float _returnSpeed = 1f;

    [Header("Ограничения поворота")]
    [SerializeField] private float _maxHorizontalAngle = 8f;
    [SerializeField] private float _maxVerticalAngle = 7f;

    private Vector3 _initialRotation;
    private Vector2 _currentVelocity;
    private Vector2 _targetRotation;
    private Vector2 _currentInput;

    private float _currentTimeOutOnReturn;

    private bool _isRun = false;
    private MenuToggleManager _menuToggleManager;

    private void Start()
    {
        // Сохраняем начальное положение камеры
        _initialRotation = transform.localEulerAngles;
        _targetRotation = new Vector2(_initialRotation.x, _initialRotation.y);
        _currentTimeOutOnReturn = _timeOutOnReturn;
    }

    private void Update()
    {
        _isRun = _menuToggleManager.DynamicCamera;
        if (_isRun == false) 
        {
            transform.localEulerAngles = _initialRotation;
            return;
        }

        HandleMouseInput();
        ApplyInertia();
        ApplyRotation();
        ClampRotation();
    }

    private void HandleMouseInput()
    {
        // Получаем ввод с плавным затуханием
        Vector2 rawInput = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        );

        _currentInput = Vector2.Lerp(
            _currentInput,
            rawInput,
            Time.deltaTime * 10f
        );
    }

    private void ApplyInertia()
    {
        // Рассчитываем целевое положение с инерцией
        Vector2 inputAcceleration = new Vector2(
            -_currentInput.y * _sensitivity,
            _currentInput.x * _sensitivity
        );

        _targetRotation += inputAcceleration * Time.deltaTime;

        // Плавное возвращение к центру при отсутствии ввода
        if (inputAcceleration.magnitude < 0.05f)
        {
            if (_currentTimeOutOnReturn > 0) _currentTimeOutOnReturn -= 1 * Time.deltaTime;

            if (_currentTimeOutOnReturn <= 0)
            {
                _targetRotation = Vector2.Lerp(
                    _targetRotation,
                    new Vector2(_initialRotation.x, _initialRotation.y),
                    Time.deltaTime * _returnSpeed
                );
            }
        }
        else _currentTimeOutOnReturn = _timeOutOnReturn;
    }

    private void ApplyRotation()
    {
        // Плавное движение к цели
        float newX = Mathf.SmoothDamp(
            transform.localEulerAngles.x,
            _targetRotation.x,
            ref _currentVelocity.x,
            _smoothTime,
            _maxSpeed
        );

        float newY = Mathf.SmoothDamp(
            transform.localEulerAngles.y,
            _targetRotation.y,
            ref _currentVelocity.y,
            _smoothTime,
            _maxSpeed
        );

        transform.localEulerAngles = new Vector3(newX, newY, _initialRotation.z);
    }

    private void ClampRotation()
    {
        // Мягкие ограничения с плавным замедлением
        Vector3 current = transform.localEulerAngles;

        float clampedX = Mathf.Clamp(
            current.x,
            _initialRotation.x - _maxVerticalAngle,
            _initialRotation.x + _maxVerticalAngle
        );

        float clampedY = Mathf.Clamp(
            current.y,
            _initialRotation.y - _maxHorizontalAngle,
            _initialRotation.y + _maxHorizontalAngle
        );

        // Плавная коррекция при выходе за границы
        transform.localEulerAngles = Vector3.Lerp(
            current,
            new Vector3(clampedX, clampedY, current.z),
            Time.deltaTime * 5f
        );
    }

    [Inject]
    private void GetMenuToggleManager (MenuToggleManager menuToggleManager)
    {
        _menuToggleManager = menuToggleManager;
    }
}
