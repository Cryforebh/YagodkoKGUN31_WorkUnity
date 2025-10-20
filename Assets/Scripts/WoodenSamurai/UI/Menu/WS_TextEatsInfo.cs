using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class WS_TextEatsInfo : MonoBehaviour
{
    [Inject] private LocalizationManager _localization;
    [Inject] private WS_SettingObjectManager _settingObjectManager;

    [SerializeField] private WS_EnumModifier _eatType;

    private TMP_Text _tMP_Text;

    private void Awake()
    {
        _tMP_Text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        _localization.ChangeLanguageEvent += _ => SetText();
    }

    private void OnEnable()
    {
        SetText();
    }

    private void SetText()
    {
        switch (_eatType)
        {
            case WS_EnumModifier.Drink:
                _tMP_Text.text = $"{_localization.GetText(EnumTextLocalization.sake)} " +
                    $"- {_localization.GetText(EnumTextLocalization.sake_info)}";
                break;
            case WS_EnumModifier.UpHelth:
                _tMP_Text.text = $"{_localization.GetText(EnumTextLocalization.sushi)} - " +
                    $"{_localization.GetText(EnumTextLocalization.sushi_info)}" +
                    $"{_settingObjectManager.MofifiSushiHealthUp}" +
                    $"{_localization.GetText(EnumTextLocalization.sushi_infotwo)}";
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        _localization.ChangeLanguageEvent -= _ => SetText();
    }
}
