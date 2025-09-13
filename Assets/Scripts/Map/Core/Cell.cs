using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private ContainerStatusGame _statusGame;
    [Inject] private MoveSystem _moveSystem;
    [Inject] private AllCell _allCell;
    [Inject] private AdvancedCursorController _cursor;

    [Header("Настройка Клетки (Положение в Массиве):")]
    [SerializeField] private int _localX;
    [SerializeField] private int _localY;

    [Header("Мешы для отображения выбора:")]
    [SerializeField] private MeshRenderer _focus;
    [SerializeField] private MeshRenderer _select;

    [Header("Мешы для отображения доступных клеток:")]
    [SerializeField] private MeshRenderer _moveFocus;
    [SerializeField] private MeshRenderer _attackFocus;

    [Header("Создавать ли Unit'та на клетке?")]
    [SerializeField] private bool _createUnit = false;

    [Header("Настройки Unit'та:")]
    [SerializeField] private Unit _installedStartUnit;
    [SerializeField] private EnumPlayers _player;

    public int LocalX => _localX;
    public int LocalY => _localY;
    public Unit InstalledStartUnit => _installedStartUnit;
    public bool CreateUnit => _createUnit;
    public EnumPlayers Player => _player;
    public Unit CurrentUnit { get; private set; }
    public event Action<Cell> OnPointerClickEvent;

    private bool _isSelected = false;
    private Vector3 _position;

    private void Awake()
    {
        _position = transform.position;
        _allCell.SetCell(this, _localX, _localY);
    }

    private void Start()
    {
        if (!CurrentUnit && _createUnit) CurrentUnit = _installedStartUnit;
        ResetAll();
    }

    public void SetSelect(Material mat)
    {
        if (CurrentUnit == null)
        {
            _select.sharedMaterial = mat;
            _select.enabled = true;
            _isSelected = true;
        }
    }

    public void ResetAll()
    {
        _focus.enabled = false;
        _select.enabled = false;
        _select.sharedMaterial = null;
        _isSelected = false;
    }

    public void MoveFocus(bool activate) => _moveFocus.enabled = activate;
    public void MoveAttackFocus(bool activate) => _attackFocus.enabled = activate;
    public void SetUnit(Unit unit) => CurrentUnit = unit;
    public void ClearUnit() => CurrentUnit = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Проверяет, является ли клетка одной из доступных для хода (чтобы не кликнуть)
        if (!_moveSystem.IsAvailableCell(this)) return;

        if (_statusGame.Status == EnumStatusGame.SelectedUnit && !_isSelected && CurrentUnit == null)
        {
            _focus.enabled = true;

            // Курсор - Выбор
            _cursor.SetCursorState(EnumStatusCursor.Select);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Проверяет, является ли клетка одной из доступных для хода (чтобы не кликнуть)
        if (!_moveSystem.IsAvailableCell(this)) return;

        if (_statusGame.Status == EnumStatusGame.SelectedUnit && CurrentUnit == null)
        {
            OnPointerClickEvent?.Invoke(this);

            _moveSystem.SetTargetActionCell(this);
            Debug.Log("Данные выбранной клетки переданы");

            // Курсор - Дефолт
            _cursor.SetCursorState(EnumStatusCursor.Default);

            _statusGame.StatusUpdate(EnumStatusGame.SelectedCell);

        }
        else if (CurrentUnit != null)
        {
            Debug.Log("Клетка уже занята другим юнитом!");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _focus.enabled = false;

        // Курсор - Дефолт
        _cursor.SetCursorState(EnumStatusCursor.Default);
    }


    public Vector3 GetPosition() => _position;
}

