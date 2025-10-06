using UnityEngine;

public class WinImage : MonoBehaviour
{
    [SerializeField] private RedWiner _redWiner;
    [SerializeField] private BlueWiner _blueWiner;
    [SerializeField] private Animation _animWinInfo;
    [SerializeField] private Animation _animRestartInfo;
    [SerializeField] private Animation _animMenuInfo;

    private void Awake()
    {
        gameObject.SetActive(false);
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
