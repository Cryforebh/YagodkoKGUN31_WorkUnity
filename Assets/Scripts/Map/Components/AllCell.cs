using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AllCell : MonoBehaviour
{
    //public List<Cell> cells;
    //[Tooltip("Не используется! Создан для отображения нижнего массива!")] public List<Cell> Vision;
    private Cell[,] CellsMassiv = new Cell[8, 8];

    public Cell[,] Cells => CellsMassiv;

    public void SetCell(Cell cell, int x, int y) 
    {
        CellsMassiv[x, y] = cell;
    }

    public Cell GetCell(int x, int y)
    {
        return CellsMassiv[x, y];
    }

    public Unit GetUnit(Cell cell)
    {
        if (cell.CurrentUnit == null)
        {
            Debug.LogWarning("В Процессе запроса юнита из клетки в классе AllCell, произошла ошибка, так как в ней нет юнита.");
            return null;
        }
        return cell.CurrentUnit;
    }

    public Cell GetCellOnUnit(Unit unit)
    {
        foreach (Cell cell in Cells)
        {
            if (cell.CurrentUnit == unit) return cell;
        }
        Debug.LogWarning($"В Процессе запроса клетки которой пренадлежит {unit.name} в классе AllCell, произошла ошибка, этот юнит не принадлежит не одной из клеток.");
        return null;
    }
    
    public void SetModifierAllUnits(EnumModifier modifier, bool offAndOn, EnumPlayers players)
    {
        foreach (Cell cell in Cells)
        {
            if (cell.CurrentUnit != null && cell.CurrentUnit.Player == players)
            cell.CurrentUnit.SetModifier(modifier, offAndOn);
        }
    }
}
