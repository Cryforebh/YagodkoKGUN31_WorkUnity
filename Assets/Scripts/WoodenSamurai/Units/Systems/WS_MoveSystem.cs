using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WS_MoveSystem : MonoBehaviour
{
    [Inject] private WS_ContainerStatusGame _statusGame;
    [Inject] private WS_AllCell _allCell;
    [Inject] private IPlayerManager _playerManager;
    [Inject] private WS_SoundsUnit _soundUnit;
    [Inject] private BoomEffectController _explosionController;

    //private SignalBus _statusGameSignal;

    //[SerializeField] private Camera _cameraMain;
    [SerializeField] private GameObject _cameraOne;
    [SerializeField] private GameObject _cameraTwo;

    private WS_Cell _targetActionCell;                         // Клетка с которой взаимодействуют
    private WS_Cell _oldCell;                                  // Клетка на которой стоял персонаж, до выбора новой клетки
    private WS_Unit _oldUnit;                                  // Персонаж который был выбран, до выбора нового (врага или друга)
    private WS_Unit _targetSelectedUnit;                       // Выбранный персонаж для взаимодействий
    private WS_Unit _targetActionUnit;                         // Персонаж с которым взаимодействуют
    private List<WS_Cell> _availableCells = new List<WS_Cell>();  // Список доступных клеток
    private List<WS_Unit> _availableUnits = new List<WS_Unit>();  // Список доступных Персонажей для атаки


    public WS_Unit GetOldUnit() => _oldUnit;

    private void Awake()
    {
        _statusGame.OnStatusChanged += HandleStatusChange;
        _playerManager.OnWinnerDeclared += HandleGameEnd;
    }

    //[Inject]
    //private void Construct(SignalBus status)
    //{
    //    _statusGameSignal = status;
    //}

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
    private void HandleStatusChange(WS_EnumStatusGame status)
    {
        if (status == WS_EnumStatusGame.SelectedUnit) HandleSelect();

        if (status == WS_EnumStatusGame.SelectedCell) HandleMovement();
        else if (status == WS_EnumStatusGame.Hit) HandleCombat();
    }

    /// <summary>
    /// Возвращает предыдущую клетку персонажа, к которой был прикреплен до перемещения
    /// </summary>
    /// <param name="unit">Перемещенный персонаж</param>
    /// <returns></returns>
    private WS_Cell GetOldCell(WS_Unit unit)
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

    public void SetTargetActionCell(WS_Cell cell)
    {
        GetOldCell(cell.CurrentUnit);
        _oldUnit = _oldCell.CurrentUnit;
        _targetActionCell = cell;
    }
    public void SetOldUnit(WS_Unit unit) => _oldUnit = unit;
    public void SetTargetSelectedUnit(WS_Unit unit)
    {
        _targetSelectedUnit = unit;
        GetOldCell(unit);
    }
    public void SetTargetActionUnit(WS_Unit unit)
    {
        _targetActionUnit = unit;                                               // Получаем Персонажа с которым взаимодействуют
        if (!unit.IsDead) _targetActionCell = GetCellWhereUnitLocated(unit);    // Получаем клетку где стоит персонаж, над которым совершили действие
        if (_targetSelectedUnit) GetOldCell(_targetSelectedUnit);               // Получаем клетку на которой стоял персонаж, который совершил действие
    }

    /// <summary>
    /// Проверяет, является ли клетка одной из доступных для хода
    /// </summary>
    /// <param name="cell">Проверяемая клетка</param>
    /// <returns></returns>
    public bool IsAvailableCell(WS_Cell cell)
    {
        if (_availableCells.Contains(cell)) return true;
        return false;
    }

    /// <summary>
    /// Проверяет, является ли персонаж одним из доступных для атаки
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public bool IsAvailableCell(WS_Unit unit)
    {
        if (_availableUnits.Contains(unit)) return true;
        return false;
    }

    /// <summary>
    /// Возвращает клетку, на которой стоит персонаж    
    /// </summary>
    /// <param name="unit">Персонаж который стоит на клетке</param>
    /// <returns></returns>
    public WS_Cell GetCellWhereUnitLocated(WS_Unit unit)
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

    public void AllowCellForce()
    {
        AllowCell();
    }

    private void AllowCell()
    {
        GetOldCell(_targetSelectedUnit);

        if (_oldCell == null)
        {
            Debug.LogError("Текущая клетка не определена!");
            return;
        }

        int startX = _oldCell.LocalX;
        int startY = _oldCell.LocalY;
        _availableCells.Clear();

        // Генерация всех клеток в пределах максимальной дистанции
        for (int targetX = 0; targetX < 8; targetX++)
        {
            for (int targetY = 0; targetY < 8; targetY++)
            {
                if (startX == targetX && startY == targetY) continue;

                // Проверка достижимости с учетом обходных путей
                if (IsReachableWithObstacleAvoidance(startX, startY, targetX, targetY))
                {
                    WS_Cell cell = _allCell.GetCell(targetX, targetY);
                    if (cell.CurrentUnit == null)
                    {
                        _availableCells.Add(cell);
                    }
                }
            }
        }
    }

    private bool IsReachableWithObstacleAvoidance(int startX, int startY, int targetX, int targetY)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();

        Vector2Int start = new Vector2Int(startX, startY);
        queue.Enqueue(start);
        distances[start] = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // Проверка цели
            if (current.x == targetX && current.y == targetY)
            {
                return distances[current] <= _targetSelectedUnit.MoveRange;
            }

            // Получение только валидных соседей
            foreach (var neighbor in GetOrthogonalNeighbors(current.x, current.y))
            {
                int newDistance = distances[current] + 1;

                if (distances.ContainsKey(neighbor) || newDistance > _targetSelectedUnit.MoveRange)
                    continue;

                WS_Cell cell = _allCell.GetCell(neighbor.x, neighbor.y);
                if (cell == null || /*cell.IsBlocked ||*/ cell.CurrentUnit != null)
                    continue;

                distances[neighbor] = newDistance;
                queue.Enqueue(neighbor);
            }
        }
        return false;
    }

    private List<Vector2Int> GetOrthogonalNeighbors(int x, int y)
    {
        List<Vector2Int> validNeighbors = new List<Vector2Int>();

        // Проверка каждой возможной клетки
        void TryAddNeighbor(int checkX, int checkY)
        {
            if (IsValidCoordinate(checkX, checkY))
            {
                validNeighbors.Add(new Vector2Int(checkX, checkY));
            }
        }

        TryAddNeighbor(x + 1, y);
        TryAddNeighbor(x - 1, y);
        TryAddNeighbor(x, y + 1);
        TryAddNeighbor(x, y - 1);

        return validNeighbors;
    }

    private bool IsValidCoordinate(int x, int y)
    {
        // Размер поля 8x8 по условию задачи
        return x >= 0 && x < 8 && y >= 0 && y < 8;
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
            _statusGame.StatusUpdate(WS_EnumStatusGame.SelectedUnit);
            return;
        }

        if (_oldCell == null)
        {
            Debug.LogError("Текущая клетка юнита не определена!");
            _statusGame.StatusUpdate(WS_EnumStatusGame.SelectedUnit);
            return;
        }

        if (!_availableCells.Contains(_targetActionCell))
        {
            Debug.LogWarning("Попытка перемещения в недоступную клетку!");
            _statusGame.StatusUpdate(WS_EnumStatusGame.SelectedUnit);
            return;
        }

        // Проигрывается диалог персонажа
        _soundUnit.SoundPlayOnUnitAndStatusGame(_targetSelectedUnit, WS_EnumStatusGame.SelectedCell);

        // Перемещение
        _targetSelectedUnit.transform.position = _targetActionCell.transform.position + Vector3.up;

        ResetMaterialAllowCell();
        ResetMaterialCellAllowUnit();

        // Обновление данных клеток
        UpdateCellOwnership(_targetSelectedUnit, _targetActionCell);

        // Смена игрока
        _playerManager.ChangePlayer();

        //_statusGame.StatusUpdate(EnumStatusGame.Empty);

        //_statusGameSignal.Fire(StatusGameSignal.Return);
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

        if (_oldCell == null || _targetActionCell == null)
        {
            Debug.LogError("Ошибка определения позиций юнитов");
            return;
        }

        // Проверка принадлежности цели
        if (_targetActionUnit.StatusUnit != WS_EnumStatusUnitEnemy.Enemy)
        {
            Debug.Log("Цель не является врагом");
            return;
        }

        // Проверка дистанции атаки
        if (!IsTargetInAttackRange(_oldCell, _targetActionCell, _targetSelectedUnit.AttackRange, _targetSelectedUnit.AttackPattern))
        {
            _statusGame.StatusUpdate(WS_EnumStatusGame.SelectedUnit);
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

            _explosionController.PlayExplosion(_targetActionUnit);

            _targetSelectedUnit.LevelUp(1);
            Debug.Log($"{_targetSelectedUnit.name} принадлежащий {_targetSelectedUnit.Player} - Получил новый уровень!");

            //_soundUnit = _soundUnit.GetAudioSource(_targetActionUnit);
            //_soundUnit.SoundPlay()

            //_targetActionCell.ClearUnit();
            _targetActionUnit.Death();
            Debug.Log($"{_targetActionUnit.name} принадлежащий {_targetActionUnit.Player} - Пал в бою!");
        }

        // Определение победителя если остался единственный игрок с юнитами.
        _playerManager.CheckWinner();

        _playerManager.ChangePlayer();

        //_statusGame.StatusUpdate(EnumStatusGame.Empty);

        //_statusGameSignal.Fire(StatusGameSignal.Return);
    }

    // Обновляет данные клеток с которыми совершались какие-то взаимодействия
    private void UpdateCellOwnership(WS_Unit unit, WS_Cell newCell)
    {
        // Поиск старой клетки
        WS_Cell oldCell = null;
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
            if (cell.CurrentUnit && cell.CurrentUnit.StatusUnit == WS_EnumStatusUnitEnemy.Enemy)
                if (IsTargetInAttackRange(_oldCell, cell, _targetSelectedUnit.AttackRange, _targetSelectedUnit.AttackPattern))
                {
                    _availableUnits.Add(cell.CurrentUnit);
                }
        }
    }

    // Метод проверки дистанции атаки
    private bool IsTargetInAttackRange(WS_Cell attacker, WS_Cell target, int range, WS_EnumAttackPattern pattern)
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
            WS_EnumAttackPattern.Square => Mathf.Max(dx, dy) <= range,
            WS_EnumAttackPattern.Diamond => (dx + dy) <= range,
            WS_EnumAttackPattern.Star => Mathf.Max(dx, dy) <= range || (dx + dy) <= (int)(range * 1.5f),
            WS_EnumAttackPattern.Cross => (dx == 0 && dy <= range) || (dy == 0 && dx <= range),
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
}
