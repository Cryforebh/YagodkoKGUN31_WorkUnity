using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class WS_Unit : MonoBehaviour, IUnitMain, IUnitVoice
{
    [Inject] private AllPlayer _allPlayer;
    [Inject] private SoundManager _soundManager; // Используется!
    [Inject] private WS_SettingObjectManager _settingObjectManager;

    [Header("Настройки характеристик:")]
    [SerializeField][Range(1f, 50f)] private float _baseHealth = 14f;
    [SerializeField][Range(0f, 10f)] private float _baseStrength = 3f;
    [SerializeField][Range(0f, 10f)] private float _lvlUpHealthCount;
    [SerializeField][Range(0f, 5f)] private float _lvlUpStrengthCount;
    [Header("Настройки атаки:")]
    [SerializeField][Range(1f, 5f)] private float _weaponDamage = 1;
    [SerializeField][Range(1f, 7f)] private int _attackRange = 1;
    [SerializeField] private WS_EnumAttackPattern _attackPattern = WS_EnumAttackPattern.Square;
    [Header("Настройки передвижения")]
    [SerializeField][Range(1f, 8f)] private int _moveRange = 2;
    [SerializeField][Range(0f, 2f)] private int _lvlUpMoveRangeCount;
    [Header("Настройки Смерти")]
    [SerializeField] private float _deathDelay = 0.5f;

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
    private WS_EnumStatusUnitEnemy _statusUnit;
    private WS_EnumStatusUnitClass _class;

    public WS_Cell CurrentCell { get; set; }
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
    public List<AudioClip> SoundSelect { get => _soundsSelect; protected set => _soundsSelect = value; }
    public List<AudioClip> SoundGoCell { get => _soundsGoCell; protected set => _soundsGoCell = value; }
    public List<AudioClip> SoundAttack { get => _soundsAttack; protected set => _soundsAttack = value; }
    public List<AudioClip> SoundDead { get => _soundsDead; protected set => _soundsDead = value; }
    public AudioSource AudioSoundSource { get => _audioSource; protected set => _audioSource = value; }
    public WS_EnumStatusUnitEnemy StatusUnit { get => _statusUnit; protected set => _statusUnit = value; }
    public bool IsEnemy
    {
        get
        {
            switch (StatusUnit)
            {
                case WS_EnumStatusUnitEnemy.My:
                    return false;
                default:
                    return true;
            }
        }
        set
        {
            switch (value)
            {
                case false:
                    StatusUnit = WS_EnumStatusUnitEnemy.My;
                    break;
                case true:
                    StatusUnit = WS_EnumStatusUnitEnemy.Enemy;
                    break;
            }
        }
    }
    public EnumPlayers Player { get ; set; }
    public WS_EnumStatusUnitClass Class { get => _class; protected set => _class = value; }
    public WS_EnumAttackPattern AttackPattern { get => _attackPattern; protected set => _attackPattern = value; }

    public Transform CurrentTransform => transform;

    public event Action<WS_Unit> DeadUnitEvent;
    public event Action<WS_Unit> HitDamageUnitEvent;
    public event Action<WS_Unit> LevelUpEvent;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _allPlayer = FindObjectOfType<AllPlayer>();
        _settingObjectManager = FindObjectOfType<WS_SettingObjectManager>();

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
    public void SetStatusUnit(WS_EnumStatusUnitEnemy statusUnit) => _statusUnit = statusUnit;
    public void SetStatusClass(WS_EnumStatusUnitClass statusUnitClass) => _class = statusUnitClass;
    public void SetAttackPattern(WS_EnumAttackPattern enumAttackPattern) => _attackPattern = enumAttackPattern;
    public void SetModifier(WS_EnumModifier modifier, bool offAndOn)
    {
        switch (modifier)
        {
            case WS_EnumModifier.None:
                break;
            case WS_EnumModifier.Drink:
                _modifierDrink = offAndOn;
                break;
            case WS_EnumModifier.UpHelth:
                _currentHealth += _settingObjectManager.MofifiSushiHealthUp;
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

            if (Class == WS_EnumStatusUnitClass.Samurai)
            {
                _baseStrength += _lvlUpStrengthCount * countUp;
                _baseHealth += _lvlUpHealthCount * countUp;
                MoveRange += _lvlUpMoveRangeCount * countUp;
            }
            if (Class == WS_EnumStatusUnitClass.Ranger)
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
            var min = Damage / 1.4f;
            var max = Damage;
            Damage = GetPastDamage = UnityEngine.Random.Range(min, max);
        }

        if (_modifierDefense) Damage = Damage / 2;

        _currentHealth = _currentHealth - Damage;

        HitDamageUnitEvent?.Invoke(this);

        if (_currentHealth <= 0) Death();
    }

    public void Death()
    {
        if (IsDead) return; // Защита от повторного вызова

        DeadUnitEvent?.Invoke(this);
        IsDead = true;
        CurrentCell.ClearUnit();

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

        Debug.Log($"{name}: Умер окончательно!");
        gameObject.SetActive(false);
    }

    public Vector3 GetPosition()
    {
        return _position;
    }

    public WS_Unit GetUnit()
    {
        return this;
    }

    public void OnDelete()
    {
        Destroy(gameObject);
    }
}
