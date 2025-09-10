using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    [Header("Настройки отображения уровня здоровья")]
    [SerializeField] private Image _healthBar;
    [SerializeField] private Vector3 _offcet = new Vector3(1, 1.5f, 0);
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Canvas _canvas;

    public Image HealthBar => _healthBar;
    public Vector3 Offcet => _offcet;
    public Camera Camera => _mainCamera;
    public Canvas Canvas => _canvas;

    private void Start()
    {
        _canvas.enabled = false;
    }
}
