using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class InputManager : MonoBehaviour
{
    [Inject] private LevelRestartPanel _restartPanel;
    [Inject] private SceneController _sceneController;

    private GameInput _controls;
    private bool _isRestarting;

    private void Awake()
    {
        _controls = new GameInput();
        _controls.Game.Restart.started += Restart_started; 
        _controls.Game.Restart.canceled += _ => StopRestart(); // Альтернативный способ привязки
    }

    private void Restart_started(UnityEngine.InputSystem.InputAction.CallbackContext callbackContext)
    {
        StartRestart();
    }

    private void StartRestart()
    {
        if (_restartPanel == null) return;

        _isRestarting = true;
        _restartPanel.ShowPanel();
        StartCoroutine(RestartProgress());
    }

    private void StopRestart()
    {
        if (_restartPanel == null) return;

        _isRestarting = false;
        _restartPanel.HidePanel();
    }

    private IEnumerator RestartProgress()
    {
        while (_isRestarting && _restartPanel.fillAmount < 1f)
        {
            _restartPanel.FillScale(Time.deltaTime); // Заполняем на 1 единицу в секунду
            yield return null;
        }

        if (_restartPanel.fillAmount >= 1f)
        {
            _sceneController.RestartScene();
        }
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();
}
