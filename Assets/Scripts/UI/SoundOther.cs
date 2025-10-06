using UnityEngine;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class SoundOther : MonoBehaviour
{
    [Inject] private SoundManager _soundManager;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = _soundManager.ValumeOther;
    }
}
