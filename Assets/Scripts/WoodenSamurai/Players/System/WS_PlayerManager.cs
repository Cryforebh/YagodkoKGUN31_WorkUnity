using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class WS_PlayerManager : MonoBehaviour, IPlayerManager
{
    [Inject] private AllPlayer _players;

    [Inject] private WS_ContainerStatusGame _statusGame;
    [Inject] private WS_GameData _gameData;

    private EnumPlayers _activePlayer = EnumPlayers.None;

    private int _countToDraw = 16;
    private bool _draw = false;
    private bool _win = false;

    public event Action<EnumPlayers> OnActivePlayerChanged; // Событие изменения активного игрока
    public event Action<EnumPlayers> OnWinnerDeclared; // Событие Победы

    [SerializeField] private Camera _cameraPlayerOne;

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
            foreach (WS_Unit unit in playerEntry.Value)
            {
                unit.SetStatusUnit(isMy ? WS_EnumStatusUnitEnemy.My : WS_EnumStatusUnitEnemy.Enemy);
            }
        }
    }

    /// <summary>
    /// Проверка на победителя (Если он есть)
    /// </summary>
    /// <returns></returns>
    public EnumPlayers? CheckWinner()
    {
        // Фильтруем игроков с юнитами, исключая None
        var activePlayers = GetPlayersWhithUnits();

        if (activePlayers.Count == 0)
        {
            _gameData.Lock = true;
            _gameData.LockClick = true;

            _draw = true;
            Debug.Log("Все игроки уничтожены! Ничья!");
            OnWinnerDeclared?.Invoke(EnumPlayers.None);
            //_winImagePlayerOne.Show(EnumPlayers.None);

            return EnumPlayers.None;
        }
        else if (activePlayers.Count == 1)
        {
            _gameData.Lock = true;
            _gameData.LockClick = true;

            _win = true;
            Debug.Log($"Победитель: {activePlayers[0]}!");
            OnWinnerDeclared?.Invoke(activePlayers[0]);
            //_winImagePlayerOne.Show(activePlayers[0]);

            return activePlayers[0];
        }

        return null;
    }

    public Camera GetCameraPlayer(EnumPlayers player)
    {
        switch (player)
        {
            case EnumPlayers.PlayerOne:
                return _cameraPlayerOne;
            default:
                Debug.LogError("Назначен не существующий Игрок!");
                return null;
        }
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

    /// <summary>
    /// Смена игрока
    /// </summary>
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

        _statusGame.StatusUpdate(WS_EnumStatusGame.Empty);
        _gameData.Lock = false;
        _gameData.LockClick = false;
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
