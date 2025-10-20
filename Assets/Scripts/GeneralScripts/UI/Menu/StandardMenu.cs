using System;
using UnityEngine;

public abstract class StandardMenu : MonoBehaviour
{
    protected bool _onMenu = false;
    public bool OnMenu => _onMenu;

    public abstract event Action<bool> OnMenuEvent;
}
