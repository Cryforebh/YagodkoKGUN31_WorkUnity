using System;
using UnityEngine;
using Zenject;

public class C_MaterialsContainer : MonoBehaviour
{
    private ColorPlayersManager _color;

    [Header("Материалы для выбора Клетки:")]
    [SerializeField] private Material _materialShow;
    [SerializeField] private Material _materialEnter;
    [SerializeField] private Material _materialEnemyShow;
    [SerializeField] private Material _materialEnemyEnter;

    [Header("Материалы для выбора Юнита:")]
    [SerializeField] private Material _materialUnitShow;
    [SerializeField] private Material _materialUnitEnter;
    [SerializeField] private Material _materialUnitilluminatedAvailable;

    [Header("Материалы игроков:")]
    [SerializeField] private Material _materialPlayerOne;
    [SerializeField] private Material _materialPlayerTwo;

    public Material MaterialShow => _materialShow;
    public Material MaterialEnter => _materialEnter;
    public Material MaterialShowEnemy => _materialEnemyShow;
    public Material MaterialEnterEnemy => _materialEnemyEnter;

    public Material MaterialUnitShow => _materialUnitShow;
    public Material MaterialUnitEnter => _materialUnitEnter;
    public Material MaterialUnitilluminatedAvailable => _materialUnitilluminatedAvailable;

    public Material PlayerOne => _materialPlayerOne;
    public Material PlayerTwo => _materialPlayerTwo;

    public event Action<Material> MaterialChangedEvent;

    [Inject]
    public void Construct(ColorPlayersManager color)
    {
        _color = color;

        //_materialPlayerOne = _color.MaterialPlayerOne;
        //_materialPlayerTwo = _color.MaterialPlayerTwo;

        MaterialUpdate();

        _color.Subscribe(ColorChangedMaterialUpdate);
    }

    private void ColorChangedMaterialUpdate(Color obj)
    {
        MaterialUpdate();
    }

    private void MaterialUpdate()
    {
        _materialPlayerOne.color = _color.PlayerOne;
        _materialPlayerTwo.color = _color.PlayerTwo;

        //_materialPlayerOne = _color.MaterialPlayerOne;
        //_materialPlayerTwo = _color.MaterialPlayerTwo;

        MaterialChangedEvent?.Invoke(_materialPlayerOne);
    }

    public void OnDestroy()
    {
        _color.Unsubscribe(ColorChangedMaterialUpdate);
    }
}
