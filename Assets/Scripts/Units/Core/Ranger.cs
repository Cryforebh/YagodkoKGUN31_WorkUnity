using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Unit
{
    [Header("Настройки характеристик:")]
    [SerializeField][Range(1f, 100f)] private float _bHealth = 7f;
    [SerializeField][Range(0f, 10f)] private float _bStrength = 1f;
    [Header("Настройки атаки:")]
    [SerializeField][Range(1f, 5f)] private float _bDamageWeapon = 3f;
    [SerializeField][Range(1f, 7f)] private int _bAttackRange = 8;
    [SerializeField] private EnumAttackPattern _bAttackPattern = EnumAttackPattern.Square;
    [Header("Настройки передвижения")]
    [SerializeField][Range(1f, 8f)] private int _bMoveRange;
    [Header("Настройки Смерти")]
    [SerializeField] private float _bDeathDelay = 2f;
    [Header("Настройки звуков")]
    [SerializeField] private List<AudioClip> _bSoundsSelect = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsGoCell = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsAttack = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsDead = new List<AudioClip>();


    private void Start()
    {
        name = "Лучник";
        Class = (EnumStatusUnitClass.Ranger);

        SetStandardCharacter(_bHealth, _bStrength);
        SetWeaponDamage(_bDamageWeapon);
        AttackRange = _bAttackRange;
        AttackPattern = _bAttackPattern;
        MoveRange = _bMoveRange;
        DeadDelay = _bDeathDelay;
        SoundSelect = _bSoundsSelect;
        SoundGoCell = _bSoundsGoCell;
        SoundAttack = _bSoundsAttack;
        SoundDead = _bSoundsDead;
    }
}
