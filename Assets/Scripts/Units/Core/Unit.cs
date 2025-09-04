using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class Unit : MonoBehaviour
{
    [Inject] private ContainerStatusGame _statusGame;
    private Vector3 _position;
    //private Unit _unit;

    private void Awake()
    {
        //_unit = GetComponent<Unit>();
        _position = transform.position;
        _statusGame.OnStatusChanged += OnGameStatusChanged;
    }

    private void OnGameStatusChanged(EnumStatusGame newStatus)
    {
        if (newStatus >= EnumStatusGame.SelectedCell)
        {
            
        }
    }

    public void Move(Vector3 position)
    {
        _position = position;
    }
}
