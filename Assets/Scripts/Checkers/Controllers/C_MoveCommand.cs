using UnityEngine;
using Zenject;

public class C_MoveCommand : MonoBehaviour, C_IGameplayCommand
{
    [Inject] private C_Battlefield _battlefield;

    /// <summary>
    /// Перемещает персонажа в выбранную клетку, если между перемещением есть вражеский персонаж, то он умирает.
    /// </summary>
    public void Interact()
    {
        var unit = _battlefield.GetUnit;

        ProccesTeleportCell(_battlefield.GetOldCell, _battlefield.GetSelectedCell, unit);

        var enemyUnit = _battlefield.FindEnemyOnIntermediateCells(_battlefield.GetOldCell, _battlefield.GetSelectedCell);
        if (enemyUnit)
        {
            enemyUnit.Death();
        }
    }

    /// <summary>
    /// Реализация процесса телепортации персонажа и удаление/добаление в клетку.
    /// </summary>
    public void ProccesTeleportCell(C_Cell oldCell, C_Cell targetCell, C_Unit unit)
    {
        unit.transform.position = targetCell.transform.position + Vector3.up;
        Debug.Log("Персонаж - перемещен.");

        _battlefield.AddUnitInCell(unit, oldCell, targetCell);
    }
}
