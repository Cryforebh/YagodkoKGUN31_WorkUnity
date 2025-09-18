using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeGameManager : MonoBehaviour
{
    private EnumTypeGame _gameType = EnumTypeGame.PVP;

    public EnumTypeGame GameType => _gameType;

    public void SetTypeGame(EnumTypeGame gameType)
    {
        _gameType = gameType;
    }
}
