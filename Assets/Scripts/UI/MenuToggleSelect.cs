using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuToggleSelect : MonoBehaviour
{
    [SerializeField] private EnumToggleMenu _enumToggle;

    private MenuToggleManager _menuToggleManager;
    private Toggle _toggle;

    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        SetValue(_enumToggle);
    }

    public void SetValue(EnumToggleMenu enumToggle)
    {
        switch (enumToggle)
        {
            case EnumToggleMenu.None:
                break;
            case EnumToggleMenu.DynamicCamera:
                _toggle.isOn = _menuToggleManager.DynamicCamera;
                break;
            case EnumToggleMenu.Woman:
                break;
            case EnumToggleMenu.MadnessMode:
                _toggle.isOn = _menuToggleManager.MadnessMode;
                break;
            default:
                break;
        }
    }

    [Inject]
    private void SetSoundManager(MenuToggleManager menuToggleManager)
    {
        _menuToggleManager = menuToggleManager;
    }

}
