using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class EnterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Inject] AdvancedCursorController _cursor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _cursor.SetCursorState(EnumStatusCursor.Select);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _cursor.SetCursorState(EnumStatusCursor.Default);
    }
}
