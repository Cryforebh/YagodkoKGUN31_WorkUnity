using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

//[RequireComponent(typeof(UnitSelectionPointer)), RequireComponent(typeof(Unit)), RequireComponent(typeof(Cell))]
public class MoveSystem : MonoBehaviour
{
    [Inject] private ContainerStatusGame _statusGame;
    [Inject] private AllCell _allCell;
    [Inject] private PlayerManager _playerManager;

    private Cell _cell;
    private Cell _oldCell;
    private Unit _unit;
    private Unit _currentUnitAction;
    private bool _select;  

    private void Awake()
    {
        _statusGame.OnStatusChanged += _statusGame_OnStatusChanged;
    }

    private void OnDestroy()
    {
        _statusGame.OnStatusChanged -= _statusGame_OnStatusChanged;
    }

    private void _statusGame_OnStatusChanged(EnumStatusGame obj)
    {
        if ( obj == EnumStatusGame.SelectedCell)
        {
            Debug.Log("Данные собраны!");
            Moved();
        }
        if ( obj == EnumStatusGame.Hit)
        {
            //Debug.Log("Данные собраны!");
            //Debug.Log("Удар по вражескому юниту!");
            Fight();
        }


    }

    public void SetSelect(bool select) {  _select = select; }
    public void SetUnitSelected(Unit unit) {  _unit = unit; }
    public void SetUnitAction(Unit unit) { _currentUnitAction = unit; }
    public void SetCell(Cell cell) { _cell = cell; }

    private void Moved()
    {
        _unit.transform.position = _cell.transform.position + Vector3.up * 2;
        Debug.Log($"Персонаж {_unit.gameObject.name} перемещен...");

        foreach (var cell in _allCell.Cells)
        {
            if (cell.CurrentUnit == _unit) _oldCell = cell;
        }

        _cell.SetUnit(_unit);
        Debug.Log($"Клетка {_cell.gameObject.name} сохранила данные о Персонаже {_unit.gameObject.name}...");

        if (_oldCell)
        {
            _oldCell.ClearUnit();
            Debug.Log($"Клетка {_oldCell.gameObject.name} удалила данные о Персонаже {_unit.gameObject.name}...");
        }
        else if (!_oldCell) { Debug.Log($"В предыдущую клетку небыло добавлено {_unit.gameObject.name}!!!"); }

        // Явное обновление статуса (опционально)
        _unit.SetStatusUnit(_unit.Player == _playerManager.ActivePlayer
            ? EnumStatusUnit.My
            : EnumStatusUnit.Enemy);

        // !!!!!!  Для тестов возвращает стадию к нулю !!!!!!!
        _statusGame.StatusUpdate(EnumStatusGame.Empty);
    }

    private void Fight()
    {
        if (_currentUnitAction.GetStatusUnit == EnumStatusUnit.Enemy)
        {
            // Логика атаки вражеского юнита
            Debug.Log($"Атакуем врага: {_unit.name}");
        }
        else
        {
            Debug.Log("Нельзя атаковать своего юнита!");
        }
    }
}
