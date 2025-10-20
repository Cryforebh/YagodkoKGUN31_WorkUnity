using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

public class RestartPanelVisual : MonoBehaviour
{
    [Inject] private SceneController _sceneController;
    [Inject] private InputManager _inputManager;
    private LocalizationManager _localization;

    [SerializeField] private TMP_Text _textComponentBack;
    [SerializeField] private float fillSpeed = 0.5f;

    private TMP_Text _textComponent;
    private float _fillAmount = 0f;
    private bool _isRestarting;
    private string _text;

    private void Start()
    {
        _inputManager.Controls.Game.Restart.started += StartRestart;
        _inputManager.Controls.Game.Restart.canceled += StopRestart;

        _localization.ChangeLanguageEvent += ChangeLanguageUpdateText;

        HidePanel();
    }

    [Inject]
    private void Construct(LocalizationManager localization)
    {
        _localization = localization;

        _textComponent = GetComponent<TMP_Text>();
        _text = _textComponent.text;

        SetTranslition();
    }

    private void ChangeLanguageUpdateText(int obj)
    {
        SetTranslition();
    }

    private void SetTranslition()
    {
        _text = _textComponent.text = _localization.GetText(EnumTextLocalization.menu_restart_button);
    }

    private void StartRestart(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _isRestarting = true;
        ShowPanel();
        StartCoroutine(RestartProgress());
    }

    private void StopRestart(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _isRestarting = false;
        HidePanel();
    }

    private void ShowPanel()
    {
        _textComponent.enabled = true;
        _textComponentBack.enabled = true;
    }

    private void HidePanel()
    {
        _textComponentBack.text = _text;
        _fillAmount = 0f;
        _textComponent.enabled = false;
        _textComponentBack.enabled = false;

    }

    private void FillScale(float time)
    {
        _fillAmount = Mathf.Clamp01(_fillAmount + time * fillSpeed);
    }

    private IEnumerator RestartProgress()
    {
        while (_isRestarting && _fillAmount < 1f)
        {
            FillScale(Time.deltaTime);
            // Обновляем текст в зависимости от _fillAmount
            string displayedText = _text.Substring(0, (int)(_text.Length * _fillAmount));

            _textComponent.text = displayedText;
            yield return null;
        }

        if (_fillAmount >= 1f)
        {
            _sceneController.RestartScene();
        }
    }

    private void OnDestroy()
    {
        _inputManager.Controls.Game.Restart.started -= StartRestart;
        _inputManager.Controls.Game.Restart.canceled -= StopRestart;

        _localization.ChangeLanguageEvent -= ChangeLanguageUpdateText;
    }
}
