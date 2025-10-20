using TMPro;
using UnityEngine;
using Zenject;

public class WS_WinImage : MonoBehaviour
{
    [Inject] LocalizationManager _localizationManager;

    [SerializeField] private WS_RedWiner _redWiner;
    [SerializeField] private WS_BlueWiner _blueWiner;
    [SerializeField] private Animation _animWinInfo;
    [SerializeField] private Animation _animRestartInfo;
    [SerializeField] private Animation _animMenuInfo;

    private TMP_Text _tMP_TextMenu;
    private TMP_Text _tMP_TextRestart;

    private void Awake()
    {
        _tMP_TextMenu = _animMenuInfo.GetComponentInChildren<TMP_Text>();
        _tMP_TextRestart = _animRestartInfo.GetComponentInChildren<TMP_Text>();

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SetText();
    }

    private void SetText()
    {
        _tMP_TextMenu.text = $"{_localizationManager.GetText(EnumTextLocalization.win_menu)} \"Esc\"";
        _tMP_TextRestart.text = $"{_localizationManager.GetText(EnumTextLocalization.win_restart)} \"R\"";
    }

    public void Show(EnumPlayers player)
    {
        gameObject.SetActive(true);

        switch (player)
        {
            case EnumPlayers.None:
                break;
            case EnumPlayers.PlayerOne:
                _blueWiner.gameObject.SetActive(true);
                break;
            case EnumPlayers.PlayerTwo:
                _redWiner.gameObject.SetActive(true);
                break;
            case EnumPlayers.PlayerThree:
                break;
            default:
                break;
        }

        _animWinInfo.Play();
        _animRestartInfo.Play();
        _animMenuInfo.Play();
    }

}
