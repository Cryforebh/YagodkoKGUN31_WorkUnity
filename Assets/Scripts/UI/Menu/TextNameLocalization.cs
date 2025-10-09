using TMPro;
using UnityEngine;
using Zenject;

public class TextNameLocalization : MonoBehaviour
{
    [Inject] private LocalizationManager _localization;

    [SerializeField] private EnumTextLocalization _textLocalization = EnumTextLocalization.None;

    private TMP_Text tMP_text;

    private void Awake()
    {
        tMP_text = GetComponent<TMP_Text>();

        _localization.ChangeLanguageEvent += _ => SetTranslition();
    }

    private void Start()
    {
        SetTranslition();
    }

    private void SetTranslition()
    {
        tMP_text.text = _localization.GetText(_textLocalization);
    }

    private void OnDestroy()
    {
        _localization.ChangeLanguageEvent -= _ => SetTranslition();
    }
}
