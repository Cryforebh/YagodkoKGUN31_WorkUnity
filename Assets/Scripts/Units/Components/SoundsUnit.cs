using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class SoundsUnit : MonoBehaviour
{
    [Inject] private ContainerStatusGame _containerStatusGame;
    [Inject] private MoveSystem _moveSystem;

    private List<AudioClip> soundClipsSelectUnit;

    private void Awake()
    {
        _containerStatusGame = FindObjectOfType<ContainerStatusGame>();
        _moveSystem = FindObjectOfType<MoveSystem>();
    }

    private void Start()
    {

    }

    public void SoundPlayOnUnitAndStatusGame(Unit unit, EnumStatusGame statusGame)
    {
        switch (statusGame)
        {
            case EnumStatusGame.Empty:
                soundClipsSelectUnit = unit.SoundSelect;
                break;
            case EnumStatusGame.SelectedUnit:
                soundClipsSelectUnit = unit.SoundSelect;
                break;
            case EnumStatusGame.SelectedCell:
                soundClipsSelectUnit = unit.SoundGoCell;
                break;
            case EnumStatusGame.Hit:
                soundClipsSelectUnit = unit.SoundAttack;
                break;
            default:
                Debug.LogWarning("Внимание: выбран статус игры, которому не пренадлежат звуки!");
                return;
        }

        if (soundClipsSelectUnit == null || soundClipsSelectUnit.Count <= 0) return;

        int randomIndex = UnityEngine.Random.Range(0, soundClipsSelectUnit.Count - 1);
        unit.AudioSoundSource.PlayOneShot(soundClipsSelectUnit[randomIndex]);
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
}
