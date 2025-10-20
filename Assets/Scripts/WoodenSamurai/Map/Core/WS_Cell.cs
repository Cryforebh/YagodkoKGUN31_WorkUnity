using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class WS_Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private WS_ContainerStatusGame _statusGame;
    [Inject] private WS_MoveSystem _moveSystem;
    [Inject] private WS_AllCell _allCell;
    [Inject] private AdvancedCursorController _cursor;

    //private SignalBus _statusGameSignal;

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
    [SerializeField] private WS_Unit _installedStartUnit;
    [SerializeField] private EnumPlayers _player;

    [Header("Настройки Модификаторов клетки:")]
    [SerializeField, Tooltip("Увеличивает: Сопротивление урона на 50%.")] private bool _forest = false;
    [SerializeField, Tooltip("Увеличивает: Дальность атаки на 1 клетку - Стрелкам; Урон на 1 ед. - Милишникам")] private bool _trees = false;

    public int LocalX => _localX;
    public int LocalY => _localY;
    public WS_Unit InstalledStartUnit => _installedStartUnit;
    public bool CreateUnit => _createUnit;
    public EnumPlayers Player => _player;
    public WS_Unit CurrentUnit { get; private set; }
    public event Action<WS_Cell> OnPointerClickEvent;

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

    //[Inject]
    //private void Construct(SignalBus signalBus)
    //{
    //    _statusGameSignal = signalBus;
    //}

    public void SetSelect(Material mat)
    {
        if (CurrentUnit == null)
        {
            _select.sharedMaterial = mat;
            //_select.enabled = true;
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
    //public void SetUnit(Unit unit) => CurrentUnit = unit;
    public void SetUnit(WS_Unit unit)
    {
        SetModifier(unit);
        CurrentUnit = unit;
    }
    public void ClearUnit()
    {
        RemoveModifier(CurrentUnit);
        CurrentUnit = null;
    }

    private void SetModifier(WS_Unit unit)
    {
        if (_forest) unit.ModifierDefense = true;
        if (_trees && unit.Class == WS_EnumStatusUnitClass.Samurai) unit.ModifierDamage = 2;
        if (_trees && unit.Class == WS_EnumStatusUnitClass.Ranger)
        {
            unit.ModifierDrowRange = true;
            unit.AttackRange += 1;
        }
    }
    private void RemoveModifier(WS_Unit unit)
    {
        if (_forest) unit.ModifierDefense = false;
        if (_trees && unit.Class == WS_EnumStatusUnitClass.Samurai) unit.ModifierDamage = 0;
        if (_trees && unit.Class == WS_EnumStatusUnitClass.Ranger)
        {
            unit.ModifierDrowRange = false;
            unit.AttackRange -= 1;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Проверяет, является ли клетка одной из доступных для хода (чтобы не кликнуть)
        if (!_moveSystem.IsAvailableCell(this)) return;

        if (_statusGame.Status == WS_EnumStatusGame.SelectedUnit && !_isSelected && CurrentUnit == null)
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

        if (_statusGame.Status == WS_EnumStatusGame.SelectedUnit && CurrentUnit == null)
        {
            OnPointerClickEvent?.Invoke(this);

            _moveSystem.SetTargetActionCell(this);
            Debug.Log("Данные выбранной клетки переданы");

            _focus.enabled = false;
            //_statusGameSignal.Fire(StatusGameSignal.SelectCell);

            // Курсор - Дефолт
            _cursor.SetCursorState(EnumStatusCursor.Default);

            _statusGame.StatusUpdate(WS_EnumStatusGame.SelectedCell);

        }
        else if (CurrentUnit != null)
        {
            Debug.Log("Клетка уже занята другим юнитом!");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _focus.enabled = false;
        _select.enabled = false;

        // Курсор - Дефолт
        _cursor.SetCursorState(EnumStatusCursor.Default);
    }


    public Vector3 GetPosition() => _position;
}

