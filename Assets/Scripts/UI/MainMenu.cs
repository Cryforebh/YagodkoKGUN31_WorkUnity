using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenu : MonoBehaviour
{
    private SceneController _sceneController;
    private InputManager _inputManager;
    private SoundManager _soundManager;
    private MenuToggleManager _menuToggleManager;

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

    [Header("Обьект с затенением.")]
    [SerializeField] private FadeController _fadeController;

    private bool _isSelectPlay = false;

    private void Awake()
    {
        _menu.SetActive(false);

        HideAllLocalWindows();
        MenuShow();

        _fadeController.Show();
    }

    private void Start()
    {
        _inputManager.Contlols.Game.Menu.canceled += _ => MenuShow();
    }

    [Inject]
    private void InputManagerAndSceneController(InputManager inputManager, SceneController sceneController, SoundManager soundManager, MenuToggleManager menuToggleManager)
    {
        Debug.Log("Load InputManager And SceneController");
        _inputManager = inputManager;
        _sceneController = sceneController;
        _soundManager = soundManager;
        _menuToggleManager = menuToggleManager;
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
    }

    public void MenuShow()
    {
        HideAllLocalWindows();

        if (_menu.activeSelf == false) _menu.SetActive(true);
    }

    public void MenuHide()
    {
        _menu.SetActive(false);
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
        if (_isSelectPlay == true) return;

        MenuHide();

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
        if (_isSelectPlay == true) return;

        MenuHide();

        _seting.SetActive(true);
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
        _menuToggleManager.DynamicCamera = toggle.isOn;
    }

    public void ToggleMadnessMode(Toggle toggle)
    {
        _menuToggleManager.MadnessMode = toggle.isOn;
    }

    //--------------------------

    public void ButtonPlayGame()
    {
        if (_isSelectPlay == true) return;

        _isSelectPlay = true;
        _fadeController.Out();
        StartCoroutine(PlayFadeShow(1));
    }

    private void OnDestroy()
    {
        _inputManager.Contlols.Game.Menu.canceled -= _ => MenuShow();
    }

    private IEnumerator PlayFadeShow(float timeOut)
    {
        yield return new WaitForSeconds(timeOut);
        _sceneController.EnterPlayGame();
    }
}
