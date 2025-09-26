using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SoundSliderValume : MonoBehaviour
{
    [SerializeField] private EnumSounds _soundsType;

    private SoundManager _soundManager;
    private Slider _slider;

    private void Start()
    {
        _slider = GetComponent<Slider>();
        SetValume(_soundsType);
    }

    public void SetValume(EnumSounds enumSounds)
    {
        switch (enumSounds)
        {
            case EnumSounds.None:
                break;
            case EnumSounds.Units:
                _slider.value = _soundManager.ValumeUnits;
                break;
            case EnumSounds.Modifications:
                _slider.value = _soundManager.ValumeModifications;
                break;
            case EnumSounds.Ambient:
                _slider.value = _soundManager.ValumeAmbient;
                break;
            case EnumSounds.Other:
                _slider.value = _soundManager.ValumeOther;
                break;
            default:
                break;
        }
    }

        [Inject]
    private void SetSoundManager(SoundManager soundManager)
    {
        _soundManager = soundManager;
    }
}
