using UnityEngine;
using Zenject;

public class C_PlayerController : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private C_MoveCommand _moveCommand;
    [Inject] private IPlayerManager _playerManager;

    private C_IGameplayCommand _command;

    private void Awake()
    {
        Construct();
    }

    private void Construct()
    {
        _command = _moveCommand;
        _signalBus.Subscribe<EnumGameEvent>(MoveUnit);
    }

    private void MoveUnit(EnumGameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case EnumGameEvent.Empty:
                break;
            case EnumGameEvent.SelectedUnit:
                break;
            case EnumGameEvent.SelectedCell:
                Debug.Log("Сигнал SelectedCell сработал");
                _command.Interact();
                break;
            case EnumGameEvent.EndMove:
                Debug.Log("Сигнал EndMove сработал");
                _playerManager.CheckWinner();
                _playerManager.ChangePlayer();
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<EnumGameEvent>(MoveUnit);
    }
}
