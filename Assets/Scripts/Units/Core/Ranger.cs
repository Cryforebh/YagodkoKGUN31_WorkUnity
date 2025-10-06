using System.Collections.Generic;
using UnityEngine;

public class Ranger : Unit
{
    [Header("Настройки звуков")]
    [SerializeField] private List<AudioClip> _bSoundsSelect = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsGoCell = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsAttack = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsDead = new List<AudioClip>();


    private void Start()
    {
        name = "Лучник";
        Class = (EnumStatusUnitClass.Ranger);

        SoundSelect = _bSoundsSelect;
        SoundGoCell = _bSoundsGoCell;
        SoundAttack = _bSoundsAttack;
        SoundDead = _bSoundsDead;
    }
}
