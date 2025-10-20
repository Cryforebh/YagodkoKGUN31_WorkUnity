using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TurnVisualPlayers : MonoBehaviour
{
    [Inject] LocalizationManager _localizationManager;
    [Inject] private ColorPlayersManager _colorContainer;
    [Inject] private IPlayerManager _playerManager;

    [SerializeField] private TMP_Text _textPlayerOne;
    [SerializeField] private TMP_Text _textPlayerTwo;
    [SerializeField] private Image _line;

    private Coroutine _currentCoroutine;

    private Animation _animationPlayerOne;
    private Animation _animationPlayerTwo;
    private Animation _animationLine;

    private AudioSource _audioSource;

    private int countColorOne;
    private int countColorTwo;

    private void Awake()
    {
        Construct();
    }

    private void Start()
    {
        _playerManager.OnActivePlayerChanged += ShowStartGame;
        _localizationManager.ChangeLanguageEvent += ChangeLanguageUpdateText;
        _colorContainer.ColorChangedEvent += ChangedUpdateColor;

        UpdateColorCount();
        UpdateTextWinInfo(_textPlayerOne);
        UpdateTextWinInfo(_textPlayerTwo);
    }

    private void Construct()
    {
        _animationPlayerOne = _textPlayerOne.GetComponent<Animation>();
        _animationPlayerTwo = _textPlayerTwo.GetComponent<Animation>();
        _animationLine = _line.GetComponent<Animation>();

        _audioSource = gameObject.GetComponent<AudioSource>();

        ResetEnable();
    }

    private void UpdateColorCount()
    {
        countColorOne = _colorContainer.CountColorPlayerOne;
        countColorTwo = _colorContainer.CountColorPlayerTwo;
    }

    private void UpdateTextWinInfo(TMP_Text player)
    {
        var turn = _localizationManager.GetText(EnumTextLocalization.game_turn);

        var countColor = 0;

        if (player == _textPlayerOne)
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
                player.text = $"{_localizationManager.GetText(EnumTextLocalization.ui_color_red)} {turn}";
                break;
            case 1:
                player.text = $"{_localizationManager.GetText(EnumTextLocalization.ui_color_blue)} {turn}";
                break;
            case 2:
                player.text = $"{_localizationManager.GetText(EnumTextLocalization.ui_color_white)} {turn}";
                break;
            case 3:
                player.text = $"{_localizationManager.GetText(EnumTextLocalization.ui_color_black)} {turn}";
                break;
            case 4:
                player.text = $"{_localizationManager.GetText(EnumTextLocalization.ui_color_orange)} {turn}";
                break;
            case 5:
                player.text = $"{_localizationManager.GetText(EnumTextLocalization.ui_color_green)} {turn}";
                break;
            case 6:
                player.text = $"{_localizationManager.GetText(EnumTextLocalization.ui_color_cyan)} {turn}";
                break;
            case 7:
                player.text = $"{_localizationManager.GetText(EnumTextLocalization.ui_color_violet)} {turn}";
                break;
            default:
                break;
        }
    }

    private void ChangedUpdateColor(Color obj)
    {
        UpdateColorCount();
        UpdateTextWinInfo(_textPlayerOne);
        UpdateTextWinInfo(_textPlayerTwo);
    }

    private void ChangeLanguageUpdateText(int obj)
    {
        UpdateColorCount();
        UpdateTextWinInfo(_textPlayerOne);
        UpdateTextWinInfo(_textPlayerTwo);
    }

    public void ShowStartGame(EnumPlayers players)
    {
        if (_playerManager.GetPlayersWhithUnits().Count < 2) return;

        ResetEnable();

        switch (players)
        {
            case EnumPlayers.PlayerOne:
                _textPlayerOne.enabled = true;
                _animationPlayerOne.Play();
                break;
            case EnumPlayers.PlayerTwo:
                _textPlayerTwo.enabled = true;
                _animationPlayerTwo.Play();
                break;
            default:
                break;
        }
        _line.enabled = true;
        _animationLine.Play();

        _audioSource.Play();
    }

    private void ResetEnable()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        _animationLine.Stop();
        _animationPlayerOne.Stop();
        _animationPlayerTwo.Stop();

        _audioSource.Stop();

        _textPlayerOne.enabled = false;
        _textPlayerTwo.enabled = false;
        _line.enabled = false;
    }

    private void OnDestroy()
    {
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);

        _playerManager.OnActivePlayerChanged -= ShowStartGame;
        _localizationManager.ChangeLanguageEvent -= ChangeLanguageUpdateText;
        _colorContainer.ColorChangedEvent -= ChangedUpdateColor;
    }
}
