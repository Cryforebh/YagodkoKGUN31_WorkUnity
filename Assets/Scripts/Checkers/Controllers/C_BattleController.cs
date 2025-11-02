using UnityEngine;
using Zenject;

public class C_BattleController : MonoBehaviour
{
    [Inject] private C_Battlefield _battlefield;
    [Inject] private InputManager _inputManager;

    [SerializeField] private StandardMenu _menu;

    private void Start()
    {
        Construct();
    }

    private void Construct()
    {
        if (_menu != null)
            _inputManager.Controls.Game.Menu.canceled += DeselectUnitControl;
    }

    private void DeselectUnitControl(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ProccesDeselectUnit();
    }

    private void ProccesDeselectUnit()
    {
        if (_menu.OnMenu == false)
            _battlefield.DeselectUnit();
    }

    private void OnDestroy()
    {
        if (_menu != null)
        _inputManager.Controls.Game.Menu.canceled -= DeselectUnitControl;
    }
}
