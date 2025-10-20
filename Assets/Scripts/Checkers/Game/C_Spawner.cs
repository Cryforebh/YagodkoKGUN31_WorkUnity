using UnityEngine;
using Zenject;

public class C_Spawner : MonoBehaviour, ISpawner
{
    [Inject] private IPlayerManager _playerManager;
    [Inject] private AllPlayer _player;
    [Inject] private C_Battlefield _battlefield;

    private EnumPlayers _playerGoesFirst;

    public void CreateUnits()
    {
        foreach (var cell in _battlefield.AllCell)
        {
            if (cell.CurrentUnit)
            {
                // Добавляем юнита в коллекцию игрока (в список, который принадлежит Player'у)
                _player.AddUnitOnPlayer(cell.CurrentUnit.Player, cell.CurrentUnit);

                // Установка статуса при создании
                bool isMy = cell.CurrentUnit.Player == _playerManager.ActivePlayer;
                if (isMy) cell.CurrentUnit.IsEnemy = false;
                else cell.CurrentUnit.IsEnemy = true;
            }
        }
    }

    public void SetWhoGoesFirst(EnumPlayers player)
    {
        _playerGoesFirst = player;
        _playerManager.ActivePlayer = _playerGoesFirst;
        Debug.Log($"Стартовый Игрок назначен - это {player}!");
    }

}
