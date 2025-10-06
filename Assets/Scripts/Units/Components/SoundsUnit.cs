using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SoundsUnit : MonoBehaviour
{
    [Inject] private ContainerStatusGame _containerStatusGame;
    [Inject] private MoveSystem _moveSystem;

    private List<AudioClip> soundClipsSelectUnit;

    private int _indexSelect;
    private int _indexCell;
    private int _indexAttack;
    private int _indexDead;

    private int _currentIndex;

    private void Awake()
    {
        _containerStatusGame = FindObjectOfType<ContainerStatusGame>();
        _moveSystem = FindObjectOfType<MoveSystem>();
    }

    public void SoundPlayOnUnitAndStatusGame(Unit unit, EnumStatusGame statusGame)
    {
        switch (statusGame)
        {
            case EnumStatusGame.Empty:
                soundClipsSelectUnit = unit.SoundSelect;
                _currentIndex = _indexSelect = UpdateIndex(_indexSelect);
                break;
            case EnumStatusGame.SelectedUnit:
                soundClipsSelectUnit = unit.SoundSelect;
                _currentIndex = _indexSelect = UpdateIndex(_indexSelect);
                break;
            case EnumStatusGame.SelectedCell:
                soundClipsSelectUnit = unit.SoundGoCell;
                _currentIndex = _indexCell = UpdateIndex(_indexCell);
                break;
            case EnumStatusGame.Hit:
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

    public void SoundPlayDead(Unit unit)
    {
        int randomIndex = UnityEngine.Random.Range(0, unit.SoundDead.Count - 1);
        unit.AudioSoundSource.PlayOneShot(unit.SoundDead[randomIndex]);
    }

    public void SoundPlayerAttack(Unit selected, Unit target)
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