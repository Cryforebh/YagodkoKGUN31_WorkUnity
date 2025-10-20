using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class C_Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private C_MaterialsContainer _materialContainer;

    [SerializeField] private int _x;
    [SerializeField] private int _y;

    [SerializeField] private bool _setDamka;
    [SerializeField] private EnumPlayers _players;

    [SerializeField] private MeshRenderer _renderEnter;
    private Material _materialShow;
    private Material _materialEnter;
    private Material _materialShowEnemy;
    private Material _materialEnterEnemy;

    [SerializeField] private C_Unit _startUnit;

    private C_Unit _currentUnit;

    public event Action<C_Cell> CellClickedEvent;
    public event Action<C_Cell> CellEnterEvent;
    public event Action<C_Cell> CellExitEvent;

    public int GetX => _x;
    public int GetY => _y;

    public MeshRenderer RenderEnter => _renderEnter;

    public Material OldMaterial { get; set; }
    public Material ShowEnemyMaterial => _materialShowEnemy;
    public Material EnterEnemyMaterial => _materialEnterEnemy;
    public Material ShowEnterMaterial => _materialShow;
    public Material EnterMaterial => _materialEnter;

    public C_Unit CurrentUnit => _currentUnit;

    public void AddUnit(C_Unit unit)
    {
        _currentUnit = unit;
        _currentUnit.Cell = this;
        if (_setDamka && _players != _currentUnit.Player)
        {
            if (_currentUnit.IsDamka == false)
            {
                _currentUnit.IsDamka = true;
                _currentUnit.UpdateMeshRenderDamka();
                Debug.Log($"{_currentUnit} - Теперь Дамка!");
            }
        }
    }
    public void ClearUnit(C_Unit unit)
    {
        _currentUnit = null;
    }

    private void Awake()
    {
        Construct();
    }

    private void Construct()
    {
        _materialEnter = _materialContainer.MaterialEnter;
        _materialEnterEnemy = _materialContainer.MaterialEnterEnemy;
        _materialShow = _materialContainer.MaterialShow;
        _materialShowEnemy = _materialContainer.MaterialShowEnemy;

        if (_startUnit != null && _startUnit.gameObject.activeSelf == true)
        {
            _currentUnit = _startUnit;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CellClickedEvent?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CellEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CellExitEvent?.Invoke(this);
    }
}
