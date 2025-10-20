using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class GameMenu : StandardMenu
{
    [Inject] private ColorPlayersManager _colorPlayerManager;
    [Inject] private ISpawner _spawner;

    private SceneController _sceneController;
    private InputManager _inputManager;
    private SoundManager _soundManager;
    private MenuToggleManager _menuToggleManager;
    private LocalizationManager _localization;

    [Header("Все окна меню")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _seting;
    [SerializeField] private GameObject _quality;
    [SerializeField] private GameObject _sounds;
    [SerializeField] private GameObject _game;
    [SerializeField] private GameObject _language;
    [SerializeField] private GameObject _whoGoesFirst;
    [SerializeField] private GameObject _manual;

    [Header("Обьект с затенением, для плавного перехода из одной сцены в другую.")]
    [SerializeField] private FadeController _fadeControllerM;
    [SerializeField] private FadeController _fadeControllerG;

    [Header("Канвас который отображает все меню.")]
    [SerializeField] private Canvas _canvas;

    [Header("Камеры отображения (игры или меню)")]
    [SerializeField] private GameObject _cameraMenu;
    [SerializeField] private GameObject _cameraGame;

    private Camera _cameraM;
    private Camera _cameraG;
    private PhysicsRaycaster _physicsRaycasterMenu;
    private PhysicsRaycaster _physicsRaycasterGame;

    private bool _isSelectedFirstPlayer = true;
    private bool _isEbdSelectedPlayer = false;

    private int _backSector = 0;

    public override event Action<bool> OnMenuEvent;

    private void Awake()
    {
        HideAllSections();
        MenuHide();

        CameraMenuShow(true);
        _fadeControllerM.Show();

        SelectedFirstPlayerShow();
    }

    private void Start()
    {
        _inputManager.Controls.Game.Menu.canceled += _ => ButtonBack();
    }

    [Inject]
    private void Construct(InputManager inputManager, SceneController sceneController, SoundManager soundManager,
        MenuToggleManager menuToggleManager, LocalizationManager localizationManager)
    {
        Debug.Log("Load InputManager And SceneController");
        _inputManager = inputManager;
        _sceneController = sceneController;
        _soundManager = soundManager;
        _menuToggleManager = menuToggleManager;
        _localization = localizationManager;

        _cameraM = _cameraMenu.GetComponent<Camera>();
        _physicsRaycasterMenu = _cameraMenu.GetComponent<PhysicsRaycaster>();
        _cameraM.enabled = false;

        _cameraG = _cameraGame.GetComponent<Camera>();
        _physicsRaycasterGame = _cameraGame.GetComponent<PhysicsRaycaster>();
    }

    private void CameraMenuShow(bool value)
    {
        _cameraM.enabled = value;
        _physicsRaycasterMenu.enabled = value;

        _cameraG.enabled = !value;
        _physicsRaycasterGame.enabled = !value;

        _canvas.enabled = value;
    }

    private void HideAllSections()
    {
        _seting.SetActive(false);
        _quality.SetActive(false);
        _sounds.SetActive(false);
        _game.SetActive(false);
        _language.SetActive(false);
        _whoGoesFirst.SetActive(false);

        if (_manual != null) _manual.SetActive(false);
    }

    public void MenuShow()
    {
        HideAllSections();

        if (_menu.activeSelf == false)
        {
            _menu.SetActive(true);
            CameraMenuShow(true);

            _menuToggleManager.IsEnterMenu = true;
            _onMenu = true;
        }
        else
        {
            _menu.SetActive(false);
            CameraMenuShow(false);

            _menuToggleManager.IsEnterMenu = false;
            _onMenu = false;
        }
        OnMenuEvent?.Invoke(_onMenu);
    }

    public void MenuHide()
    {
        _menu.SetActive(false);
    }

    public void ButtonContinue()
    {
        MenuHide();

        SetBackSector(1); // Чтобы использовать ButtonBack() для открытия меню из игры
        CameraMenuShow(false);
    }

    public void ButtonSettingShow()
    {
        HideAllSections();
        MenuHide();

        SetBackSector(1);
        _seting.SetActive(true);
    }

    public void ButtonRestart()
    {
        _sceneController.RestartScene();
    }

    public void ButtonManualShow()
    {
        HideAllSections();
        MenuHide();

        SetBackSector(1);
        _manual.SetActive(true);
    }

    public void ButtonExitOfMainMenu()
    {
        _sceneController.ExitOfMainMenu();
    }

    public void ButtonExit()
    {
        Application.Quit();
    }

    public void ButtonBack()
    {
        if (_isSelectedFirstPlayer) return;
        if (!_isEbdSelectedPlayer) return;

        if (_backSector == 0) ButtonContinue();
        else if (_backSector == 1) MenuShow();
        else if (_backSector == 2) ButtonSettingShow();
    }

    //--------------------------

    public void ButtonQuality()
    {
        MenuHide();
        SetBackSector(2);
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

        SetBackSector(2);
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

        SetBackSector(2);
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
        _colorPlayerManager.SetColor(dropdown.value, EnumPlayers.PlayerOne);
    }

    public void ValueColorPlayerTwo(TMP_Dropdown dropdown)
    {
        _colorPlayerManager.SetColor(dropdown.value, EnumPlayers.PlayerTwo);
    }

    //----------------------------------------------

    public void ButtonLanguageShow()
    {
        MenuHide();

        SetBackSector(2);
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
        _whoGoesFirst.SetActive(true);
    }

    public void ButtonFirstPlayerOne()
    {
        if (_isSelectedFirstPlayer == false) return;
        InitializableGame(EnumPlayers.PlayerOne);
    }

    public void ButtonFirstPlayerTwo()
    {
        if (_isSelectedFirstPlayer == false) return;
        InitializableGame(EnumPlayers.PlayerTwo);
    }

    private void InitializableGame(EnumPlayers player)
    {
        // Здесь логика спавна и установка первоходящего игрока
        if (_spawner != null)
        {
            _spawner.SetWhoGoesFirst(player);
            _spawner.CreateUnits();
        }

        _isSelectedFirstPlayer = false;

        _fadeControllerM.Out();
        StartCoroutine(PlayFadeShow(1));
    }

    private IEnumerator PlayFadeShow(float timeOut)
    {
        yield return new WaitForSeconds(timeOut);

        _whoGoesFirst.SetActive(false);
        CameraMenuShow(false);
        SetBackSector(1); // Чтобы использовать ButtonBack() для открытия меню из игры
        _fadeControllerG.Show();

        _isEbdSelectedPlayer = true;
    }

    private void SetBackSector(int sector)
    {
        _backSector = sector;
    }

    private void OnDestroy()
    {
        _inputManager.Controls.Game.Menu.canceled -= _ => ButtonBack();
    }
}
