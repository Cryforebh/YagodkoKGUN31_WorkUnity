using System;
using UnityEngine;

public class WS_ContainerStatusGame : MonoBehaviour
{
    public event Action<WS_EnumStatusGame> OnStatusChanged; // Событие для отписки и подписки в случае изменения статуса игры

    private WS_EnumStatusGame _enumStatusGame;
    public WS_EnumStatusGame Status => _enumStatusGame;

    public void StatusUpdate(WS_EnumStatusGame statusGame)
    {
        if (_enumStatusGame != statusGame)
        {
            _enumStatusGame = statusGame;
            OnStatusChanged?.Invoke(_enumStatusGame); // Триггерим событие
        }
    }
}
