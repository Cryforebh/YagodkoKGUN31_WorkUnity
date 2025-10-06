using UnityEngine;
using Zenject;

public class CellManager : MonoBehaviour
{
    [Inject] private DiContainer _container;
    [SerializeField] private Material _selectionMaterial;

    private Cell _selectedCell;

    private void OnEnable() => RegisterCells();
    private void OnDisable() => UnregisterCells();

    private void RegisterCells()
    {
        foreach (var cell in GetComponentsInChildren<Cell>())
        {
            cell.OnPointerClickEvent += HandleCellSelection;
            _container.Inject(cell);
        }
    }

    private void UnregisterCells()
    {
        foreach (var cell in GetComponentsInChildren<Cell>())
            cell.OnPointerClickEvent -= HandleCellSelection;
    }

    private void HandleCellSelection(Cell cell)
    {
        _selectedCell?.ResetAll();
        cell.SetSelect(_selectionMaterial);
        _selectedCell = cell;
    }
}
