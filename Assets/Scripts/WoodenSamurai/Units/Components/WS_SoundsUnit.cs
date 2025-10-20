using System.Collections.Generic;
using UnityEngine;

public class WS_SoundsUnit : MonoBehaviour
{
    //[Inject] private WS_ContainerStatusGame _containerStatusGame;
    //[Inject] private WS_MoveSystem _moveSystem;

    [Header("Настройки звуков (Не для WS!)")]
    [SerializeField] private List<AudioClip> _bSoundsSelect = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsGoCell = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsAttack = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _bSoundsDead = new List<AudioClip>();

    private List<AudioClip> soundClipsSelectUnit;

    private int _indexSelect;
    private int _indexCell;
    private int _indexAttack;
    private int _indexDead;

    private int _currentIndex;

    private void Awake()
    {
        //_containerStatusGame = FindObjectOfType<WS_ContainerStatusGame>();
        //_moveSystem = FindObjectOfType<WS_MoveSystem>();
    }

    public void SoundPlayOnUnitAndStatusGame(WS_Unit unit, WS_EnumStatusGame statusGame)
    {
        switch (statusGame)
        {
            case WS_EnumStatusGame.Empty:
                soundClipsSelectUnit = unit.SoundSelect;
                _currentIndex = _indexSelect = UpdateIndex(_indexSelect);
                break;
            case WS_EnumStatusGame.SelectedUnit:
                soundClipsSelectUnit = unit.SoundSelect;
                _currentIndex = _indexSelect = UpdateIndex(_indexSelect);
                break;
            case WS_EnumStatusGame.SelectedCell:
                soundClipsSelectUnit = unit.SoundGoCell;
                _currentIndex = _indexCell = UpdateIndex(_indexCell);
                break;
            case WS_EnumStatusGame.Hit:
                soundClipsSelectUnit = unit.SoundAttack;
                _currentIndex = _indexAttack = UpdateIndex(_indexAttack);
                break;
            default:
                Debug.LogWarning("Внимание: выбран статус игры, которому не пренадлежат звуки!");
                return;
        }

        if (soundClipsSelectUnit == null || soundClipsSelectUnit.Count <= 0) return;

        unit.AudioSoundSource.PlayOneShot(soundClipsSelectUnit[_currentIndex]);
    }

    public void SoundPlayDead(WS_Unit unit)
    {
        int randomIndex = UnityEngine.Random.Range(0, unit.SoundDead.Count - 1);
        unit.AudioSoundSource.PlayOneShot(unit.SoundDead[randomIndex]);
    }

    public void SoundPlayerAttack(WS_Unit selected, WS_Unit target)
    {
        int randomIndex = UnityEngine.Random.Range(0, selected.SoundAttack.Count - 1);
        target.AudioSoundSource.PlayOneShot(selected.SoundAttack[randomIndex]);
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
}