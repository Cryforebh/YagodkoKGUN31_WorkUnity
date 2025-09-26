using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public abstract class Unit : MonoBehaviour
{
    [Inject] private AllPlayer _allPlayer;
    [Inject] private SoundManager _soundManager;

    private Vector3 _position;
    private float _baseHealth = 15f;
    private float _health;
    private float _baseDamage = 2f;
    private float _weaponDamage = 0;
    private float _baseStrength = 3f;
    private int _attackRange;
    private int _moveRange;
    private int _level = 1;
    private int _countLevelUpMax = 4;
    private float _deathDelay = 2f;
    private bool _modifierDefense = false;
    private bool _modifierDrink = false;
    private int _modifierDamage = 0;
    private bool _modifierDrowRange = false;
    private Collider _collider;
    private Animator _animator;
    private List<AudioClip> _soundsSelect;
    private List<AudioClip> _soundsGoCell;
    private List<AudioClip> _soundsAttack;
    private List<AudioClip> _soundsDead;
    private AudioSource _audioSource;
    private EnumStatusUnitEnemy _statusUnit;
    private EnumPlayers _player;
    private EnumStatusUnitClass _class;
    private EnumAttackPattern _attackPattern;

    public float GetDamage => _modifierDamage + _baseDamage + _baseStrength + _weaponDamage;
    public float GetStrength => _baseStrength;
    public float Health { get => _health; protected set => _health = value; }
    public int AttackRange { get => _attackRange; set => _attackRange = value; }
    public int MoveRange { get => _moveRange; set => _moveRange = value; }
    public float GetMaxHealth { get; private set; }
    public float GetPastDamage { get; private set; }
    public bool IsDead { get; private set; }
    public int Level => _level;
    public float DeadDelay { get => _deathDelay; protected set => _deathDelay = value; }
    public bool ModifierDefense { get => _modifierDefense; set => _modifierDefense = value; }
    public int ModifierDamage { get => _modifierDamage; set => _modifierDamage = value; }
    public bool ModifierDrowRange { get => _modifierDrowRange; set => _modifierDrowRange = value; }
    public List<AudioClip> SoundSelect { get => _soundsSelect; protected set => _soundsSelect = value; }
    public List<AudioClip> SoundGoCell { get => _soundsGoCell; protected set => _soundsGoCell = value; }
    public List<AudioClip> SoundAttack { get => _soundsAttack; protected set => _soundsAttack = value; }
    public List<AudioClip> SoundDead { get => _soundsDead; protected set => _soundsDead = value; }
    public AudioSource AudioSoundSource { get => _audioSource; protected set => _audioSource = value; }
    public EnumStatusUnitEnemy StatusUnit { get => _statusUnit; protected set => _statusUnit = value; }
    public EnumPlayers Player { get => _player; protected set => _player = value; }
    public EnumStatusUnitClass Class { get => _class; protected set => _class = value; }
    public EnumAttackPattern AttackPattern { get => _attackPattern; protected set => _attackPattern = value; }

    public event Action<Unit> DeadUnitEvent;
    public event Action<Unit> HitDamageUnitEvent;
    public event Action<Unit> LevelUpEvent;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _audioSource = GetComponent<AudioSource>();
        _allPlayer = FindObjectOfType<AllPlayer>();
    }

    private void Start()
    {
        if (_audioSource == null)
        {
            Debug.LogError($"Отсутствует компонент AudioSource на {this.name}!");
        }

        name = "Юнит";
        _position = transform.position;
        _health = _baseHealth + _baseStrength;
        GetMaxHealth = _health;
    }

    public void SetStandardCharacter(float baseHealth, float baseStrength)
    {
        _health = baseHealth + baseStrength;
        _baseHealth = baseHealth;
        _baseStrength = baseStrength;
        GetMaxHealth = _health;
    }

    public void SetWeaponDamage(float weaponDamage) => _weaponDamage = weaponDamage;
    public void SetAttackRange(int range) => _attackRange = range;
    public void SetStatusUnit(EnumStatusUnitEnemy statusUnit) => _statusUnit = statusUnit;
    public void SetStatusClass(EnumStatusUnitClass statusUnitClass) => _class = statusUnitClass;
    public void SetAttackPattern(EnumAttackPattern enumAttackPattern) => _attackPattern = enumAttackPattern;
    public void SetPlayer(EnumPlayers player) => _player = player;
    public void SetModifier(EnumModifier modifier, bool offAndOn)
    {
        switch (modifier)
        {
            case EnumModifier.None:
                break;
            case EnumModifier.Drink:
                _modifierDrink = offAndOn;
                break;
            case EnumModifier.UpHelth:
                _health += 3f;
                if (GetMaxHealth < _health) _health = GetMaxHealth;
                break;
            default:
                break;
        }
    }

    public void LevelUp(int countUp)
    {
        if (countUp <= 0 || countUp > _countLevelUpMax) return;

        if (_countLevelUpMax > 0)
        {
            _countLevelUpMax -= 1 * countUp;

            _level += countUp;

            if (Class == EnumStatusUnitClass.Samurai)
            {
                _baseStrength += 0.5f * countUp;
                _baseHealth += 2 * countUp;
                MoveRange += 2 % _level;
            }
            if (Class == EnumStatusUnitClass.Ranger)
            {
                _baseStrength += 1 * countUp;
                _baseHealth += 3 * countUp;
                MoveRange += 1;
            }

            //_baseStrength += 1 * countUp;
            //_baseHealth += 3 * countUp;
            ////AttackRange += 1 * countUp;
            //MoveRange += 1 * countUp;

            var oldMaxHealth = GetMaxHealth;
            GetMaxHealth = _baseHealth + _baseStrength;

            _health = GetMaxHealth - (oldMaxHealth - _health);

            var massPlus = 0.02f * countUp;
            transform.localScale += new Vector3(massPlus, massPlus, massPlus);

            LevelUpEvent?.Invoke(this);
        }
    }

    public void HitDamageHealth(float Damage)
    {
        if (!_modifierDrink)
        {
            var min = Damage / 1.3f;
            var max = Damage;
            Damage = GetPastDamage = UnityEngine.Random.Range(min, max);
        }

        if (_modifierDefense) Damage = Damage / 2;

        _health = _health - Damage;

        HitDamageUnitEvent?.Invoke(this);

        if (_health <= 0) Dead(this);
    }

    private void Dead(Unit unitDead)
    {
        if (IsDead) return; // Защита от повторного вызова

        DeadUnitEvent?.Invoke(this);
        IsDead = true;

        StartCoroutine(DeathProcess());
    }

    private IEnumerator DeathProcess()
    {
        // 1. Запустить анимацию смерти
        //_animator.SetTrigger("Die");

        // 2. Отключить коллайдер и управление
        //_collider.enabled = false;
        _allPlayer.RemoveUnitOnPlayer(Player, this);
        // GetComponent<PlayerMovement>().enabled = false; // Пример отключения скрипта

        // 3. Ждать заданное время
        yield return new WaitForSeconds(_deathDelay);



        foreach (var objChild in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            objChild.enabled = false;
        }

        // 3. Ждать заданное время
        yield return new WaitForSeconds(0.5f);

        Debug.Log($"{this.name}: Умер окончательно!");
        //enabled = false;
        gameObject.SetActive(false);
        //this.IsDestroyed();

        // 4. Окончательные действия (например, исчезновение)
        //Destroy(gameObject); // Или gameObject.SetActive(false);
    }

    public Vector3 GetPosition()
    {
        return _position;
    }

    public Unit GetUnit()
    {
        return this;
    }

    public void OnDelete()
    {
        Destroy(gameObject);
    }
}
