using TMPro;
using UnityEngine;
using Zenject;

public class WinVisualPlayers : MonoBehaviour
{
    [Inject] LocalizationManager _localizationManager;
    [Inject] private ColorPlayersManager _colorContainer;
    [Inject] private IPlayerManager _playerManager;

    [SerializeField] private TMP_Text WinInfo;
    [SerializeField] private TMP_Text DrawInfo;
    [SerializeField] private TMP_Text PlayerOne;
    [SerializeField] private TMP_Text PlayerTwo;
    [SerializeField] private TMP_Text RestartInfo;
    [SerializeField] private TMP_Text MenuInfo;

    private Animation _animDrawInfo;
    private Animation _animWinInfo;
    private Animation _animRestartInfo;
    private Animation _animMenuInfo;

    private AudioSource _audioSource;

    private int countColorOne;
    private int countColorTwo;

    private void Awake()
    {
        _animDrawInfo = DrawInfo.GetComponent<Animation>();
        _animWinInfo = WinInfo.GetComponent<Animation>();
        _animRestartInfo = RestartInfo.GetComponent<Animation>();
        _animMenuInfo = MenuInfo.GetComponent<Animation>();

        _audioSource = GetComponent<AudioSource>();

        DrawInfo.enabled = false;
        WinInfo.enabled = false;
        PlayerOne.enabled = false;
        PlayerTwo.enabled = false;
        RestartInfo.enabled = false;
        MenuInfo.enabled = false;
    }

    private void Start()
    {
        _localizationManager.ChangeLanguageEvent += ChangeLanguageUpdateText;
        _colorContainer.ColorChangedEvent += ChangedUpdateColor;
        _playerManager.OnWinnerDeclared += WinnerShow;

        UpdateColorCount();
        UpdateTextWinInfo(PlayerOne);
        UpdateTextWinInfo(PlayerTwo);
        UpdateTextKey();
    }

    private void UpdateColorCount()
    {
        countColorOne = _colorContainer.CountColorPlayerOne;
        countColorTwo = _colorContainer.CountColorPlayerTwo;
    }

    private void UpdateTextWinInfo(TMP_Text player)
    {
        var countColor = 0;

        if (player == PlayerOne)
        {
            countColor = countColorOne;
        }
        else
        {
            countColor = countColorTwo;
        }

        switch (countColor)
        {
            case 0:
                player.text = _localizationManager.GetText(EnumTextLocalization.ui_color_red);
                break;
            case 1:
                player.text = _localizationManager.GetText(EnumTextLocalization.ui_color_blue);
                break;
            case 2:
                player.text = _localizationManager.GetText(EnumTextLocalization.ui_color_white);
                break;
            case 3:
                player.text = _localizationManager.GetText(EnumTextLocalization.ui_color_black);
                break;
            case 4:
                player.text = _localizationManager.GetText(EnumTextLocalization.ui_color_orange);
                break;
            case 5:
                player.text = _localizationManager.GetText(EnumTextLocalization.ui_color_green);
                break;
            case 6:
                player.text = _localizationManager.GetText(EnumTextLocalization.ui_color_cyan);
                break;
            case 7:
                player.text = _localizationManager.GetText(EnumTextLocalization.ui_color_violet);
                break;
            default:
                break;
        }
    }

    private void UpdateTextKey()
    {
        MenuInfo.text = $"{_localizationManager.GetText(EnumTextLocalization.win_menu)} \"Esc\"";

        if (RestartInfo)
            RestartInfo.text = $"{_localizationManager.GetText(EnumTextLocalization.win_restart)} \"R\"";
    }

    private void ChangedUpdateColor(Color obj)
    {
        UpdateColorCount();
        UpdateTextWinInfo(PlayerOne);
        UpdateTextWinInfo(PlayerTwo);
    }

    private void ChangeLanguageUpdateText(int obj)
    {
        UpdateColorCount();
        UpdateTextWinInfo(PlayerOne);
        UpdateTextWinInfo(PlayerTwo);
        UpdateTextKey();
    }

    private void WinnerShow(EnumPlayers player)
    {
        Show(player);
    }

    public void Show(EnumPlayers player)
    {
        RestartInfo.enabled = true;
        MenuInfo.enabled = true;

        if (!_playerManager.Draw)
        {
            WinInfo.enabled = true;

            switch (player)
            {
                case EnumPlayers.PlayerOne:
                    PlayerOne.enabled = true;
                    break;
                case EnumPlayers.PlayerTwo:
                    PlayerTwo.enabled = true;
                    break;
                default:
                    break;
            }

            _animWinInfo.Play();
        }
        else
        {
            DrawInfo.enabled = true;
            _animDrawInfo.Play();
        }

        _animRestartInfo.Play();
        _animMenuInfo.Play();

        _audioSource.Play();
    }

    private void OnDestroy()
    {
        _localizationManager.ChangeLanguageEvent -= ChangeLanguageUpdateText;
        _colorContainer.ColorChangedEvent -= ChangedUpdateColor;
        _playerManager.OnWinnerDeclared -= WinnerShow;
    }
}
