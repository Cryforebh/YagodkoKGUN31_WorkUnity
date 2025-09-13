using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMaterialManager : MonoBehaviour
{
    [Header("Материалы - Выбора:")]
    [SerializeField] private Material _focusMaterialUnit;
    [SerializeField] private Material _selectMaterialUnit;
    [SerializeField] private Material _focusMaterialEnemyUnit;
    [Header("Материалы - Игроков:")]
    [SerializeField] private Material _playerOne;
    [SerializeField] private Material _playerTwo;
    [SerializeField] private Material _playerThree;

    public Material GetFocusMaterialUnit => _focusMaterialUnit;
    public Material GetSelectMaterialUnit => _selectMaterialUnit;
    public Material GetFocusMaterialEnemyUnit => _focusMaterialEnemyUnit;

    public Material GetMaterialPlayer(EnumPlayers player)
    {
        switch (player)
        {
            case EnumPlayers.PlayerOne:
                return _playerOne;
            case EnumPlayers.PlayerTwo:
                return _playerTwo;
            case EnumPlayers.PlayerThree:
                return _playerThree;
            default:
                Debug.LogError($"{player} - не найден! Присвоен материал Игрока номер 1 !");
                return _playerOne;
        }
    } 
}
