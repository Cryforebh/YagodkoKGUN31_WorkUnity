using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum NeighbourType
{
    None = 0,
    Left = 1,
    Right = 2,
    Up = 4,
    Down = 8
}

public enum Team
{
    Player1,
    Player2
}
