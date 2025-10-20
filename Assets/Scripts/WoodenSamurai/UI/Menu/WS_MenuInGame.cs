using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class WS_MenuInGame : StandardMenu
{
    [Inject] private InputManager _levelManager;
    [Inject] private SceneController _sceneController;
    [Inject] private WS_SpawnUnitOnCell _spawn;
    [Inject] private SoundManager _soundManager;
    [Inject] private MenuToggleManager _menuToggleManager;
    [Inject] private LocalizationManager _localization;
    [Inject] private ColorPlayersManager _colorPlayersManager;

    [SerializeField] private Canvas _canvas;

    [SerializeField] private GameObject _cameraMenu;
    [SerializeField] private GameObject _cameraGame;

    [Header("Все окна меню")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _manual;
    [SerializeField] private GameObject _manualUnits;
    [SerializeField] private GameObject _manualModifi;
    [SerializeField] private GameObject _manualEat;
    [SerializeField] private GameObject _seting;
    [SerializeField] private GameObject _quality;
    [SerializeField] private GameObject _sounds;
    [SerializeField] private GameObject _game;
    [SerializeField] private GameObject _language;

    [SerializeField] private GameObject _whoGoesFirst;

    [Header("Обьект с затенением.")]
    [SerializeField] private FadeController _fadeControllerM;
    [SerializeField] private FadeController _fadeControllerG;

    private Camera _cameraM;
    private Camera _cameraG;
    private PhysicsRaycaster _physicsRaycasterMenu;
    private PhysicsRaycaster _physicsRaycasterGame;

    private bool _isSelectedFirstPlayer = true;
    private bool _isEbdSelectedPlayer = false;

    public override event Action<bool> OnMenuEvent;

    private void Awake()
    {
        HideAllLocalWindows();

        _cameraM = _cameraMenu.GetComponent<Camera>();
        _physicsRaycasterMenu = _cameraMenu.GetComponent<PhysicsRaycaster>();
        _cameraM.enabled = false;

        _cameraG = _cameraGame.GetComponent<Camera>();
        _physicsRaycasterGame = _cameraGame.GetComponent<PhysicsRaycaster>();

        _menu.SetActive(false);
        CameraMenuShow(true);

        _fadeControllerM.Show();
        SelectedFirstPlayerShow();
    }

    private void Start()
    {
        _levelManager.Controls.Game.Menu.canceled += _ => MenuShow();
    }

    private void CameraMenuShow(bool value)
    {
        if (value)
        {
            _canvas.enabled = true;

            _cameraM.enabled = true;
            _physicsRaycasterMenu.enabled = true;
            _onMenu = true;

            _cameraG.enabled = false;
            _physicsRaycasterGame.enabled = false;

        }
        else
        {
            _canvas.enabled = false;

            _cameraM.enabled = false;
            _physicsRaycasterMenu.enabled = false;
            _onMenu = false;

            _cameraG.enabled = true;
            _physicsRaycasterGame.enabled = true;
        }
        OnMenuEvent?.Invoke(_onMenu);
    }

    private void HideAllLocalWindows()
    {
        _manual.SetActive(false);
        _manualUnits.SetActive(false);
        _manualModifi.SetActive(false);
        _manualEat.SetActive(false);
        _seting.SetActive(false);
        _quality.SetActive(false);
        _sounds.SetActive(false);
        _game.SetActive(false);
        _language.SetActive(false);
    }

    public void MenuShow()
    {
        if (_isSelectedFirstPlayer) return;
        if (!_isEbdSelectedPlayer) return;

        HideAllLocalWindows();

        if (_menu.activeSelf == false)
        {
            _menu.SetActive(true);
            CameraMenuShow(true);

            _menuToggleManager.IsEnterMenu = true;
        }
        else
        {
            _menu.SetActive(false);
            CameraMenuShow(false);

            _menuToggleManager.IsEnterMenu = false;
        }
    }


    public void ExitOfMainMenu()
    {
        _sceneController.ExitOfMainMenu();
    }

    public void MenuShowForcedly()
    {
        HideAllLocalWindows();

        _menu.SetActive(true);
        CameraMenuShow(true);
    }

    public void MenuHide()
    {
        _menu.SetActive(false);
    }

    public void MenuRestart()
    {
        _sceneController.RestartScene();
    }

    public void ButtonBack()
    {
        MenuShow();
    }

    public void ButtonExit()
    {
        Application.Quit();
    }

    public void ButtonManualShow()
    {
        _menu.SetActive(false);
        _manual.SetActive(true);
        _manualUnits.SetActive(true);
    }

    public void ButtonManualLeft()
    {
        if (_manualUnits.activeSelf == true)
        {
            _manualUnits.SetActive(false);
            _manualEat.SetActive(true);
            return;
        }
        else if (_manualEat.activeSelf == true)
        {
            _manualEat.SetActive(false);
            _manualModifi.SetActive(true);
            return;
        }
        else if (_manualModifi.activeSelf == true)
        {
            _manualModifi.SetActive(false);
            _manualUnits.SetActive(true);
            return;
        }
    }

    public void ButtonManualRight()
    {
        if (_manualUnits.activeSelf == true)
        {
            _manualUnits.SetActive(false);
            _manualModifi.SetActive(true);
            return;
        }
        else if (_manualModifi.activeSelf == true)
        {
            _manualModifi.SetActive(false);
            _manualEat.SetActive(true);
            return;
        }
        else if (_manualEat.activeSelf == true)
        {
            _manualEat.SetActive(false);
            _manualUnits.SetActive(true);
            return;
        }
    }

    public void ButtonSettingShow()
    {
        _menu.SetActive(false);
        _seting.SetActive(true);
    }

    public void ButtonContinue()
    {
        MenuShow();
        CameraMenuShow(false);
    }

    //--------------------------

    public void ButtonQuality()
    {
        MenuHide();

        _seting.SetActive(false);
        _quality.SetActive(true);
    }

    public void ButtonQualityVeryLow()
    {
        QualitySettings.SetQualityLevel(0);
    }
    public void ButtonQualityLow()
    {
        QualitySettings.SetQualityLevel(1);
    }
    public void ButtonQualityMedium()
    {
        QualitySettings.SetQualityLevel(2);
    }
    public void ButtonQualityHigh()
    {
        QualitySettings.SetQualityLevel(3);
    }
    public void ButtonQualityVeryHigh()
    {
        QualitySettings.SetQualityLevel(4);
    }
    public void ButtonQualityUltra()
    {
        QualitySettings.SetQualityLevel(5);
    }

    //--------------------------

    public void ButtonSoundsShow()
    {
        MenuHide();

        _seting.SetActive(false);
        _sounds.SetActive(true);
    }

    public void ValueSoundUnits(Slider slider)
    {
        _soundManager.ValumeUnits = slider.value;
    }

    public void ValueSoundModifications(Slider slider)
    {
        _soundManager.ValumeModifications = slider.value;
    }

    public void ValueSoundAmbient(Slider slider)
    {
        _soundManager.ValumeAmbient = slider.value;
    }

    public void ValueSoundOther(Slider slider)
    {
        _soundManager.ValumeOther = slider.value;
    }

    //--------------------------

    public void ButtonGameShow()
    {
        MenuHide();

        _seting.SetActive(false);
        _game.SetActive(true);
    }

    public void ToggleDynamicCamera(Toggle toggle)
    {
        _menuToggleManager.UpdateToggle(EnumToggleMenu.DynamicCamera, toggle.isOn);
    }

    public void ToggleMadnessMode(Toggle toggle)
    {
        _menuToggleManager.UpdateToggle(EnumToggleMenu.MadnessMode, toggle.isOn);
    }

    public void ToggleImmersiveObjects(Toggle toggle)
    {
        _menuToggleManager.UpdateToggle(EnumToggleMenu.ImmersiveObjects, toggle.isOn);
    }

    public void ValueColorPlayerOne(TMP_Dropdown dropdown)
    {
        _colorPlayersManager.SetColor(dropdown.value, EnumPlayers.PlayerOne);
    }

    public void ValueColorPlayerTwo(TMP_Dropdown dropdown)
    {
        _colorPlayersManager.SetColor(dropdown.value, EnumPlayers.PlayerTwo);
    }

    //----------------------------------------------

    public void ButtonLanguageShow()
    {
        MenuHide();

        _seting.SetActive(false);
        _language.SetActive(true);
    }

    public void ValueLanguage(TMP_Dropdown dropdown)
    {
        _localization.Language(dropdown.value);
    }

    ////////////////////////////////////////////

    public void SelectedFirstPlayerShow()
    {
        _menu.SetActive(false);
        _whoGoesFirst.SetActive(true);
    }

    public void SelectedFirstPlayerOne()
    {
        if (_isSelectedFirstPlayer == false) return;

        _spawn.SetWhoGoesFirst(EnumPlayers.PlayerOne);
        _spawn.CreateUnits();

        _isSelectedFirstPlayer = false;

        _fadeControllerM.Out();
        StartCoroutine(PlayFadeShow(1));
    }

    public void SelectedFirstPlayerTwo()
    {
        if (_isSelectedFirstPlayer == false) return;

        _spawn.SetWhoGoesFirst(EnumPlayers.PlayerTwo);
        _spawn.CreateUnits();

        _isSelectedFirstPlayer = false;

        _fadeControllerM.Out();
        StartCoroutine(PlayFadeShow(1));
    }

    private void OnDestroy()
    {
        //_levelManager.Contlols.Game.Menu.canceled -= _ => MenuShow();
        _levelManager.Controls.Game.Menu.canceled -= _ => MenuShow();
    }

    private IEnumerator PlayFadeShow(float timeOut)
    {
        yield return new WaitForSeconds(timeOut);

        _whoGoesFirst.SetActive(false);
        CameraMenuShow(false);
        _fadeControllerG.Show();

        _isEbdSelectedPlayer = true;
    }
}
