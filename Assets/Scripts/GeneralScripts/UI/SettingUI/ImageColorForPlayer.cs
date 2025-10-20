using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ImageColorForPlayer : MonoBehaviour
{
    private ColorPlayersManager _colorContainer;

    [SerializeField] private EnumPlayers _player;

    private Image _image;

    private void Start()
    {
        _colorContainer.Subscribe(ColorChangedUpdate);
    }

    private void ColorChangedUpdate(Color obj)
    {
        ColorUpdate();
    }

    [Inject]
    private void Construct(ColorPlayersManager colorContainer)
    {
        _colorContainer = colorContainer;

        _image = GetComponent<Image>();

        ColorUpdate();
    }

    private void ColorUpdate()
    {
        if (_image == null) 
        {
            Debug.LogError($"Image {this} - не существует для обновления цвета");
            return;
        }

        switch (_player)
        {
            case EnumPlayers.PlayerOne:
                _image.color = _colorContainer.PlayerOne;
                break;
            case EnumPlayers.PlayerTwo:
                _image.color = _colorContainer.PlayerTwo;
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        _colorContainer.Unsubscribe(ColorChangedUpdate);
    }
}
