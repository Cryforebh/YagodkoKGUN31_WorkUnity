using System.Collections.Generic;
using UnityEngine;

public class VoiceUnitManager : MonoBehaviour
{
    [Header("Настройки звука Невозможного Дествия:")]
    [SerializeField] private AudioSource _sourceDontAction;
    [SerializeField] private AudioClip _voiceDontActive;
    [Header("Настройки звуков:")]
    [SerializeField] private List<AudioClip> _bSoundsSelect = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsGoCell = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsDead = new List<AudioClip>();

    private List<AudioClip> soundClipsSelectUnit;
    private List<AudioClip> soundClipsDeadUnit;

    private int _indexSelect;
    private int _indexCell;
    private int _indexDead;

    private int _currentIndex;
    private int _currentIndexDead;

    /// <summary>
    /// Проигрывает звук на Юните в зависимости от статуса игры (НЕ для WS!).
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="statusGame"></param>
    public void VoicePlay(IUnitVoice unit, EnumGameEvent statusGame)
    {
        switch (statusGame)
        {
            case EnumGameEvent.Empty:
                soundClipsSelectUnit = _bSoundsSelect;
                _currentIndex = _indexSelect = UpdateIndex(_indexSelect);
                break;
            case EnumGameEvent.SelectedUnit:
                soundClipsSelectUnit = _bSoundsSelect;
                _currentIndex = _indexSelect = UpdateIndex(_indexSelect);
                break;
            case EnumGameEvent.SelectedCell:
                soundClipsSelectUnit = _bSoundsGoCell;
                _currentIndex = _indexCell = UpdateIndex(_indexCell);
                break;
            default:
                Debug.LogWarning("Внимание: выбран статус игры, которому не пренадлежат звуки!");
                return;
        }

        if (soundClipsSelectUnit == null || soundClipsSelectUnit.Count <= 0) return;

        unit.AudioSoundSource.Stop();
        unit.AudioSoundSource.PlayOneShot(soundClipsSelectUnit[_currentIndex]);
    }

    public void VoicePlayDontActive()
    {
        _sourceDontAction.Stop();
        _sourceDontAction.PlayOneShot(_voiceDontActive);
    }

    /// <summary>
    /// Проигрывает звук cмерти на Юните (НЕ для WS!).
    /// </summary>
    /// <param name="unit"></param>
    public void VoicePlayDead(IUnitVoice unit)
    {
        soundClipsDeadUnit = _bSoundsDead;
        _currentIndexDead = _indexDead = UpdateIndexDead(_indexDead);

        unit.AudioSoundSource.Stop();
        unit.AudioSoundSource.PlayOneShot(soundClipsDeadUnit[_currentIndexDead]);
    }

    private int UpdateIndex(int index)
    {
        if (soundClipsSelectUnit.Count <= index + 1)
        {
            return 0;
        }
        else
        {
            return index + 1;
        }
    }

    private int UpdateIndexDead(int indexDead)
    {
        if (soundClipsDeadUnit.Count <= indexDead + 1) return 0;
        else return indexDead + 1;
    }
}
