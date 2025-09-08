using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class Unit : MonoBehaviour
{
    private Vector3 _position;
    [SerializeField][Range(1f, 100f)] private float _baseHealth = 10f;
    [ReadOnly] private float _health;
    private float _baseDamage;
    [SerializeField][Range(0f, 10f)] private float _baseStrength = 3f;
    [ReadOnly] private EnumStatusUnit _statusUnit;
    [ReadOnly] private EnumPlayers _player;

    public float GetDamage => _baseDamage + _baseStrength;
    public float GetStrength => _baseStrength;
    public float GetHealth => _baseHealth + _baseStrength;
    public bool IsDead { get; private set; }
    public EnumStatusUnit GetStatusUnit => _statusUnit;
    public EnumPlayers Player => _player;

    public void SetStatusUnit(EnumStatusUnit statusUnit)
    {
        _statusUnit = statusUnit;
    }

    public void SetPlayer(EnumPlayers player) =>  _player = player;

    public void SetHealth(float health) 
    {
        _health = health;
        if (_health <= 0) Dead();
    }

    private void Dead()
    {
        enabled = false;
        gameObject.SetActive(false);
        this.IsDestroyed();
        IsDead = true;
    }

    

    private void Start()
    {
        _position = transform.position;
    }



    public Vector3 GetPosition()
    {
        return _position;
    }

    public Unit GetUnit()
    {
        return this;
    } 
}
