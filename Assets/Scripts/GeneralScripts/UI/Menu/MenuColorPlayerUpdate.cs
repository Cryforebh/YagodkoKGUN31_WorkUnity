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

    private void UpdateLanguage()
    {
        _list = _dropdown.options;

        for (int i = 0; i < _list.Count; i++)
        {
            switch (i)
            {
                case 0:
                    _list[i].text = _localization.GetText(EnumTextLocalization.ui_color_red);
                    break;
                case 1:
                    _list[i].text = _localization.GetText(EnumTextLocalization.ui_color_blue);
                    break;
                case 2:
                    _list[i].text = _localization.GetText(EnumTextLocalization.ui_color_white);
                    break;
                case 3:
                    _list[i].text = _localization.GetText(EnumTextLocalization.ui_color_black);
                    break;
                case 4:
                    _list[i].text = _localization.GetText(EnumTextLocalization.ui_color_orange);
                    break;
                case 5:
                    _list[i].text = _localization.GetText(EnumTextLocalization.ui_color_green);
                    break;
                case 6:
                    _list[i].text = _localization.GetText(EnumTextLocalization.ui_color_cyan);
                    break;
                case 7:
                    _list[i].text = _localization.GetText(EnumTextLocalization.ui_color_violet);
                    break;
                default:
                    break;
            }
        }

        _dropdown.captionText.text = _list[_dropdown.value].text;
    }

    private void UpdateCount()
    {
        switch (_player)
        {
            case EnumPlayers.PlayerOne:
                _dropdown.value = _colorPlayersManager.CountColorPlayerOne;
                break;
            case EnumPlayers.PlayerTwo:
                _dropdown.value = _colorPlayersManager.CountColorPlayerTwo;
                break;
            default:
                break;
        }

    }

    private void OnDestroy()
    {
        _colorPlayersManager.Unsubscribe(ColorChangedUpdateCount);
        _localization.ChangeLanguageEvent -= ChangeLanguageTextUpdate;
    }
}
