using UnityEngine;

public class InputManager : MonoBehaviour
{
    private GameInput _controls;

    public GameInput Contlols => _controls;

    private void Awake()
    {
        _controls = new GameInput();
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();
}
