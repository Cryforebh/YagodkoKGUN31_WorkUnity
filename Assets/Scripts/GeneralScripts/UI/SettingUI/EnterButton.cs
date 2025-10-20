using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class EnterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Inject] private AdvancedCursorController _cursor;
    [Inject] private InputManager _inputManager;

    private void Start()
    {
        if (_cursor == null)
        {
            Debug.LogError("Cursor не назначен! Проверьте привязки Zenject.");
        }

        _inputManager.Controls.Game.Restart.canceled += Restart_started;
        _inputManager.Controls.Game.Menu.canceled += Menu_started;
    }

    private void Menu_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        DefaultCursor();
    }

    private void Restart_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        DefaultCursor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _cursor.SetCursorState(EnumStatusCursor.Select);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DefaultCursor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DefaultCursor();
    }

    private void DefaultCursor()
    {
        _cursor.SetCursorState(EnumStatusCursor.Default);
    }

    private void OnDestroy()
    {
        _inputManager.Controls.Game.Restart.canceled -= Restart_started;
        _inputManager.Controls.Game.Menu.canceled -= Menu_started;
    }
}
