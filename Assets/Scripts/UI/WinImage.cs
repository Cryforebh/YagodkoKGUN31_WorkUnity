using UnityEngine;

public class WinImage : MonoBehaviour
{
    [SerializeField] private RedWiner _redWiner;
    [SerializeField] private BlueWiner _blueWiner;

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
    }

}
