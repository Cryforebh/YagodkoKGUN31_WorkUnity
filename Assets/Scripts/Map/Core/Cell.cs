using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private ContainerStatusGame _statusGame;

    [SerializeField] private MeshRenderer _focus;
    [SerializeField] private MeshRenderer _select;

    public Unit Unit { get; set; }
    public event Action<Cell> OnPointerClickEvent;

    private bool _isSelected = false;

    private void Start() => ResetAll();

    public void SetSelect(Material mat)
    {
        _select.sharedMaterial = mat;
        _select.enabled = true;
        _isSelected = true;
    }

    public void ResetAll()
    {
        _focus.enabled = false;
        _select.enabled = false;
        _select.sharedMaterial = null;
        _isSelected = false;    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_statusGame.StatusInt == 1 && !_isSelected)
        {
            _focus.enabled = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_statusGame.StatusInt == 1)
        {
            OnPointerClickEvent?.Invoke(this);
            _statusGame.StatusUpdateInt(3);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
        => _focus.enabled = false;
}

