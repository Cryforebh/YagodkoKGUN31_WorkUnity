using UnityEngine;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class SoundType : MonoBehaviour
{
    [SerializeField] private EnumSounds _soundsType;

    private AudioSource _audioSource;
    private float _oldVolume;
    private SoundManager _soundManager;


    private void Start()
    {
        _soundManager = FindObjectOfType<SoundManager>();

        _audioSource = GetComponent<AudioSource>();
        _oldVolume = _audioSource.volume; // 100%
        SetValume(_soundsType);
    }

    private void Update()
    {
        SetValume(_soundsType);
    }

    [Inject]
    private void SetSoundManager(SoundManager soundManager)
    {
        _soundManager = soundManager;
    }

    public void SetValume(EnumSounds enumSounds)
    {
        switch (enumSounds)
        {
            case EnumSounds.None:
                break;
            case EnumSounds.Units:
                _audioSource.volume = _oldVolume * _soundManager.ValumeUnits;
                break;
            case EnumSounds.Modifications:
                _audioSource.volume = _oldVolume * _soundManager.ValumeModifications;
                break;
            case EnumSounds.Ambient:
                _audioSource.volume = _oldVolume * _soundManager.ValumeAmbient;
                break;
            case EnumSounds.Other:
                _audioSource.volume = _oldVolume * _soundManager.ValumeOther;
                break;
            default:
                break;
        }

        //Если исходное значение 0 → 0 × 0.4 = 0
        //Если исходное значение 0.5 → 0.5 × 0.4 = 0.2
        //Если исходное значение 1 → 1 × 0.4 = 0.4
    }
}
