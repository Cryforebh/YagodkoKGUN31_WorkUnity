using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PlayerManager : MonoBehaviour
{
    [Inject] private AllPlayer _players;
    [Inject] private TypeGameManager _gameManager;

    private EnumPlayers _activePlayer = EnumPlayers.None;

    public event Action<EnumPlayers> OnActivePlayerChanged; // Событие изменения активного игрока
    public event Action<EnumPlayers> OnWinnerDeclared; // Событие Победы

    [SerializeField] private Camera _cameraPlayerOne;
    [SerializeField] private Camera _cameraPlayerTwo;

    [SerializeField] private WinImage _winImagePlayerOne;
    [SerializeField] private WinImage _winImagePlayerTwo;


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
                Debug.Log($"Ходит - {_activePlayer}!");
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
                unit.SetStatusUnit(isMy ? EnumStatusUnitEnemy.My : EnumStatusUnitEnemy.Enemy);
            }
        }
    }

    public EnumPlayers? CheckWinner()
    {
        // Фильтруем игроков с юнитами, исключая None
        var activePlayers = _players.PlayersCollection
            .Where(p => p.Key != EnumPlayers.None && p.Value.Count > 0)
            .Select(p => p.Key)
            .ToList();

        if (activePlayers.Count == 0)
        {
            Debug.Log("Все игроки уничтожены! Ничья!");
            OnWinnerDeclared?.Invoke(EnumPlayers.None);
            _winImagePlayerOne.Show(EnumPlayers.None);
            _winImagePlayerTwo.Show(EnumPlayers.None);
            return EnumPlayers.None;
        }
        else if (activePlayers.Count == 1)
        {
            Debug.Log($"Победитель: {activePlayers[0]}!");
            OnWinnerDeclared?.Invoke(activePlayers[0]);
            _winImagePlayerOne.Show(activePlayers[0]);
            _winImagePlayerTwo.Show(activePlayers[0]);
            return activePlayers[0];
        }

        return null;
    }
    
    public Camera GetCameraPlayer(EnumPlayers player)
    {
        switch (player)
        {
            case EnumPlayers.None:
                Debug.LogError($"У этого {player} - Нет камеры!");
                return null;
            case EnumPlayers.PlayerOne:
                return _cameraPlayerOne;
            case EnumPlayers.PlayerTwo:
                return _cameraPlayerTwo;
            case EnumPlayers.PlayerThree:
                Debug.LogError($"У этого {player} - Нет камеры!");
                return null;
            default:
                Debug.LogError("Назначен не существующий Игрок!");
                return null;
        }
    }

    private void Start()
    {
        //// Стартовый активный игрок (пример)
        //ActivePlayer = EnumPlayers.PlayerTwo;
        //Debug.Log($"Стартовый персонаж назначен - это {ActivePlayer}!");
    }
}
