using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class C_PlayerManager : MonoBehaviour, IPlayerManager
{
    [Inject] private AllPlayer _players;
    [Inject] private GameEvent _gameEvent;
    [Inject] private IGameData _gameData;
    [Inject] private C_Battlefield _battlefield;

    private EnumPlayers _activePlayer = EnumPlayers.None;

    private int _countToDraw = 28;
    private bool _draw = false;
    private bool _win = false;
    private bool _playerOneNoGo = false;
    private bool _playerTwoNoGo = false;
    private bool _isFirstMove = true;

    public event Action<EnumPlayers> OnActivePlayerChanged; // Событие изменения активного игрока
    public event Action<EnumPlayers> OnWinnerDeclared; // Событие Победы

    public int CountToDraw => _countToDraw;
    public bool Draw => _draw;
    public bool Win => _win;


    public EnumPlayers ActivePlayer
    {
        get => _activePlayer;
        set
        {
            if (_activePlayer != value)
            {
                _activePlayer = value;
                UpdateAllUnitsStatusEnemy(); // Обновляем статусы юнитов

                if (_isFirstMove)
                {
                    _isFirstMove = false;
                    DeterminingPossibilityOfMoveAllPlayers();
                }

                OnActivePlayerChanged?.Invoke(_activePlayer); // Вызываем событие
                Debug.Log($"Ходит - {_activePlayer}!");
            }
        }
    }

    private void DeterminingPossibilityOfMoveActivePlayer()
    {
        _battlefield.CheckAvailableAllCells(_activePlayer);

        if (_activePlayer == EnumPlayers.PlayerOne) _playerOneNoGo = !_battlefield.IsAvailableCellPlayer;
        else if (_activePlayer == EnumPlayers.PlayerTwo) _playerTwoNoGo = !_battlefield.IsAvailableCellPlayer;
    }

    private void DeterminingPossibilityOfMoveAllPlayers()
    {
        _battlefield.CheckAvailableAllCells(EnumPlayers.PlayerOne);
        _playerOneNoGo = !_battlefield.IsAvailableCellPlayer;

        _battlefield.CheckAvailableAllCells(EnumPlayers.PlayerTwo);
        _playerTwoNoGo = !_battlefield.IsAvailableCellPlayer;
    }

    private void UpdateAllUnitsStatusEnemy()
    {
        foreach (var playerEntry in _players.PlayersCollection)
        {
            bool isFriendly = playerEntry.Key == _activePlayer;
            foreach (IUnitMain unit in playerEntry.Value)
            {
                if (isFriendly) unit.IsEnemy = false;
                else unit.IsEnemy = true;
            }
        }
    }

    public EnumPlayers? CheckWinner()
    {
        _gameData.Lock = true;
        _gameData.LockClick = true;

        var activePlayers = GetPlayersWhithUnits();

        if (activePlayers.Count == 0)
        {
            EndGame(EnumPlayers.None, "Ничья!");
            return EnumPlayers.None;
        }
        else if (activePlayers.Count == 1)
        {
            EndGame(activePlayers[0], $"Победитель: {activePlayers[0]}!");
            return activePlayers[0];
        }

        // Если у какого-то игрока одна фигура, то будет идти счетчик ходов, если счетчик закончится, то - Ничья!
        if (IsCountOneUnit())
        {
            _countToDraw -= 1;
            if (_countToDraw <= 0)
            {
                EndGame(EnumPlayers.None, "Ничья!");
                return EnumPlayers.None;
            }
        }

        //DeterminingPossibilityOfMoveActivePlayer();
        DeterminingPossibilityOfMoveAllPlayers();

        if (_playerTwoNoGo && _playerOneNoGo)
        {
            EndGame(EnumPlayers.None, "Ничья!");
            return EnumPlayers.None;
        }
        else if (_playerOneNoGo)
        {
            EndGame(activePlayers[1], $"Победитель: {activePlayers[1]}!");
            return activePlayers[1];
        }
        else if (_playerTwoNoGo)
        {
            EndGame(activePlayers[0], $"Победитель: {activePlayers[0]}!");
            return activePlayers[0];
        }

        _gameData.Lock = false;
        _gameData.LockClick = false;

        return null;
    }

    private void EndGame(EnumPlayers player, string message)
    {
        if (_draw || _win) return;

        //_gameData.Lock = true;
        //_gameData.LockClick = true;
        _draw = player == EnumPlayers.None;
        _win = player != EnumPlayers.None;
        Debug.Log(message);
        OnWinnerDeclared?.Invoke(player);

    }

    public List<EnumPlayers> GetPlayersWhithUnits()
    {
        // Фильтруем игроков с юнитами, исключая None
        var activePlayers = _players.PlayersCollection
            .Where(p => p.Key != EnumPlayers.None && p.Value.Count > 0)
            .Select(p => p.Key)
            .ToList();

        return activePlayers;
    }

    public bool IsCountOneUnit()
    {
        var activePlayers = _players.PlayersCollection
        .Where(p => p.Key != EnumPlayers.None && p.Value.Count == 1)
        .Select(p => p.Key)
        .ToList();
        return activePlayers.Count == 1;
    }

    public void ChangePlayer()
    {
        _gameData.LockClick = true;

        if (_win == true || _draw == true) return;

        StartCoroutine(TimeOutForLockAndChange());
        StartCoroutine(ProcessChangePlayer());
    }

    private IEnumerator ProcessChangePlayer()
    {
        yield return new WaitForSeconds(1.5f);

        _gameEvent.StatusUpdate(EnumGameEvent.Empty);

        if (!_draw && !_win)
        {
            _gameData.Lock = false;
            _gameData.LockClick = false;
        }
    }

    private IEnumerator TimeOutForLockAndChange()
    {
        yield return new WaitForSeconds(0.4f);

        _gameData.Lock = true;

        if (ActivePlayer == EnumPlayers.PlayerTwo)
        {
            ActivePlayer = EnumPlayers.PlayerOne;
        }
        else if (ActivePlayer == EnumPlayers.PlayerOne)
        {
            ActivePlayer = EnumPlayers.PlayerTwo;
        }
    }
}
