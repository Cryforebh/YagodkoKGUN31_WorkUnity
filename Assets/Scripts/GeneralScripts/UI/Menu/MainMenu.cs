using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenu : MonoBehaviour
{
    private SceneController _sceneController;
    private InputManager _inputManager;
    private SoundManager _soundManager;
    private MenuToggleManager _menuToggleManager;
    private LocalizationManager _localization;
    private ColorPlayersManager _colorPlayersManager;

    [Header("Все окна меню")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _selectGames;
    [SerializeField] private GameObject _seting;
    [SerializeField] private GameObject _quality;
    [SerializeField] private GameObject _sounds;
    [SerializeField] private GameObject _game;
    [SerializeField] private GameObject _language;

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
        _inputManager.Controls.Game.Menu.canceled += _ => MenuShow();
    }

    [Inject]
    private void InputManagerAndSceneController(ColorPlayersManager colorPlayersManager, InputManager inputManager, SceneController sceneController, SoundManager soundManager, MenuToggleManager menuToggleManager, LocalizationManager localizationManager)
    {
        Debug.Log("Load InputManager And SceneController");
        _colorPlayersManager = colorPlayersManager;
        _inputManager = inputManager;
        _sceneController = sceneController;
        _soundManager = soundManager;
        _menuToggleManager = menuToggleManager;
        _localization = localizationManager;
    }

    private void HideAllLocalWindows()
    {
        _seting.SetActive(false);
        _quality.SetActive(false);
        _sounds.SetActive(false);
        _game.SetActive(false);
        _language.SetActive(false);
        _selectGames.SetActive(false);
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

    public void ToggleImmersiveObjects(Toggle toggle)
    {
        _menuToggleManager.ImmersiveObjects = toggle.isOn;
    }

    public void ValueColorPlayerOne(TMP_Dropdown dropdown)
    {
        _colorPlayersManager.SetColor(dropdown.value, EnumPlayers.PlayerOne);
    }

    public void ValueColorPlayerTwo(TMP_Dropdown dropdown)
    {
        _colorPlayersManager.SetColor(dropdown.value, EnumPlayers.PlayerTwo);
    }


    //--------------------------


    public void ButtonSelectGamesShow()
    {
        MenuHide();

        _seting.SetActive(false);
        _selectGames.SetActive(true);
    }

    public void ButtonPlayWS()
    {
        if (_isSelectPlay == true) return;

        _isSelectPlay = true;
        _fadeController.Out();
        _sceneController.EnterPlayGame(1);
        //StartCoroutine(PlayFadeShow(1));
        //StartCoroutine(LoadAsync(1, 1));
    }

    public void ButtonPlayCheckers()
    {
        if (_isSelectPlay == true) return;

        _isSelectPlay = true;
        _fadeController.Out();
        _sceneController.EnterPlayGame(2);
    }


    //--------------------------


    private void OnDestroy()
    {
        _inputManager.Controls.Game.Menu.canceled -= _ => MenuShow();
    }

    private IEnumerator PlayFadeShow(int sceneIndex, float timeOut)
    {
        yield return new WaitForSeconds(timeOut);
        _sceneController.EnterPlayGame(sceneIndex);
    }

    private IEnumerator LoadAsync(int sceneIndex, float timeOut)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false; // Запрещаем авто-переход

        yield return new WaitForSeconds(timeOut);

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true; // Разрешаем переход
            }

            yield return null;
        }
    }
}
