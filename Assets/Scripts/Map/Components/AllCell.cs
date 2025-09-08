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
    
}
