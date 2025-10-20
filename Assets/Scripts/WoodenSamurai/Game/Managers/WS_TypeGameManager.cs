using UnityEngine;

public class WS_TypeGameManager : MonoBehaviour
{
    private WS_EnumTypeGame _gameType = WS_EnumTypeGame.PVP;

    public WS_EnumTypeGame GameType => _gameType;

    public void SetTypeGame(WS_EnumTypeGame gameType)
    {
        _gameType = gameType;
    }
}
