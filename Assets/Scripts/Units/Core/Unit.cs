using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class Unit : MonoBehaviour
{
    [Inject] private AllPlayer _allPlayer;

    private Vector3 _position;
    private float _baseHealth = 15f;
    private float _health;
    private float _baseDamage = 2f;
    private float _weaponDamage = 0;
    private float _baseStrength = 3f;
    private EnumStatusUnit _statusUnit;
    private EnumPlayers _player;

    public float GetDamage => _baseDamage + _baseStrength + _weaponDamage;
    public float GetStrength => _baseStrength;
    public float GetHealth => _health;
    public float GetMaxHealth {  get; private set; }
    public float GetPastDamage { get; private set; }
    public bool IsDead { get; private set; }
    public EnumStatusUnit GetStatusUnit => _statusUnit;
    public EnumPlayers Player => _player;

    public event Action<Unit> DeadUnitEvent;
    public event Action<Unit> HitDamageUnitEvent;

    public void SetStandardCharacter(float baseHealth, float baseStrength)
    {
        _health = baseHealth + baseStrength;
        _baseHealth = baseHealth;
        _baseStrength = baseStrength;
        GetMaxHealth = _health;
    }

    public void SetWeaponDamage(float weaponDamage)
    {
        _weaponDamage = weaponDamage;
    }

    public void SetStatusUnit(EnumStatusUnit statusUnit) => _statusUnit = statusUnit;

    public void SetPlayer(EnumPlayers player) =>  _player = player;

    public void HitDamageHealth(float Damage) 
    {
        var min = Damage / 2;
        var max = Damage;
        Damage = GetPastDamage = UnityEngine.Random.Range(min, max);

        _health = _health - Damage;

        HitDamageUnitEvent?.Invoke(this);

        if (_health <= 0) Dead(this);
    }

    private void Dead(Unit unitDead)
    {
        DeadUnitEvent?.Invoke(this);
        IsDead = true;
        _allPlayer.RemoveUnitOnPlayer(Player, this);
        enabled = false;
        gameObject.SetActive(false);
        this.IsDestroyed();
    }

    private void Awake()
    {
        _allPlayer = FindObjectOfType<AllPlayer>();
    }

    private void Start()
    {
        name = "Юнит";
        _position = transform.position;
        _health = _baseHealth + _baseStrength;
        GetMaxHealth = _health;
    }

    public Vector3 GetPosition()
    {
        return _position;
    }

    public Unit GetUnit()
    {
        return this;
    } 

    private void CurrentUnit(Unit unit)
    {
        unit = this; 
    }

    private void OnDestroy()
    {
 
    }
}
