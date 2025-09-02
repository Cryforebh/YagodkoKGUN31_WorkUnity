using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler 
{
    [SerializeField] private MeshRenderer _focus;
    [SerializeField] private MeshRenderer _select;

    public Unit Unit { get; set; }
    public event Action<Cell> OnPointerClickEvent;

    private void Start() => ResetAll();

    public void SetSelect(Material mat)
    {
        _select.sharedMaterial = mat;
        _select.enabled = true;
    }

    public void ResetAll()
    {
        _focus.enabled = false;
        _select.enabled = false;
        _select.sharedMaterial = null;
    }

    public void OnPointerEnter(PointerEventData eventData) 
        => _focus.enabled = true;

    public void OnPointerClick(PointerEventData eventData) 
        => OnPointerClickEvent?.Invoke(this);

    public void OnPointerExit(PointerEventData eventData) 
        => _focus.enabled = false;
}

