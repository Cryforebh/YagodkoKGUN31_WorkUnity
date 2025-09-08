using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerManager : MonoBehaviour
{
    [Inject] private AllPlayer _players;

    private EnumPlayers _activePlayer = EnumPlayers.None;

    public event Action<EnumPlayers> OnActivePlayerChanged; // Событие изменения активного игрока

    public EnumPlayers ActivePlayer
    {
        get => _activePlayer;
        set
        {
            if (_activePlayer != value)
            {
                _activePlayer = value;
                UpdateAllUnitsStatuses(); // Обновляем статусы юнитов
                OnActivePlayerChanged?.Invoke(_activePlayer); // Вызываем событие
            }
        }
    }

    private void UpdateAllUnitsStatuses()
    {
        foreach (var playerEntry in _players.PlayersCollection)
        {
            bool isMy = playerEntry.Key == _activePlayer;
            foreach (Unit unit in playerEntry.Value)
            {
                unit.SetStatusUnit(isMy ? EnumStatusUnit.My : EnumStatusUnit.Enemy);
            }
        }
    }

    private void Start()
    {
        // Стартовый активный игрок (пример)
        ActivePlayer = EnumPlayers.PlayerTwo;
        Debug.LogWarning($"Стартовый персонаж назначен - это {ActivePlayer}!");
    }

}
