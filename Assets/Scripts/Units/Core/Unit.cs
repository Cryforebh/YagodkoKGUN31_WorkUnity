using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class Unit : MonoBehaviour
{
    [Inject] private AllPlayer _allPlayer;
    [Inject] private SoundManager _soundManager;

    [Header("Настройки характеристик:")]
    [SerializeField][Range(1f, 50f)] private float _baseHealth = 14f;
    [SerializeField][Range(0f, 10f)] private float _baseStrength = 3f;
    [SerializeField][Range(0f, 10f)] private float _lvlUpHealthCount;
    [SerializeField][Range(0f, 5f)] private float _lvlUpStrengthCount;
    [Header("Настройки атаки:")]
    [SerializeField][Range(1f, 5f)] private float _weaponDamage = 1;
    [SerializeField][Range(1f, 7f)] private int _attackRange = 1;
    [SerializeField] private EnumAttackPattern _attackPattern = EnumAttackPattern.Square;
    [Header("Настройки передвижения")]
    [SerializeField][Range(1f, 8f)] private int _moveRange = 2;
    [SerializeField][Range(0f, 2f)] private int _lvlUpMoveRangeCount;
    [Header("Настройки Смерти")]
    [SerializeField] private float _deathDelay = 0.5f;
    [Header("Настройки Модификаций")]
    [SerializeField][Range(1f, 10f)] private float _modifierSushiHealth = 5;

    private Vector3 _position;
    private float _currentHealth;
    private float _baseDamage = 2f;
    private int _level = 1;
    private int _countLevelUpMax = 4;
    private bool _modifierDefense = false;
    private bool _modifierDrink = false;
    private int _modifierDamage = 0;
    private bool _modifierDrowRange = false;
    private List<AudioClip> _soundsSelect;
    private List<AudioClip> _soundsGoCell;
    private List<AudioClip> _soundsAttack;
    private List<AudioClip> _soundsDead;
    private AudioSource _audioSource;
    private EnumStatusUnitEnemy _statusUnit;
    private EnumPlayers _player;
    private EnumStatusUnitClass _class;

    public float GetDamage => _modifierDamage + _baseDamage + _baseStrength + _weaponDamage;
    public float GetStrength => _baseStrength;
    public float Health { get => _currentHealth; protected set => _currentHealth = value; }
    public int AttackRange { get => _attackRange; set => _attackRange = value; }
    public int MoveRange { get => _moveRange; set => _moveRange = value; }
    public float GetMaxHealth => _baseHealth + _baseStrength;
    public float GetPastDamage { get; private set; }
    public bool IsDead { get; private set; }
    public int Level => _level;
    public float LvlUpHealthCounts => _lvlUpHealthCount + _lvlUpStrengthCount;
    public float LvlUpDamageCounts => _lvlUpStrengthCount;
    public int LvlUpMoveRangeCount => _lvlUpMoveRangeCount;
    public float DeadDelay { get => _deathDelay; protected set => _deathDelay = value; }
    public bool ModifierDefense { get => _modifierDefense; set => _modifierDefense = value; }
    public int ModifierDamage { get => _modifierDamage; set => _modifierDamage = value; }
    public bool ModifierDrowRange { get => _modifierDrowRange; set => _modifierDrowRange = value; }
    public float ModifierSushiHealth => _modifierSushiHealth;
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
        _audioSource = GetComponent<AudioSource>();
        _allPlayer = FindObjectOfType<AllPlayer>();
        _currentHealth = GetMaxHealth;

        name = "Юнит";
        _position = transform.position;

        if (_audioSource == null)
        {
            Debug.LogError($"Отсутствует компонент AudioSource на {this.name}!");
        }
    }

    public void SetStandardCharacter(float baseHealth, float baseStrength)
    {
        _currentHealth = baseHealth + baseStrength;
        _baseHealth = baseHealth;
        _baseStrength = baseStrength;
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
                _currentHealth += _modifierSushiHealth;
                if (GetMaxHealth < _currentHealth) _currentHealth = GetMaxHealth;
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

            var oldMaxHealth = GetMaxHealth;

            if (Class == EnumStatusUnitClass.Samurai)
            {
                _baseStrength += _lvlUpStrengthCount * countUp;
                _baseHealth += _lvlUpHealthCount * countUp;
                MoveRange += _lvlUpMoveRangeCount * countUp;
            }
            if (Class == EnumStatusUnitClass.Ranger)
            {
                _baseStrength += _lvlUpStrengthCount * countUp;
                _baseHealth += _lvlUpHealthCount * countUp;
                MoveRange += _lvlUpMoveRangeCount * countUp;
            }

            _currentHealth += GetMaxHealth - oldMaxHealth;

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

        _currentHealth = _currentHealth - Damage;

        HitDamageUnitEvent?.Invoke(this);

        if (_currentHealth <= 0) Dead(this);
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
        _allPlayer.RemoveUnitOnPlayer(Player, this);

        yield return new WaitForSeconds(_deathDelay);

        foreach (var objChild in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            objChild.enabled = false;
        }

        yield return new WaitForSeconds(0.5f);

        Debug.Log($"{this.name}: Умер окончательно!");
        gameObject.SetActive(false);
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
