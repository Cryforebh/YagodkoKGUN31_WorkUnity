using TMPro;
using UnityEngine;
using Zenject;

public class TextNameLocalization : MonoBehaviour
{
    private LocalizationManager _localization;

    [SerializeField] private EnumTextLocalization _textLocalization = EnumTextLocalization.None;
    [SerializeField] private bool _isOnTwoName = false;
    [SerializeField] private EnumTextLocalization _textLocalizationTwo = EnumTextLocalization.None;

    private TMP_Text tMP_text;

    private void Awake()
    {

    }

    [Inject]
    private void Construct(LocalizationManager localization)
    {
        _localization = localization;

        tMP_text = GetComponent<TMP_Text>();
        SetTranslition();

        _localization.ChangeLanguageEvent += ChangeLanguageUpdateText;
    }

    private void ChangeLanguageUpdateText(int obj)
    {
        SetTranslition();
    }

    private void SetTranslition()
    {
        if (!_isOnTwoName)
            tMP_text.text = _localization.GetText(_textLocalization);
        else
        {
            tMP_text.text = $"{_localization.GetText(_textLocalization)} {_localization.GetText(_textLocalizationTwo)}";
        }
    }

    private void OnDestroy()
    {
        _localization.ChangeLanguageEvent -= ChangeLanguageUpdateText;
    }
}
