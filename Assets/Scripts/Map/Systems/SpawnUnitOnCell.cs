using UnityEngine;
using Zenject;

public class SpawnUnitOnCell : MonoBehaviour
{
    [Inject] private AllCell _allCell;
    [Inject] private AllPlayer _player;
    [Inject] private PlayerManager _playerManager;
    //[Inject] private UnitManager _unitManager;
    [SerializeField] private Unit _unit;

    private void Start()
    {
        foreach (var cell in _allCell.Cells)
        {
            if (cell.CreateUnit)
            {
                Create(cell);
                //Debug.Log($"Создан {cell.CurrentUnit.name}, у него {cell.CurrentUnit.GetHealth} здоровья и {cell.CurrentUnit.GetDamage} урона.\n" +
                //    $"Это {cell.CurrentUnit.GetStatusUnit}!");
            }
        }
    }

    private void Create(Cell cell)
    {
        Unit newUnit;

        // Создаём Юнита в кординатах клетки
        /* - Префаб Юнита устанавливается в самой Клетке если он есть */
        /* - Если не установлен - то создается Юнит из SpawnUnitOnCell */
        if (cell.Unit == null) newUnit = Instantiate(_unit, cell.transform.position + Vector3.up * 2, Quaternion.identity);
        else 
            newUnit = Instantiate(cell.Unit, cell.transform.position + Vector3.up * 2, Quaternion.identity);

        // Подписываем юнита на собития связанные с ним
        //newUnit.

        // Присваиваем юниту владельца (игрока из клетки)
        newUnit.SetPlayer(cell.Player);

        // Добавляем юнита в коллекцию игрока (в список, который принадлежит Player'у указанного в Cell)
        _player.AddUnitOnPlayer(cell.Player, newUnit);

        // Установка статуса при создании
        bool isMy = cell.Player == _playerManager.ActivePlayer;
        newUnit.SetStatusUnit(isMy ? EnumStatusUnit.My : EnumStatusUnit.Enemy);

        // Привязываем юнита к клетке
        cell.SetUnit(newUnit);
    }
}
