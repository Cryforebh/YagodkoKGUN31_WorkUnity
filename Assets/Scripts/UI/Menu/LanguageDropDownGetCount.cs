
using TMPro;
using UnityEngine;
using Zenject;

public class LanguageDropDownGetCount : MonoBehaviour
{
    [Inject] private LocalizationManager _localization;

    private TMP_Dropdown _dropdown;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        SetCount();
    }

    private void SetCount()
    {
        _dropdown.value = _localization.GetCurrentCountLanguage;
    }
}
