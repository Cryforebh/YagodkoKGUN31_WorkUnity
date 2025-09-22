using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Zenject;

public class WhoGoesFirst : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Inject] AdvancedCursorController _cursor;
    [Inject] private SpawnUnitOnCell _spawn;

    [SerializeField] private GameObject _background;

    private void Awake()
    {
        
    }

    public void SelectedPlayerOne()
    {
        _spawn.SetWhoGoesFirst(EnumPlayers.PlayerOne);
        OnDisable();
    }

    public void SelectedPlayerTwo()
    {
        _spawn.SetWhoGoesFirst(EnumPlayers.PlayerTwo);
        OnDisable();
    }

    private void OnDisable()
    {
        Destroy(_background);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _cursor.SetCursorState(EnumStatusCursor.Select);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _cursor.SetCursorState(EnumStatusCursor.Default);
    }
}
