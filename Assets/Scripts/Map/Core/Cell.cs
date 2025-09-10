using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private ContainerStatusGame _statusGame;
    [Inject] private MoveSystem _moveSystem;
    [Inject] private AllCell _allCell;
    [Inject] private AllPlayer _allPlayer;

    [Header("Настройка Клетки (Положение в Массиве):")]
    [SerializeField] private int _localX;
    [SerializeField] private int _localY;

    [Header("Мешы для отображения выбора:")]
    [SerializeField] private MeshRenderer _focus;
    [SerializeField] private MeshRenderer _select;

    [Header("Создавать ли Unit'та на клетке?")]
    [SerializeField] private bool _createUnit = false;

    [Header("Настройки Unit'та:")]
    [SerializeField] private Unit _unit;
    //[SerializeField] private EnumStatusUnit _isStatusEnemy;
    [SerializeField] private EnumPlayers _player;

    public Unit Unit => _unit;
    public bool CreateUnit => _createUnit;
    //public EnumStatusUnit StatusEnemy => _isStatusEnemy;
    public EnumPlayers Player => _player;
    public Unit CurrentUnit { get; private set; }
    public event Action<Cell> OnPointerClickEvent;

    private bool _isSelected = false;
    private Vector3 _position;

    private void Awake()
    {
        _position = transform.position;
        //_allCell.cells.Add(this);
        _allCell.SetCell(this, _localX, _localY);
    }

    private void Start()
    {
        if (!CurrentUnit && _createUnit) CurrentUnit = _unit;
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

    public void SetUnit(Unit unit) => CurrentUnit = unit;
    public void ClearUnit() => CurrentUnit = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_statusGame.Status == EnumStatusGame.SelectedUnit && !_isSelected && CurrentUnit == null)
        {
            _focus.enabled = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_statusGame.Status == EnumStatusGame.SelectedUnit && CurrentUnit == null)
        {
            OnPointerClickEvent?.Invoke(this);

            _moveSystem.SetTargetCell(this);
            Debug.Log("Данные выбранной клетки переданы");

            _statusGame.StatusUpdate(EnumStatusGame.SelectedCell);
        }
        else if (CurrentUnit != null)
        {
            Debug.Log("Клетка уже занята другим юнитом!");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
        => _focus.enabled = false;

    public Vector3 GetPosition() => _position;
}

