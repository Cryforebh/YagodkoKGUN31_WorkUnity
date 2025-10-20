using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlayersManager : MonoBehaviour
{
    [Header("Материалы игроков:")]
    [SerializeField] private Material _materialPlayerOne;
    [SerializeField] private Material _materialPlayerTwo;

    [Header("Цвета игроков:")]
    [SerializeField] private Color _playerOne;
    [SerializeField] private Color _playerTwo;

    [SerializeField] private Color _red;
    [SerializeField] private Color _blue;
    [SerializeField] private Color _white;
    [SerializeField] private Color _black;
    [SerializeField] private Color _orange;
    [SerializeField] private Color _green;
    [SerializeField] private Color _cyan;
    [SerializeField] private Color _violet;

    private Color _targetColor;
    private int _targetCount;

    private Color _oldColor;
    private int _oldCount;

    public Color Red => _red;
    public Color Blue => _blue;
    public Color Orange => _orange;
    public Color White => _white;
    public Color Black => _black;
    public Color Green => _green;
    public Color Cyan => _cyan;
    public Color Violet => _violet;

    public Color PlayerOne { get => _playerOne; set => _playerOne = value; }
    public Color PlayerTwo { get => _playerTwo; set => _playerTwo = value; }

    public Material MaterialPlayerOne => _materialPlayerOne;
    public Material MaterialPlayerTwo => _materialPlayerTwo;

    public int CountColorPlayerOne = 0;
    public int CountColorPlayerTwo = 1;

    public event Action<Color> ColorChangedEvent;

    public void EventColorChangedReset()
    {
        ColorChangedEvent = null;
    }

    private void Awake()
    {
        SetColor(CountColorPlayerOne, EnumPlayers.PlayerOne);
        SetColor(CountColorPlayerTwo, EnumPlayers.PlayerTwo);
    }

    public void SetColor(int count, EnumPlayers player)
    {
        switch (count)
        {

            case 0:
                _targetColor = _red;
                _targetCount = 0;
                break;
            case 1:
                _targetColor = _blue;
                _targetCount = 1;
                break;
            case 2:
                _targetColor = _white;
                _targetCount = 2;
                break;
            case 3:
                _targetColor = _black;
                _targetCount = 3;
                break;
            case 4:
                _targetColor = _orange;
                _targetCount = 4;
                break;
            case 5:
                _targetColor = _green;
                _targetCount = 5;
                break;
            case 6:
                _targetColor = _cyan;
                _targetCount = 6;
                break;
            case 7:
                _targetColor = _violet;
                _targetCount = 7;
                break;
            default:
                break;
        }

        switch (player)
        {
            case EnumPlayers.PlayerOne:
                _oldColor = _playerOne;
                _oldCount = CountColorPlayerOne;

                _playerOne = _targetColor;
                CountColorPlayerOne = _targetCount;
                _materialPlayerOne.color = _playerOne;
                ColorChangedEvent?.Invoke(_playerOne);

                if (CountColorPlayerTwo == CountColorPlayerOne)
                {
                    _playerTwo = _oldColor;
                    CountColorPlayerTwo = _oldCount;
                    _materialPlayerTwo.color = _playerTwo;
                    ColorChangedEvent?.Invoke(_playerTwo);
                }
                break;
            case EnumPlayers.PlayerTwo:
                _oldColor = _playerTwo;
                _oldCount = CountColorPlayerTwo;

                _playerTwo = _targetColor;
                CountColorPlayerTwo = _targetCount;
                _materialPlayerTwo.color = _playerTwo;
                ColorChangedEvent?.Invoke(_playerTwo);

                if (CountColorPlayerTwo == CountColorPlayerOne)
                {
                    _playerOne = _oldColor;
                    CountColorPlayerOne = _oldCount;
                    _materialPlayerOne.color = _playerOne;
                    ColorChangedEvent?.Invoke(_playerOne);
                }
                break;
            default:
                break;
        }
    }

    public int GetCountCurrentCollorPlayer(EnumPlayers player)
    {
        if (player == EnumPlayers.PlayerOne)
        {
            return CountColorPlayerOne;
        }
        else
        {
            return CountColorPlayerTwo;
        }
    }

    private List<Action<Color>> _subscribers = new List<Action<Color>>();


    public void Subscribe(Action<Color> action)
    {
        ColorChangedEvent += action;
        _subscribers.Add(action);
    }

    public void Unsubscribe(Action<Color> action)
    {
        foreach (var subscriber in _subscribers)
        {
            if (action == subscriber)
            ColorChangedEvent -= subscriber;
        }
    }

    public void UnsubscribeAll()
    {
        foreach (var subscriber in _subscribers)
        {
            ColorChangedEvent -= subscriber;
        }
        _subscribers.Clear();
    }
}
