using System.Collections.Generic;
using UnityEngine;

public class WS_Samurai : WS_Unit
{
    [Header("Настройки звуков")]
    [SerializeField] private List<AudioClip> _bSoundsSelect = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsGoCell = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsAttack = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsDead = new List<AudioClip>();

    private void Start()
    {
        name = "Самурай";
        Class = (WS_EnumStatusUnitClass.Samurai);

        SoundSelect = _bSoundsSelect;
        SoundGoCell = _bSoundsGoCell;
        SoundAttack = _bSoundsAttack;
        SoundDead = _bSoundsDead;
    }

}
