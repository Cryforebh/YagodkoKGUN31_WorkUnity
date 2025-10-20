using System;

using UnityEngine;

public class GameEvent : MonoBehaviour
{
    public event Action<EnumGameEvent> OnStatusChanged; // Событие для отписки и подписки в случае изменения статуса игры

    private EnumGameEvent _enumStatusGame;
    public EnumGameEvent Status => _enumStatusGame;

    public void StatusUpdate(EnumGameEvent statusGame)
    {
        if (_enumStatusGame != statusGame)
        {
            _enumStatusGame = statusGame;
            OnStatusChanged?.Invoke(_enumStatusGame); // Триггерим событие
        }
    }
}
