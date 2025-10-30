using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class C_Battlefield : MonoBehaviour
{
    [Inject] private GameEvent _gameEvent;
    [Inject] private SignalBus _signalBus;
    [Inject] private AdvancedCursorController _animatedCursor;
    [Inject] private IGameData _gameData;
    [Inject] private VoiceUnitManager _voiceUnit;

    private C_Cell[,] _allCells = new C_Cell[8, 8];
    private C_Unit _unitEnter;
    private C_Unit _oldUnit;
    private C_Cell _selectCell;
    private C_Cell _oldCell;

    private bool _isCursorEnterTarget = false;

    private List<C_Cell> _availableCells;
    private List<C_Cell> _availableAttackCells;
    private List<C_Cell> _availableEmemyCells;

    private void Start()
    {
        Construct();
    }

    private void Construct()
    {
        AddCellsOnBattlefield();
    }

    private void OnEnable()
    {
        StartCoroutine(VerificationProcessLock());
    }

    private void OnDisable()
    {
        StopCoroutine(VerificationProcessLock());
    }

    private void AddCellsOnBattlefield()
    {
        C_Cell[] cells = GetComponentsInChildren<C_Cell>();

        foreach (C_Cell cell in cells)
        {
            int x = cell.GetX;
            int y = cell.GetY;

            // Корректность координат
            if (x >= 0 && x < 8 && y >= 0 && y < 8)
            {
                cell.CellClickedEvent += CellClicked;
                cell.CellEnterEvent += CellEnter;
                cell.CellExitEvent += CellExit;

                if (cell.CurrentUnit != null)
                {
                    cell.CurrentUnit.OnEnterEvent += UnitEnter;
                    cell.CurrentUnit.OnClickEvent += UnitClick;
                    cell.CurrentUnit.OnExitEvent += UnitExit;

                    cell.CurrentUnit.Cell = cell;
                }

                _allCells[x, y] = cell;
            }
            else
            {
                Debug.LogWarning($"Некорректные координаты для клетки {cell}: X = {x}, Y = {y}");
            }
        }
    }

    public C_Cell[,] AllCell => _allCells;
    public List<C_Cell> AvailableAttackCells => _availableEmemyCells;
    public C_Cell GetSelectedCell => _selectCell;
    public C_Cell GetOldCell => _oldCell;
    public C_Unit GetUnit => _oldUnit;

    public C_Cell GetCellCurrentUnit(C_Unit unit)
    {
        foreach (C_Cell cell in _allCells)
        {
            if (cell.CurrentUnit == unit) return cell;
        }
        return null;
    }

    public void AddUnitInCell(C_Unit unit, C_Cell oldcell, C_Cell targetcell)
    {
        foreach (C_Cell cell in _allCells)
        {
            if (cell == targetcell)
            {
                cell.AddUnit(unit);
            }
            if (cell == oldcell) oldcell.ClearUnit(unit);
        }
        Debug.Log($"Персонаж {unit} - добавлен в выбранную клетку {targetcell} и удален из старой {oldcell}.");
    }

    public C_Unit GetUnitFromCell(C_Cell cell)
    {
        foreach (C_Cell targetcell in _allCells)
        {
            if (cell.CurrentUnit == targetcell.CurrentUnit) return cell.CurrentUnit;
        }
        return null;
    }

    private void UnitEnter(C_Unit unit)
    {
        _unitEnter = unit;
        ProccesEnter(unit);
    }

    private void ProccesEnter(C_Unit unit)
    {
        if (_gameEvent.Status >= EnumGameEvent.SelectedCell) return;
        if (unit.IsEnemy) return;

        //VisibleAvailableCells(false);
        //CheckAvailableCells(unit.Cell, unit);
        //if (_availableCells.Count == 0 && _availableAttackCells.Count == 0)
        //{
        //    unit.MeshRendererCloth.material = unit.DontEnterMaterialCloth;
        //    _animatedCursor.SetCursorState(EnumStatusCursor.Default);
        //}
        //else
        //{
            unit.MeshRendererCloth.material = unit.EnterMaterialCloth;
            _animatedCursor.SetCursorState(EnumStatusCursor.Select);
        //}
    }

    private void UnitExit(C_Unit unit)
    {
        _animatedCursor.SetCursorState(EnumStatusCursor.Default);

        if (unit.IsEnemy) return;
        _unitEnter = null;

        if (_oldUnit == unit) return;

        unit.MeshRendererCloth.material = unit.OldMaterialCloth;

    }

    private void UnitClick(C_Unit unit)
    {
        _unitEnter = null;

        // Если событие — выбор клетки, то метод завершается без дальнейших действий.
        if (_gameEvent.Status >= EnumGameEvent.SelectedCell) return;

        // Если выбранный юнит является вражеским, метод также завершается.
        if (unit.IsEnemy) return;

        // Скрываем доступные клетки на игровом поле.
        VisibleAvailableCells(false);
        // Проверяем доступные клетки для перемещения/атаки для выбранного юнита.
        CheckAvailableCells(unit.Cell, unit);
        // Проверяем, есть ли доступные клетки для перемещения или атаки.
        if (_availableCells.Count == 0 && _availableAttackCells.Count == 0)
        {
            //_voiceUnit.VoicePlayDontActive();
            // Если доступных клеток нет и был ранее выбранный юнит, то:
            if (_oldUnit != null)
            {
                _oldUnit.MeshRendererCloth.material = _oldUnit.OldMaterialCloth;
                _gameEvent.StatusUpdate(EnumGameEvent.Empty);

                VisibleAvailableCells(false);

                _signalBus.Fire(_gameEvent.Status);

                // Обнуляем ссылки на старый юнит и клетку.
                _oldUnit = null;
                _oldCell = null;
                Debug.Log("Информация о старой Клетки и Юните отчищенны - Так-как идти некуда.");
            }
            return;
        }

        // Если игровое событие Empty, то обрабатываем выбор юнита:
        if (_gameEvent.Status == EnumGameEvent.Empty)
        {
            // Запоминаем клетку, на которой находится выбранный юнит.
            _oldCell = unit.Cell;

            // Если старого юнита ещё нет, устанавливаем текущего юнита как старый.
            if (_oldUnit == null) _oldUnit = unit;

            // Устанавливаем событие выбора юнита.
            _gameEvent.StatusUpdate(EnumGameEvent.SelectedUnit);

            //// Проверяем доступные клетки для выбранного юнита.
            //CheckAvailableCells(_oldCell, unit);
            // Показываем доступные клетки на игровом поле.
            VisibleAvailableCells(true);

            Debug.Log("Юнит выбран.");

            _voiceUnit.VoicePlay(unit, _gameEvent.Status);
            _signalBus.Fire(_gameEvent.Status);
        }
        // Если уже выбран юнит и выбран новый юнит (не тот же самый), то:
        else if (_gameEvent.Status == EnumGameEvent.SelectedUnit && _oldUnit != unit)
        {
            _oldUnit.MeshRendererCloth.material = _oldUnit.OldMaterialCloth;

            // Запоминаем новую клетку и юнит.
            _oldCell = unit.Cell;
            _oldUnit = unit;

            // Устанавливаем событие выбора юнита.
            _gameEvent.StatusUpdate(EnumGameEvent.SelectedUnit);

            //VisibleAvailableCells(false);
            //CheckAvailableCells(_oldCell, unit);
            VisibleAvailableCells(true);

            Debug.Log("Юнит выбран.");

            _voiceUnit.VoicePlay(unit, _gameEvent.Status);
            _signalBus.Fire(_gameEvent.Status);
        }
        // Если выбран тот же юнит снова, то снимаем выбор:
        else if (_oldUnit == unit)
        {

            Debug.Log("Выбор Юнита - Снят.");
            _gameEvent.StatusUpdate(EnumGameEvent.Empty);

            // Скрываем доступные клетки.
            VisibleAvailableCells(false);

            _signalBus.Fire(_gameEvent.Status);

            // Обнуляем ссылки на старый юнит и клетку.
            _oldUnit = null;
            _oldCell = null;
            Debug.Log("Информация о старом Юните очищенна.");
        }
    }

    private void CellEnter(C_Cell cell)
    {
        //if (cell.CurrentUnit != null) return;

        if (cell.RenderEnter.enabled == true)
        {
            cell.RenderEnter.material = cell.EnterMaterial;
            _animatedCursor.SetCursorState(EnumStatusCursor.Select);
        }
    }

    private void CellClicked(C_Cell cell)
    {
        // Проверяем, выбран ли какой-либо юнит. Если нет (_oldUnit == null), то метод завершается.
        if (_oldUnit == null) return;
        // Проверяем, есть ли юнит на выбранной клетке. Если есть, метод завершается.
        if (cell.CurrentUnit != null) return;

        _animatedCursor.SetCursorState(EnumStatusCursor.Default);
        _selectCell = cell;

        // Проверяем доступность клетки для перемещения или атаки.

        if (IsSelected(_selectCell) && _gameEvent.Status != EnumGameEvent.SelectedCell)
        {
            VisibleAvailableCells(false);

            _gameEvent.StatusUpdate(EnumGameEvent.SelectedCell);
            Debug.Log("Клетка выбрана - обычная.");

            _voiceUnit.VoicePlay(_oldUnit, _gameEvent.Status);
            _signalBus.Fire(_gameEvent.Status);

            _oldUnit.MeshRendererCloth.material = _oldUnit.OldMaterialCloth;
            _oldUnit = null;

            Debug.Log("Информация о старом Юните очищенна.");

            _gameEvent.StatusUpdate(EnumGameEvent.EndMove);

            _signalBus.Fire(_gameEvent.Status);

            Debug.Log("Стадия игры вернута к Empty.");
        }
        else if (IsSelectedAttack(_selectCell))
        {
            VisibleAvailableCells(false);

            _gameEvent.StatusUpdate(EnumGameEvent.SelectedCell);
            Debug.Log("Клетка выбрана - вражеская.");

            _voiceUnit.VoicePlay(_oldUnit, _gameEvent.Status);
            _signalBus.Fire(_gameEvent.Status);

            // Рисуем отображения следующих вражеских клеток, если они доступны
            // Проверяем доступные клетки для атаки, исходя из выбранной клетки и выбранного юнита.
            CheckAvailableAttackCells(_selectCell, _oldUnit);

            // Если выбранная клетка доступна для атаки, то запоминаем ее и рисуем для отображения доступных ходов.
            if (IsCellAvailableAttack(_selectCell) == true)
            {
                Debug.Log("Доступная для хода клетка с вражеским Юнитом - Найдена.");
                _oldCell = cell;

                Debug.Log("Информация о выбранной клетке добавлена, как старая клетка.");
                VisibleAvailableCells(true);
                Debug.Log("Доступная для хода клетка с вражеским Юнитом - Показана.");

                return;
            }
            else
            {
                Debug.Log("Доступная для хода клетка с вражеским Юнитом - Не найдена.");

                _oldUnit.MeshRendererCloth.material = _oldUnit.OldMaterialCloth;
                _oldUnit = null;

                Debug.Log("Информация о старом Юните очищенна.");

                _gameEvent.StatusUpdate(EnumGameEvent.EndMove);

                _signalBus.Fire(_gameEvent.Status);

                Debug.Log("Стадия игры вернута к Empty.");
            }
        }
    }

    private void CellExit(C_Cell cell)
    {
        if (cell.RenderEnter.enabled == true)
        {
            cell.RenderEnter.material = cell.OldMaterial;
            _animatedCursor.SetCursorState(EnumStatusCursor.Default);
        }
    }

    public bool CheckAvailableAllCells(EnumPlayers player)
    {
        _availableCells = null;
        _availableAttackCells = null;

        _availableCells = new List<C_Cell>();
        _availableAttackCells = new List<C_Cell>();
        _availableEmemyCells = new List<C_Cell>();

        var all = new List<C_Cell>();
        foreach (C_Cell cell in _allCells)
        {
            if (all.Count > 0) return true;
            if (cell.CurrentUnit != null && cell.CurrentUnit.Player == player)
            {
                if (cell.CurrentUnit.IsDamka == false)
                {
                    all = FindDiagonalCells(cell, _allCells);
                    all.AddRange(_availableAttackCells);
                }
                else
                {
                    all = FindAllDiagonalCells(cell, _allCells);
                    all.AddRange(_availableAttackCells);
                }
            }
        }
        return false;
    }

    private void CheckAvailableCells(C_Cell cell, C_Unit oldUnit)
    {
        _availableCells = null;
        _availableAttackCells = null;

        _availableCells = new List<C_Cell>();
        _availableAttackCells = new List<C_Cell>();
        _availableEmemyCells = new List<C_Cell>();

        if (oldUnit.IsDamka == false)
        {
            _availableCells = FindDiagonalCells(cell, _allCells);
        }
        else
        {
            _availableCells = FindAllDiagonalCells(cell, _allCells);
        }

        //_availableCells.RemoveAll(cell => _availableAttackCells.Contains(cell));
    }

    private void CheckAvailableAttackCells(C_Cell cell, C_Unit oldUnit)
    {
        _availableCells = null;
        _availableAttackCells = null;

        _availableAttackCells = new List<C_Cell>();
        _availableEmemyCells = new List<C_Cell>();

        if (oldUnit.IsDamka == false)
        {
            _availableAttackCells = FindDiagonalEnemyCells(cell, _allCells);
        }
        else
        {
            _availableAttackCells = FindAllDiagonalEnemyCells(cell, _allCells);
        }
    }

    /// <summary>
    /// Находит доступные для хода клетки, и возвращает массив клеток доступных для хода (для обычных пешек).
    /// </summary>
    /// <param name="targetCell">Клетка, которую выбрали для хода.</param>
    /// <param name="allCells">Массив, содержащий все игровые клетки.</param>
    /// <returns></returns>
    public List<C_Cell> FindDiagonalCells(C_Cell targetCell, C_Cell[,] allCells)
    {
        List<C_Cell> diagonalCells = new List<C_Cell>();
        int x = targetCell.GetX;
        int y = targetCell.GetY;
        C_Cell cell;

        bool allowedGoBackPlayerOne = targetCell.CurrentUnit.Player == EnumPlayers.PlayerOne;
        bool allowedGoBackPlayerTwo = targetCell.CurrentUnit.Player == EnumPlayers.PlayerTwo;

        // Проверяем границы массива и добавляем диагональные клетки

        // вниз-вправо
        if (x < 7 && y < 7)
        {
            cell = allCells[x + 1, y + 1];
            // если на клетке нет персонажа
            if (!cell.CurrentUnit && allowedGoBackPlayerOne) diagonalCells.Add(cell);
            // если на клетке есть персонаж и он вражеский
            else if (cell.CurrentUnit && cell.CurrentUnit.IsEnemy && x < 6 && y < 6)
            {
                // добавляем клетку в массив, где храняться клетки с вражеским персонажем
                _availableEmemyCells.Add(cell);
                // если через клетку вражеского персонажа нет другого персонажа
                if (!allCells[x + 2, y + 2].CurrentUnit)
                    //diagonalCells.Add(allCells[x + 2, y + 2]);
                    // добавляем клетку в массив, где храняться клетки находящиеся за вражеским персонажем
                    _availableAttackCells.Add(allCells[x + 2, y + 2]);
            }
        }
        // вверх-вправо
        if (x > 0 && y < 7)
        {
            cell = allCells[x - 1, y + 1];
            if (!cell.CurrentUnit && allowedGoBackPlayerOne) diagonalCells.Add(cell);
            else if (cell.CurrentUnit && cell.CurrentUnit.IsEnemy && x > 1 && y < 6)
            {
                _availableEmemyCells.Add(cell);
                if (!allCells[x - 2, y + 2].CurrentUnit)
                    _availableAttackCells.Add(allCells[x - 2, y + 2]);
            }
        }

        // вверх-влево
        if (x > 0 && y > 0)
        {
            cell = allCells[x - 1, y - 1];
            if (!cell.CurrentUnit && allowedGoBackPlayerTwo) diagonalCells.Add(cell);
            else if (cell.CurrentUnit && cell.CurrentUnit.IsEnemy && x > 1 && y > 1)
            {
                _availableEmemyCells.Add(cell);
                if (!allCells[x - 2, y - 2].CurrentUnit)
                    _availableAttackCells.Add(allCells[x - 2, y - 2]);
            }
        }
        // вниз-влево
        if (x < 7 && y > 0)
        {
            cell = allCells[x + 1, y - 1];
            if (!cell.CurrentUnit && allowedGoBackPlayerTwo) diagonalCells.Add(cell);
            else if (cell.CurrentUnit && cell.CurrentUnit.IsEnemy && x < 6 && y > 1)
            {
                _availableEmemyCells.Add(cell);
                if (!allCells[x + 2, y - 2].CurrentUnit)
                    _availableAttackCells.Add(allCells[x + 2, y - 2]);
            }
        }

        return diagonalCells;
    }

    /// <summary>
    /// Находит доступные для хода вражеские клетки, и возвращает массив клеток доступных для перешагивания через вражеского персонажа.
    /// </summary>
    /// <param name="targetCell">Клетка, которую выбрали для хода.</param>
    /// <param name="allCells">Массив, содержащий все игровые клетки.</param>
    /// <returns></returns>
    public List<C_Cell> FindDiagonalEnemyCells(C_Cell targetCell, C_Cell[,] allCells)
    {
        List<C_Cell> diagonalAttackCells = new List<C_Cell>();
        int x = targetCell.GetX;
        int y = targetCell.GetY;
        C_Cell cell;

        // вниз-вправо
        if (x < 7 && y < 7)
        {
            cell = allCells[x + 1, y + 1];
            // если на клетке есть персонаж и он вражеский
            if (cell.CurrentUnit && cell.CurrentUnit.IsEnemy && x < 6 && y < 6)
            {
                // добавляем клетку в массив, где храняться клетки с вражеским персонажем
                _availableEmemyCells.Add(cell);
                // если через клетку вражеского персонажа нет другого персонажа
                if (!allCells[x + 2, y + 2].CurrentUnit)
                    diagonalAttackCells.Add(allCells[x + 2, y + 2]);
            }
        }
        // вверх-вправо
        if (x > 0 && y < 7)
        {
            cell = allCells[x - 1, y + 1];
            if (cell.CurrentUnit && cell.CurrentUnit.IsEnemy && x > 1 && y < 6)
            {
                _availableEmemyCells.Add(cell);
                if (!allCells[x - 2, y + 2].CurrentUnit)
                    diagonalAttackCells.Add(allCells[x - 2, y + 2]);
            }
        }
        // вверх-влево
        if (x > 0 && y > 0)
        {
            cell = allCells[x - 1, y - 1];
            if (cell.CurrentUnit && cell.CurrentUnit.IsEnemy && x > 1 && y > 1)
            {
                _availableEmemyCells.Add(cell);
                if (!allCells[x - 2, y - 2].CurrentUnit)
                    diagonalAttackCells.Add(allCells[x - 2, y - 2]);
            }
        }
        if (x < 7 && y > 0)
        // вниз-влево
        {
            cell = allCells[x + 1, y - 1];
            if (cell.CurrentUnit && cell.CurrentUnit.IsEnemy && x < 6 && y > 1)
            {
                _availableEmemyCells.Add(cell);
                if (!allCells[x + 2, y - 2].CurrentUnit)
                    diagonalAttackCells.Add(allCells[x + 2, y - 2]);
            }
        }
        return diagonalAttackCells;
    }

    /// <summary>
    /// Находит доступные для хода клетки, и возвращает массив клеток доступных для хода (для ДАМОК).
    /// </summary>
    /// <param name="targetCell">Клетка, которую выбрали для хода.</param>
    /// <param name="allCells">Массив, содержащий все игровые клетки.</param>
    /// <returns></returns>
    private List<C_Cell> FindAllDiagonalCells(C_Cell targetCell, C_Cell[,] allCells)
    {
        _availableEmemyCells = new List<C_Cell>();
        bool diagonalWhichEnemy = false;

        List<C_Cell> diagonalCells = new List<C_Cell>();
        int x = targetCell.GetX;
        int y = targetCell.GetY;
        C_Cell cell = null;

        // Вниз-вправо
        FormulaAllDiagonalCells(diagonalWhichEnemy, diagonalCells, x, y, cell, "downright");
        diagonalWhichEnemy = false;

        // Вверх-вправо
        FormulaAllDiagonalCells(diagonalWhichEnemy, diagonalCells, x, y, cell, "upright");
        diagonalWhichEnemy = false;

        // Вверх-влево
        FormulaAllDiagonalCells(diagonalWhichEnemy, diagonalCells, x, y, cell, "upleft");
        diagonalWhichEnemy = false;

        // Вниз-влево
        FormulaAllDiagonalCells(diagonalWhichEnemy, diagonalCells, x, y, cell, "downleft");
        diagonalWhichEnemy = false;

        return diagonalCells;
    }

    /// <summary>
    /// Находит доступные для хода вражеские клетки, и возвращает массив клеток доступных для перешагивания через вражеского персонажа (для ДАМОК).
    /// </summary>
    /// <param name="targetCell">Клетка, которую выбрали для хода.</param>
    /// <param name="allCells">Массив, содержащий все игровые клетки.</param>
    /// <returns></returns>
    private List<C_Cell> FindAllDiagonalEnemyCells(C_Cell targetCell, C_Cell[,] allCells)
    {
        _availableEmemyCells = new List<C_Cell>();
        bool diagonalWhichEnemy = false;

        List<C_Cell> diagonalAttackCells = new List<C_Cell>();
        int x = targetCell.GetX;
        int y = targetCell.GetY;
        C_Cell cell = null;
        C_Unit unit = null;

        // Вниз-вправо
        FormulaAllDiagonalEnemyCells(diagonalWhichEnemy, diagonalAttackCells, x, y, cell, unit, "downright");
        diagonalWhichEnemy = false;

        // Вверх-вправо
        FormulaAllDiagonalEnemyCells(diagonalWhichEnemy, diagonalAttackCells, x, y, cell, unit, "upright");
        diagonalWhichEnemy = false;

        // Вверх-влево
        FormulaAllDiagonalEnemyCells(diagonalWhichEnemy, diagonalAttackCells, x, y, cell, unit, "upleft");
        diagonalWhichEnemy = false;

        // Вниз-влево
        FormulaAllDiagonalEnemyCells(diagonalWhichEnemy, diagonalAttackCells, x, y, cell, unit, "downleft");
        diagonalWhichEnemy = false;

        return diagonalAttackCells;
    }

    private void FormulaAllDiagonalCells(bool diagonalWhichEnemy, List<C_Cell> diagonalCells,
    int x, int y, C_Cell cell, string direction)
    {
        int formula = 0;
        int directionX = 1;
        int directionY = 1;

        switch (direction)
        {
            case "downright":
                formula = Math.Min(7 - x, 7 - y);
                directionX = 1;
                directionY = 1;
                break;
            case "upright":
                formula = Math.Min(x, 7 - y);
                directionX = -1;
                directionY = 1;
                break;
            case "upleft":
                formula = Math.Min(x, y);
                directionX = -1;
                directionY = -1;
                break;
            case "downleft":
                formula = Math.Min(7 - x, y);
                directionX = 1;
                directionY = -1;
                break;
            default:
                break;
        }

        for (int i = 1; i <= formula; i++)
        {
            int currentX = x + (i * directionX);
            int currentY = y + (i * directionY);

            // Проверка границ перед доступом к ячейке
            if (currentX < 0 || currentX >= 8 || currentY < 0 || currentY >= 8)
                continue;

            cell = _allCells[currentX, currentY];

            // если на клетке дружественный персонаж
            if (cell.CurrentUnit && !cell.CurrentUnit.IsEnemy) break;
            // если на клетке есть персонаж и он вражеский
            else if (cell.CurrentUnit && cell.CurrentUnit.IsEnemy && diagonalWhichEnemy == false)
            {
                // добавляем клетку в массив, где храняться клетки с вражеским персонажем
                _availableEmemyCells.Add(cell);
                // если через клетку вражеского персонажа нет другого персонажа
                // проверяем, можно ли перейти через вражескую клетку
                int nextX = x + ((i + 1) * directionX);
                int nextY = y + ((i + 1) * directionY);
                // проверяем границы для следующей клетки
                if (nextX >= 0 && nextX < 8 && nextY >= 0 && nextY < 8)
                {
                    Debug.Log($"x = {nextX},y = {nextY}. Найдена диагональная клетка за вражеским персонажем.");
                    if (!_allCells[nextX, nextY].CurrentUnit)
                    {
                        Debug.Log($"x = {nextX},y = {nextY}. Клетка пуста для атаки.\"!");
                        // добавляем клетку в массив, где храняться клетки находящиеся за вражеским персонажем
                        _availableAttackCells.Add(_allCells[nextX, nextY]);

                    }
                    else break;
                }
                diagonalWhichEnemy = true;
            }
            // если ранее была обнаружена клетка с вражеским персонажем и клетка ранее не добавлялась
            else if (diagonalWhichEnemy == true && !_availableAttackCells.Contains(cell))
            {
                // если за клеткой вражеского персонажа есть другие свободные клетки
                if (!cell.CurrentUnit)
                {
                    _availableAttackCells.Add(cell);
                    Debug.Log($"Найдена ДОП диагональная клетка {cell} за вражеским персонажем.!");
                }
                else
                {
                    break;
                }
            }
            // если на клетке нет персонажа
            else if (diagonalWhichEnemy == false && !cell.CurrentUnit)
            {
                diagonalCells.Add(cell);
                Debug.Log($"Найдена Пустая диагональная клетка {cell}...");
            }

        }
    }

    private void FormulaAllDiagonalEnemyCells(bool diagonalWhichEnemy, List<C_Cell> diagonalAttackCells,
        int x, int y, C_Cell cell, C_Unit unit, string direction)
    {
        int formula = 0;
        int directionX = 1;
        int directionY = 1;

        switch (direction)
        {
            case "downright":
                formula = Math.Min(7 - x, 7 - y);
                directionX = 1;
                directionY = 1;
                break;
            case "upright":
                formula = Math.Min(x, 7 - y);
                directionX = -1;
                directionY = 1;
                break;
            case "upleft":
                formula = Math.Min(x, y);
                directionX = -1;
                directionY = -1;
                break;
            case "downleft":
                formula = Math.Min(7 - x, y);
                directionX = 1;
                directionY = -1;
                break;
            default:
                break;
        }

        for (int i = 1; i <= formula; i++)
        {
            int currentX = x + (i * directionX);
            int currentY = y + (i * directionY);

            // Проверка границ перед доступом к ячейке
            if (currentX < 0 || currentX > 8 || currentY < 0 || currentY > 8)
                continue;

            cell = _allCells[currentX, currentY];
            unit = cell.CurrentUnit;

            // если на клетке дружественный персонаж
            if (unit && !cell.CurrentUnit.IsEnemy) break;
            // если на клетке есть персонаж и он вражеский
            else if (unit && cell.CurrentUnit.IsEnemy && diagonalWhichEnemy == false)
            {
                // добавляем клетку в массив, где храняться клетки с вражеским персонажем
                _availableEmemyCells.Add(cell);
                // если через клетку вражеского персонажа нет другого персонажа
                // Проверяем, можно ли перейти через вражескую клетку
                int nextX = x + ((i + 1) * directionX);
                int nextY = y + ((i + 1) * directionY);

                // проверяем границы для следующей клетки
                if (nextX >= 0 && nextX < 8 && nextY >= 0 && nextY < 8)
                {
                    Debug.Log($"x = {nextX},y = {nextY}. Найдена диагональная клетка за вражеским персонажем.");

                    if (!_allCells[nextX, nextY].CurrentUnit)
                    {
                        Debug.Log($"x = {nextX},y = {nextY}. Клетка пуста для атаки.\"!");

                        diagonalAttackCells.Add(_allCells[nextX, nextY]);
                    }
                    else break;
                }
                diagonalWhichEnemy = true;
            }
            // если ранее была обнаружена клетка с вражеским персонажем и клетка ранее не добавлялась
            else if (diagonalWhichEnemy == true && !diagonalAttackCells.Contains(cell))
            {
                // если за клеткой вражеского персонажа есть другие свободные клетки
                if (!unit) diagonalAttackCells.Add(cell);
                else break;
            }
        }
    }

    private bool IsCellAvailable(C_Cell cell)
    {
        if (_availableCells == null) return false;
        if (_availableCells.Count == 0) return false;
        return true;
    }

    private bool IsCellAvailableAttack(C_Cell cell)
    {
        if (_availableAttackCells == null) return false;
        if (_availableAttackCells.Count == 0) return false;
        return true;
    }

    public bool IsSelected(C_Cell cell)
    {
        if (IsCellAvailable(cell) == false) return false;

        foreach (var targetcell in _availableCells)
        {
            if (targetcell == cell)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsSelectedAttack(C_Cell cell)
    {
        if (IsCellAvailableAttack(cell) == false) return false;

        foreach (var targetcell in _availableAttackCells)
        {
            if (targetcell == cell)
            {
                return true;
            }
        }
        return false;
    }

    private void VisibleAvailableCells(bool vision)
    {
        ResetMaterialCell();

        if (_availableCells != null)
        {
            foreach (var targetCell in _availableCells)
            {
                targetCell.RenderEnter.enabled = vision;
                if (vision) targetCell.OldMaterial = targetCell.ShowEnterMaterial;
            }
        }

        if (_availableAttackCells != null)
        {
            foreach (var targetcell in _availableAttackCells)
            {
                targetcell.RenderEnter.enabled = vision;
                if (vision)
                {
                    targetcell.OldMaterial = targetcell.ShowEnemyMaterial;
                    targetcell.RenderEnter.material = targetcell.OldMaterial;
                }
            }
        }
    }

    private void ResetMaterialCell()
    {
        foreach (var targetcell in _allCells)
        {
            targetcell.RenderEnter.material = targetcell.ShowEnterMaterial;
            targetcell.OldMaterial = null;
        }
    }

    public C_Unit FindEnemyOnIntermediateCells(C_Cell oldCell, C_Cell newCell)
    {
        var oldX = oldCell.GetX;
        var oldY = oldCell.GetY;
        var newX = newCell.GetX;
        var newY = newCell.GetY;

        // Проверяем, является ли ход диагональным
        if (Math.Abs(newX - oldX) != Math.Abs(newY - oldY))
            return null; // Ход не диагональный

        // Определяем направление движения
        int directionX = newX > oldX ? 1 : -1;
        int directionY = newY > oldY ? 1 : -1;

        // Вычисляем количество шагов
        int steps = Math.Abs(newX - oldX);

        for (int i = 1; i < steps; i++)
        {
            // Вычисляем координаты промежуточной клетки
            int intermediateX = oldX + (i * directionX);
            int intermediateY = oldY + (i * directionY);

            // Проверяем границы массива
            if (intermediateX < 0 || intermediateX >= 8 || intermediateY < 0 || intermediateY >= 8)
                continue; // Промежуточная клетка выходит за границы поля

            // Получаем промежуточную клетку
            C_Cell intermediateCell = _allCells[intermediateX, intermediateY];

            // Проверяем, есть ли на промежуточной клетке вражеский юнит
            if (intermediateCell.CurrentUnit != null && intermediateCell.CurrentUnit.IsEnemy)
                return intermediateCell.CurrentUnit;
        }

        Debug.Log($"Между {oldCell} и {newCell} вражеский персонаж не был найден!");
        return null; // Вражеского юнита на промежуточных клетках нет
    }

    /// <summary>
    /// Убирает признаки выделения обьекта во время блокировки, и возвращает при отмене блокировки (если указатель направлен на него)
    /// </summary>
    private IEnumerator VerificationProcessLock()
    {
        while (true)
        {

            if (_gameData.Lock == true)
            {
                _isCursorEnterTarget = true;
                _animatedCursor.SetCursorState(EnumStatusCursor.Default);
                if (_unitEnter != null) _unitEnter.MeshRendererCloth.material = _unitEnter.OldMaterialCloth;
            }
            else if (_isCursorEnterTarget)
            {
                _isCursorEnterTarget = false;

                if (_unitEnter != null)
                {
                    ProccesEnter(_unitEnter);
                    _unitEnter = null;
                }
            }

            yield return null;
        }
    }

    private void OnDestroy()
    {
        foreach (C_Cell cell in _allCells)
        {
            cell.CellClickedEvent -= CellClicked;
            cell.CellEnterEvent -= CellEnter;
            cell.CellExitEvent -= CellExit;
        }
    }
}

