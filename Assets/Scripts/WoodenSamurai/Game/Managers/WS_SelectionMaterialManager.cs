using System;
using UnityEngine;
using Zenject;

public class WS_SelectionMaterialManager : MonoBehaviour
{
    private ColorPlayersManager _color;

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

    public event Action<Material> MaterialChangedEvent;

    public Material PlayerOne => _playerOne;
    public Material PlayerTwo => _playerTwo;

    public void Awake()
    {
        MaterialUpdate();
        _color.Subscribe(ColorChangedMaterialUpdate);
    }

    private void ColorChangedMaterialUpdate(Color obj)
    {
        MaterialUpdate();
    }

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

    [Inject]
    public void Construct(ColorPlayersManager color)
    {
        _color = color;
        //_color.EventColorChangedReset();
        //color.UnsubscribeAll();
    }

    private void MaterialUpdate()
    {
        _playerOne.color = _color.PlayerOne;
        _playerTwo.color = _color.PlayerTwo;

        MaterialChangedEvent?.Invoke(_playerOne);
    }

    public void OnDestroy()
    {
        _color.Unsubscribe(ColorChangedMaterialUpdate);
    }
}
