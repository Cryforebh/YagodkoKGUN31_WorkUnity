using TMPro;
using UnityEngine;
using Zenject;

public class TextColorForPlayer : MonoBehaviour
{
    private ColorPlayersManager _colorContainer;

    [SerializeField] private EnumPlayers _player;

    private TMP_Text _textThis;

    private void Awake()
    {
        ColorUpdate();
        _colorContainer.ColorChangedEvent += _ => ColorUpdate();
    }

    [Inject]
    private void Construct(ColorPlayersManager colorContainer)
    {
        _colorContainer = colorContainer;

        _textThis = GetComponent<TMP_Text>();
    }

    private void ColorUpdate()
    {
        switch (_player)
        {
            case EnumPlayers.PlayerOne:
                _textThis.color = _colorContainer.PlayerOne;
                break;
            case EnumPlayers.PlayerTwo:
                _textThis.color = _colorContainer.PlayerTwo;
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        _colorContainer.ColorChangedEvent -= _ => ColorUpdate();
    }
}
