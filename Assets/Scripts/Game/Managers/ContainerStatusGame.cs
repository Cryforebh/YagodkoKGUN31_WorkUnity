using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerStatusGame : MonoBehaviour
{
    public event Action<EnumStatusGame> OnStatusChanged; // Событие для отписки и подписки в случае изменения статуса игры

    private EnumStatusGame _enumStatusGame;
    public EnumStatusGame Status => _enumStatusGame;
    public int StatusInt => (int)_enumStatusGame;

    public UnitSelectionPointer LastSelectedPointer { get; set; }

    public void StatusUpdateInt(int statusGame)
    {
        if (_enumStatusGame != (EnumStatusGame)statusGame)
        {
            _enumStatusGame = (EnumStatusGame)statusGame;
            OnStatusChanged?.Invoke(_enumStatusGame); // Триггерим событие
        }
    }

    public void StatusUpdate(EnumStatusGame statusGame)
    {
        if (_enumStatusGame != statusGame)
        {
            _enumStatusGame = statusGame;
            OnStatusChanged?.Invoke(_enumStatusGame); // Триггерим событие
        }
    }
}
