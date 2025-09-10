using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MoveSystem : MonoBehaviour
{
    [Inject] private ContainerStatusGame _statusGame;
    [Inject] private AllCell _allCell;
    [Inject] private PlayerManager _playerManager;

    //[SerializeField] private Camera _cameraMain;
    [SerializeField] private GameObject _cameraOne;
    [SerializeField] private GameObject _cameraTwo;

    private Cell _targetCell;
    private Unit _selectedUnit;
    private Unit _actionTargetUnit;

    private void Awake()
    {
        _statusGame.OnStatusChanged += HandleStatusChange;
        _playerManager.OnWinnerDeclared += HandleGameEnd;
    }

    private void OnDestroy()
    {
        _statusGame.OnStatusChanged -= HandleStatusChange;
        _playerManager.OnWinnerDeclared -= HandleGameEnd;
    }

    private void HandleStatusChange(EnumStatusGame status)
    {
        if (status == EnumStatusGame.SelectedCell) HandleMovement();
        else if (status == EnumStatusGame.Hit) HandleCombat();
    }

    public void SetTargetCell(Cell cell) => _targetCell = cell;
    public void SetSelectedUnit(Unit unit) => _selectedUnit = unit;
    public void SetActionTarget(Unit unit) => _actionTargetUnit = unit;

    private void HandleMovement()
    {
        if (_selectedUnit == null || _targetCell == null)
        {
            Debug.LogError("Не выбраны юнит или клетка!");
            return;
        }

        // Перемещение
        _selectedUnit.transform.position = _targetCell.transform.position + Vector3.up * 2;

        // Обновление данных клеток
        UpdateCellOwnership(_selectedUnit, _targetCell);

        SelectPlayer();

        _statusGame.StatusUpdate(EnumStatusGame.Empty);
    }

    private void UpdateCellOwnership(Unit unit, Cell newCell)
    {
        // Поиск старой клетки
        Cell oldCell = null;
        foreach (var cell in _allCell.Cells)
        {
            if (cell.CurrentUnit == unit)
            {
                oldCell = cell;
                break;
            }
        }

        if (oldCell != null) oldCell.ClearUnit();
        newCell.SetUnit(unit);
    }

    private void HandleCombat()
    {
        if (_actionTargetUnit == null)
        {
            Debug.LogError("Цель для атаки не установлена!");
            return;
        }

        if (_actionTargetUnit.GetStatusUnit == EnumStatusUnit.Enemy)
        {
            // Логика урона

            _actionTargetUnit.HitDamageHealth(_selectedUnit.GetDamage);
            Debug.Log($"Нанесен урон юниту {_actionTargetUnit.name}: {_actionTargetUnit.GetPastDamage} урона, осталось {_actionTargetUnit.GetHealth} HP.");

            if (_actionTargetUnit.IsDead)
            {
                _allCell.GetCellOnUnit(_actionTargetUnit).ClearUnit();
                Debug.Log($"{_actionTargetUnit.name} - Пал в бою!");
            }

            // Сюда добавить определение победителя если остался единственный игрок с юнитами.
            _playerManager.CheckWinner();

            SelectPlayer();

            _statusGame.StatusUpdate(EnumStatusGame.Empty);
        }
        else
        {
            Debug.Log("Ошибка: цель не является врагом");
        }
    }

    private void HandleGameEnd(EnumPlayers winner)
    {
        if (winner == EnumPlayers.None)
        {
            Debug.Log("Игра окончена. Нет победителя!");
        }
        else
        {
            Debug.LogWarning($"ИГРА ОКОНЧЕНА! ПОБЕДИТЕЛЬ: {winner}");
            // Остановка игры, показ UI и т.д.
        }
    }

    private void SelectPlayer()
    {
        if (_playerManager.ActivePlayer == EnumPlayers.PlayerTwo)
        {
            //_cameraOne.SetActive(true);
            //_cameraTwo.SetActive(false);
            _playerManager.ActivePlayer = EnumPlayers.PlayerOne;
        }
        else if (_playerManager.ActivePlayer == EnumPlayers.PlayerOne)
        {
            //_cameraOne.SetActive(false);
            //_cameraTwo.SetActive(true);
            _playerManager.ActivePlayer = EnumPlayers.PlayerTwo;
        }
    }
}
