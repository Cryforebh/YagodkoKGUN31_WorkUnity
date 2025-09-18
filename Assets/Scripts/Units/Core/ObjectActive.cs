using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(Collider))]
public class ObjectActive : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] AdvancedCursorController _cursor;

    [SerializeField] private AudioClip[] _audioClip;
    [SerializeField] private EnumStatusCursor _cursorSelect;

    private AudioSource _audioSource;
    private int _currentIndex;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _currentIndex = _audioClip.Length;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_audioClip.Length == 0) return;

        _currentIndex = (_currentIndex + 1) % _audioClip.Length;
        _audioSource.PlayOneShot(_audioClip[_currentIndex]);
    }   

    public void OnPointerEnter(PointerEventData eventData)
    {
        _cursor.SetCursorState(_cursorSelect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _cursor.SetCursorState(EnumStatusCursor.Default);
    }
}
