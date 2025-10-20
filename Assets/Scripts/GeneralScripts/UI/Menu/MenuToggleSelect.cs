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

        _menuToggleManager.OnEnterToggleMenuEvent += SetValue;

        SetValue(_enumToggle);
    }

    public void SetValue(EnumToggleMenu enumToggle)
    {
        switch (_enumToggle)
        {
            case EnumToggleMenu.DynamicCamera:
                _toggle.isOn = _menuToggleManager.DynamicCamera;
                break;
            case EnumToggleMenu.ImmersiveObjects:
                _toggle.isOn = _menuToggleManager.ImmersiveObjects;
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

    private void OnDestroy()
    {
        _menuToggleManager.OnEnterToggleMenuEvent -= SetValue;
    }

}
