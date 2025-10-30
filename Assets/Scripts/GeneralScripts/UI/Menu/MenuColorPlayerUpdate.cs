using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class MenuColorPlayerUpdate : MonoBehaviour
{
    [Inject] private ColorPlayersManager _colorPlayersManager;
    [Inject] private LocalizationManager _localization;

    [SerializeField] private EnumPlayers _player;

    private TMP_Dropdown _dropdown;
    private List<TMP_Dropdown.OptionData> _list;

    private void Start()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
        UpdateCount();
        UpdateLanguage();
        _colorPlayersManager.Subscribe(ColorChangedUpdateCount);
        _localization.ChangeLanguageEvent += ChangeLanguageTextUpdate;
    }

    private void ChangeLanguageTextUpdate(int obj)
    {
        UpdateLanguage();
    }

    private void ColorChangedUpdateCount(Color obj)
    {
        UpdateCount();
    }

    // Тут я применил новый, для себя, способ реализации цикла Switch
    private void UpdateLanguage()
    {
        _list = _dropdown.options;

        for (int i = 0; i < _list.Count; i++)
        {
            _list[i].text = i switch 
            {
                0 => _localization.GetText(EnumTextLocalization.ui_color_red),
                1 => _localization.GetText(EnumTextLocalization.ui_color_blue),
                2 => _localization.GetText(EnumTextLocalization.ui_color_white),
                3 => _localization.GetText(EnumTextLocalization.ui_color_black),
                4 => _localization.GetText(EnumTextLocalization.ui_color_orange),
                5 => _localization.GetText(EnumTextLocalization.ui_color_green),
                6 => _localization.GetText(EnumTextLocalization.ui_color_cyan),
                7 => _localization.GetText(EnumTextLocalization.ui_color_violet),
                _ => "Dont is Color"
            };
        }
        _dropdown.captionText.text = _list[_dropdown.value].text;
    }

    private void UpdateCount()
    {
        _dropdown.value = _player switch
        {
            EnumPlayers.PlayerOne => _colorPlayersManager.CountColorPlayerOne,
            EnumPlayers.PlayerTwo => _colorPlayersManager.CountColorPlayerTwo,
            _ => throw new ArgumentException($"Неизвестный игрок: {_player}")
        };
    }

    private void OnDestroy()
    {
        _colorPlayersManager.Unsubscribe(ColorChangedUpdateCount);
        _localization.ChangeLanguageEvent -= ChangeLanguageTextUpdate;
    }
}
