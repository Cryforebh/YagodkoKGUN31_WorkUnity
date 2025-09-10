using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Unit))]
public class HealthBar : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private HealthBarManager _healthBarManager;

    private Vector3 _offcet;
    private Unit _unit;
    private Canvas _canvas;
    private Image _healthBar;
    private Camera _camera;
    private float _maxHealth;
    private float _currentHealth;

    private void Awake()
    {
        _healthBarManager = FindObjectOfType <HealthBarManager>();
    }

    private void Start()
    {
        _offcet = _healthBarManager.Offcet;
        _healthBar = _healthBarManager.HealthBar;
        _camera = _healthBarManager.Camera;
        _unit = GetComponent<Unit>();
        _canvas = _healthBarManager.Canvas;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _canvas.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _canvas.enabled = true;
        Disaplay();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _canvas.enabled = false;
    }

    private void PositionOnUnit()
    {
        _canvas.transform.position = _unit.transform.position + _offcet;
        //_canvas.transform.LookAt(_camera.transform);
        //_canvas.transform.Rotate(0, 180f, 0); // Корректировка ориентации
    }

    private void Disaplay()
    {
        PositionOnUnit();
        _maxHealth = _unit.GetMaxHealth;
        _currentHealth = _unit.GetHealth;
        _healthBar.fillAmount = _currentHealth / _maxHealth;
    }

}
