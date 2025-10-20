using UnityEngine;
using Zenject;

public class ActivePlayerSensor : MonoBehaviour
{
    [Inject] private IPlayerManager _playerManager;
    [Inject] private GameEvent _statusGame;

    [SerializeField] private MeshRenderer _meshRendererSensor;
    [SerializeField] private Material _materialRed;
    [SerializeField] private Material _materialBlue;

    private void Start()
    {
        _statusGame.OnStatusChanged += TurnPlayer;
        _playerManager.OnActivePlayerChanged += PlayerGoesFirst;
    }

    public void PlayerGoesFirst(EnumPlayers players)
    {
        switch (players)
        {
            case EnumPlayers.None:
                break;
            case EnumPlayers.PlayerOne:
                _meshRendererSensor.material = _materialBlue;
                break;
            case EnumPlayers.PlayerTwo:
                _meshRendererSensor.material = _materialRed;
                break;
            default:
                break;
        }
    }

    public void TurnPlayer(EnumGameEvent statusGame)
    {
        if (statusGame == EnumGameEvent.Empty)
        {
            PlayerGoesFirst(_playerManager.ActivePlayer);
        }
    }

    private void OnDestroy()
    {
        _statusGame.OnStatusChanged -= TurnPlayer;
        _playerManager.OnActivePlayerChanged -= PlayerGoesFirst;
    }
}
