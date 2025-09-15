using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MoveSystem : MonoBehaviour
{
    [Inject] private ContainerStatusGame _statusGame;
    [Inject] private AllCell _allCell;
    [Inject] private PlayerManager _playerManager;
    [Inject] private SoundsUnit _soundUnit;

    //[SerializeField] private Camera _cameraMain;
    [SerializeField] private GameObject _cameraOne;
    [SerializeField] private GameObject _cameraTwo;

    private Cell _targetActionCell;                         // Клетка с которой взаимодействуют
    private Cell _oldCell;                                  // Клетка на которой стоял персонаж, до выбора новой клетки
    private Unit _oldUnit;                                  // Персонаж который был выбран, до выбора нового (врага или друга)
    private Unit _targetSelectedUnit;                       // Выбранный персонаж для взаимодействий
    private Unit _targetActionUnit;                         // Персонаж с которым взаимодействуют
    private List<Cell> _availableCells = new List<Cell>();  // Список доступных клеток
    private List<Unit> _availableUnits = new List<Unit>();  // Список доступных Персонажей для атаки


    public Unit GetOldUnit () => _oldUnit; 

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

    /// <summary>
    /// Включает в себя условия при которых будут реализовываться разные методы: 
    /// - Отображение допустимых клеток для хода;
    /// - Перемещение персонажа;
    /// - Действия связанные с атакой и дальнейшей победой или поражением;
    /// </summary>
    /// <param name="status">Статус игры - условие, которое реализует тот или иной метод хранящийся внутри</param>
    private void HandleStatusChange(EnumStatusGame status)
    {
        if (status == EnumStatusGame.SelectedUnit) HandleSelect();

        if (status == EnumStatusGame.SelectedCell) HandleMovement();
        else if (status == EnumStatusGame.Hit) HandleCombat();
    }

    /// <summary>
    /// Возвращает предыдущую клетку персонажа, к которой был прикреплен до перемещения
    /// </summary>
    /// <param name="unit">Перемещенный персонаж</param>
    /// <returns></returns>
    private Cell GetOldCell(Unit unit)
    {
        foreach (var value in _allCell.Cells)
        {
            if (value.CurrentUnit == unit)
            {
                _oldCell = value;
                break;
            }
        }
        if (!_oldCell) return null;
        return _oldCell;
    }

    public void SetTargetActionCell(Cell cell)
    {
        GetOldCell(cell.CurrentUnit);
        _oldUnit = _oldCell.CurrentUnit;
        _targetActionCell = cell;
    }
    public void SetOldUnit(Unit unit) => _oldUnit = unit;
    public void SetTargetSelectedUnit(Unit unit)
    {
        _targetSelectedUnit = unit;
        GetOldCell(unit);
    }
    public void SetTargetActionUnit(Unit unit)
    {
        _targetActionUnit = unit;                                               // Получаем Персонажа с которым взаимодействуют
        if (!unit.IsDead) _targetActionCell = GetCellWhereUnitLocated(unit);    // Получаем клетку где стоит персонаж, над которым совершили действие
        if (_targetSelectedUnit) GetOldCell(_targetSelectedUnit);               // Получаем клетку на которой стоял персонаж, который совершил действие
    }

    //public void SetSoundUnit(SoundsUnit soundsUnit) => _soundUnit = soundsUnit; 

    /// <summary>
    /// Проверяет, является ли клетка одной из доступных для хода
    /// </summary>
    /// <param name="cell">Проверяемая клетка</param>
    /// <returns></returns>
    public bool IsAvailableCell(Cell cell)
    {
        if (_availableCells.Contains(cell)) return true;
        return false;
    }

    /// <summary>
    /// Проверяет, является ли персонаж одним из доступных для атаки
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public bool IsAvailableCell(Unit unit)
    {
        if (_availableUnits.Contains(unit)) return true;
        return false;
    }

    /// <summary>
    /// Возвращает клетку, на которой стоит персонаж    
    /// </summary>
    /// <param name="unit">Персонаж который стоит на клетке</param>
    /// <returns></returns>
    public Cell GetCellWhereUnitLocated(Unit unit)
    {
        if (!unit.IsDead) return _allCell.GetCellOnUnit(unit);
        Debug.LogError("Персонаж уже мертв! Поэтому через него не возможно достать клетку на которой он стоял.");
        return null;
    }

    /// <summary>
    /// Принудительный выбор клетки, чтобы обработать клетки до вызова события изменения статуса игры (EnumStatusGame)
    /// </summary>
    public void ForcedСallSelect() => HandleSelect();

    /// <summary>
    /// Включает в себя спект действий с методами, которые обрабатывают допустимые клетки для выбора
    /// </summary>
    private void HandleSelect()
    {
        ResetMaterialAllowCell();
        ResetMaterialCellAllowUnit();
        AllowCell();
        SetMaterialAllowCell();
        AllowAttackUnits();
        SetMaterialCellAllowUnit();
    }

    /// <summary>
    /// Определяет и устанавливает доступные для хода клетки
    /// </summary>
    private void AllowCell()
    {
        GetOldCell(_targetSelectedUnit);

        if (_oldCell == null)
        {
            Debug.LogError("Текущая клетка не определена!");
            return;
        }

        int x = _oldCell.LocalX;
        int y = _oldCell.LocalY;

        // Очищаем предыдущий список
        _availableCells.Clear();

        // Определяем шаблон перемещения "Звезда" с радиусом 3
        for (int dx = -_targetSelectedUnit.MoveRange; dx <= _targetSelectedUnit.MoveRange; dx++)
        {
            for (int dy = -_targetSelectedUnit.MoveRange; dy <= _targetSelectedUnit.MoveRange; dy++)
            {
                // Пропускаем клетки за пределами радиуса
                if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) > _targetSelectedUnit.MoveRange)
                    continue;

                // Пропускаем центральную клетку
                if (dx == 0 && dy == 0)
                    continue;

                // Формируем "звездообразный" паттерн
                if (Mathf.Abs(dx) + Mathf.Abs(dy) <= _targetSelectedUnit.MoveRange)
                {
                    int targetX = x + dx;
                    int targetY = y + dy;

                    // Проверяем границы доски
                    if (targetX >= 0 && targetX < 8 && targetY >= 0 && targetY < 8)
                    {
                        Cell cell = _allCell.GetCell(targetX, targetY);
                        if (cell.CurrentUnit == null)
                        {
                            _availableCells.Add(cell);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Включает подстветку доступных клеток для хода
    /// </summary>
    private void SetMaterialAllowCell()
    {
        foreach (var cell in _availableCells)
        {
            cell.MoveFocus(true);
        }
    }

    /// <summary>
    /// Выключает подстветку доступных клеток для хода
    /// </summary>
    private void ResetMaterialAllowCell()
    {
        GetOldCell(_oldUnit);
        if (_availableCells.Count == 0) return;
        foreach (var cell in _availableCells)
        {
            cell.MoveFocus(false);
        }
    }

    /// <summary>
    /// Включает подстветку доступных клеток для атаки
    /// </summary>
    private void SetMaterialCellAllowUnit()
    {
        GetOldCell(_oldUnit);
        if (_availableUnits.Count == 0) return;
        foreach (var unit in _availableUnits)
        {
            GetCellWhereUnitLocated(unit).MoveAttackFocus(true);
        }
    }

    /// <summary>
    /// Выключает подстветку доступных клеток для атаки
    /// </summary>
    private void ResetMaterialCellAllowUnit()
    {
        GetOldCell(_oldUnit);
        if (_availableUnits.Count == 0) return;
        foreach (var unit in _availableUnits)
        {
            if (!unit.IsDead) GetCellWhereUnitLocated(unit).MoveAttackFocus(false);
            _oldCell.MoveAttackFocus(false);
        }
    }

    /// <summary>
    /// Передвижение Персонажа
    /// </summary>
    private void HandleMovement()
    {
        if (_targetSelectedUnit == null || _targetActionCell == null)
        {
            Debug.LogError("Не выбраны юнит или клетка!");
            _statusGame.StatusUpdate(EnumStatusGame.SelectedUnit);
            return;
        }

        if (_oldCell == null)
        {
            Debug.LogError("Текущая клетка юнита не определена!");
            _statusGame.StatusUpdate(EnumStatusGame.SelectedUnit);
            return;
        }

        if (!_availableCells.Contains(_targetActionCell))
        {
            Debug.LogWarning("Попытка перемещения в недоступную клетку!");
            _statusGame.StatusUpdate(EnumStatusGame.SelectedUnit);
            return;
        }

        // Проигрывается диалог персонажа
        _soundUnit.SoundPlayOnUnitAndStatusGame(_targetSelectedUnit, EnumStatusGame.SelectedCell);

        // Перемещение
        _targetSelectedUnit.transform.position = _targetActionCell.transform.position + Vector3.up;

        ResetMaterialAllowCell();
        ResetMaterialCellAllowUnit();

        // Обновление данных клеток
        UpdateCellOwnership(_targetSelectedUnit, _targetActionCell);

        // Смена игрока
        ChangePlayer();

        _statusGame.StatusUpdate(EnumStatusGame.Empty);
    }

    /// <summary>
    /// Атака персонажа
    /// </summary>
    private void HandleCombat()
    {
        // Проверка основных условий
        if (_targetActionUnit == null || _targetSelectedUnit == null)
        {
            Debug.LogError("Не выбраны юнит или цель для атаки!");
            return;
        }

        //// Поиск клеток
        //Cell attackerCell = GetOldCell(_targetSelectedUnit);
        //Cell targetCell = _allCell.GetCellOnUnit(_targetActionUnit);

        if (_oldCell == null || _targetActionCell == null)
        {
            Debug.LogError("Ошибка определения позиций юнитов");
            return;
        }

        // Проверка принадлежности цели
        if (_targetActionUnit.StatusUnit != EnumStatusUnitEnemy.Enemy)
        {
            Debug.Log("Цель не является врагом");
            return;
        }

        // Проверка дистанции атаки
        if (!IsTargetInAttackRange(_oldCell, _targetActionCell, _targetSelectedUnit.AttackRange, _targetSelectedUnit.AttackPattern))
        {
            _statusGame.StatusUpdate(EnumStatusGame.SelectedUnit);
            Debug.LogWarning("Цель вне зоны досягаемости атаки!");
            return;
        }

        // Обновление состояния игры (Убираем отображение выбранных клеток)
        ResetMaterialAllowCell();
        ResetMaterialCellAllowUnit();

        // Логика урона
        // Нанесение урона
        _targetActionUnit.HitDamageHealth(_targetSelectedUnit.GetDamage);
        Debug.Log($"Нанесен урон юниту {_targetActionUnit.name}: {_targetActionUnit.GetPastDamage} урона, осталось {_targetActionUnit.Health} HP.");

        // Проигрывается диалог персонажа
        _soundUnit.SoundPlayerAttack(_targetSelectedUnit, _targetActionUnit);

        // Обработка смерти юнита и повышения уровня киллера
        if (_targetActionUnit.IsDead)
        {
            // Проигрывается диалог персонажа
            _soundUnit.SoundPlayDead(_targetActionUnit);

            _targetSelectedUnit.LevelUp(1);
            Debug.Log($"{_targetSelectedUnit.name} принадлежащий {_targetSelectedUnit.Player} - Получил новый уровень!");

            //_soundUnit = _soundUnit.GetAudioSource(_targetActionUnit);
            //_soundUnit.SoundPlay()

            _targetActionCell.ClearUnit();
            Debug.Log($"{_targetActionUnit.name} принадлежащий {_targetActionUnit.Player} - Пал в бою!");
        }

        // Определение победителя если остался единственный игрок с юнитами.
        _playerManager.CheckWinner();

        ChangePlayer();

        _statusGame.StatusUpdate(EnumStatusGame.Empty);
    }

    // Обновляет данные клеток с которыми совершались какие-то взаимодействия
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

        // Установка и удаление юнитов с клеток
        if (oldCell != null) oldCell.ClearUnit();
        newCell.SetUnit(unit);
    }

    private void AllowAttackUnits()
    {
        _availableUnits.Clear();

        // Собираем вражеских Персонажей в один массив
        foreach (var cell in _allCell.Cells)
        {
            if (cell.CurrentUnit && cell.CurrentUnit.StatusUnit == EnumStatusUnitEnemy.Enemy)
                if (IsTargetInAttackRange(_oldCell, cell, _targetSelectedUnit.AttackRange, _targetSelectedUnit.AttackPattern))
                {
                    _availableUnits.Add(cell.CurrentUnit);
                }
        }
    }

    // Метод проверки дистанции атаки
    private bool IsTargetInAttackRange(Cell attacker, Cell target, int range, EnumAttackPattern pattern)
    {
        if (attacker == null || target == null || range < 1)
        {
            Debug.LogError("Неверные параметры дистанции атаки!");
            return false;
        }

        int dx = Mathf.Abs(attacker.LocalX - target.LocalX);
        int dy = Mathf.Abs(attacker.LocalY - target.LocalY);

        return pattern switch
        {
            EnumAttackPattern.Square => Mathf.Max(dx, dy) <= range,
            EnumAttackPattern.Diamond => (dx + dy) <= range,
            EnumAttackPattern.Star => Mathf.Max(dx, dy) <= range || (dx + dy) <= (int)(range * 1.5f),
            EnumAttackPattern.Cross => (dx == 0 && dy <= range) || (dy == 0 && dx <= range),
            _ => false
        };
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

    private void ChangePlayer()
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
