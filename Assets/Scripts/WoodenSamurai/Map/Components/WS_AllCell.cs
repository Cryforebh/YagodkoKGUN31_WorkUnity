using UnityEngine;

public class WS_AllCell : MonoBehaviour
{
    private WS_Cell[,] CellsMassiv = new WS_Cell[8, 8];

    public WS_Cell[,] Cells => CellsMassiv;

    public void SetCell(WS_Cell cell, int x, int y)
    {
        CellsMassiv[x, y] = cell;
    }

    public WS_Cell GetCell(int x, int y)
    {
        return CellsMassiv[x, y];
    }

    public WS_Unit GetUnit(WS_Cell cell)
    {
        if (cell.CurrentUnit == null)
        {
            Debug.LogWarning("В Процессе запроса юнита из клетки в классе AllCell, произошла ошибка, так как в ней нет юнита.");
            return null;
        }
        return cell.CurrentUnit;
    }

    public WS_Cell GetCellOnUnit(WS_Unit unit)
    {
        foreach (WS_Cell cell in Cells)
        {
            if (cell.CurrentUnit == unit) return cell;
        }
        Debug.LogWarning($"В Процессе запроса клетки которой пренадлежит {unit.name} в классе AllCell, произошла ошибка, этот юнит не принадлежит не одной из клеток.");
        return null;
    }

    public void SetModifierAllUnits(WS_EnumModifier modifier, bool offAndOn, EnumPlayers players)
    {
        foreach (WS_Cell cell in Cells)
        {
            if (cell.CurrentUnit != null && cell.CurrentUnit.Player == players)
                cell.CurrentUnit.SetModifier(modifier, offAndOn);
        }
    }
}
