using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WS_WhoNextPlayerTurn : MonoBehaviour
{
    [SerializeField] private Image Red;
    [SerializeField] private Image Blue;
    [SerializeField] private Image _line;
    [SerializeField] private Image _lineTwo;
    [SerializeField] private float _timeShow = 3f;
    [SerializeField] private bool _isTrueLine = false;
    [SerializeField] private bool _isTrueLineTwo = false;

    private IPlayerManager _playerManager;
    //private SignalBus _statusGame;
    private Coroutine _currentCoroutine;

    private Animation _redAnimation;
    private Animation _blueAnimation;
    private Animation _lineAnimation;
    private Animation _lineTwoAnimation;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (_lineTwo) _lineAnimation = _line.GetComponent<Animation>();
        if (_line) _lineTwoAnimation = _lineTwo.GetComponent<Animation>();

        _redAnimation = Red.GetComponent<Animation>();
        _blueAnimation = Blue.GetComponent<Animation>();
        _audioSource = gameObject.GetComponent<AudioSource>();

        ResetEnable();
    }

    private void Start()
    {
        _playerManager.OnActivePlayerChanged += ShowStartGame;
    }

    [Inject]
    private void Construct(IPlayerManager player/*, SignalBus signalBus*/)
    {
        _playerManager = player;
        //_statusGame = signalBus;

        //_statusGame.Subscribe<StatusGameSignal>(FakeSubscrube);
    }

    public void ShowStartGame(EnumPlayers players)
    {
        if (_playerManager.GetPlayersWhithUnits().Count < 2) return;

        ResetEnable();

        switch (players)
        {
            case EnumPlayers.PlayerOne:
                Blue.enabled = true;
                _blueAnimation.Play();
                break;
            case EnumPlayers.PlayerTwo:
                Red.enabled = true;
                _redAnimation.Play();
                break;
            default:
                break;
        }

        if (_isTrueLine)
        {
            _line.enabled = true;
            _lineAnimation.Play();
        }

        if (_isTrueLineTwo)
        {
            _lineTwoAnimation.Play();
            _lineTwo.enabled = true;
        }

        _audioSource.Play();
        //_currentCoroutine = StartCoroutine(TimeShow());
    }

    /*
    public void Show(StatusGameSignal statusGame)
    {
        if (_playerManager.GetPlayersWhithUnits().Count < 2) return;

        if (statusGame == StatusGameSignal.Return)
        {

            ResetEnable();

            ShowStartGame(_playerManager.ActivePlayer);

            //_currentCoroutine = StartCoroutine(TimeShow());
        }

        //if (statusGame == StatusGameSignal.SelectUnit) ResetEnable();
    }
    */

    //private void FakeSubscrube(StatusGameSignal statusGame) { } // Чтобы не вылезали знаки "внимание" в дебаге, так как не используется сигнал!

    private IEnumerator TimeShow()
    {
        yield return new WaitForSeconds(_timeShow);
        ResetEnable();
    }

    private void ResetEnable()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        if (_line)
        {
            _lineAnimation.Stop();
            _line.enabled = false;
        }
        if (_lineTwo)
        {
            _lineTwoAnimation.Stop();
            _lineTwo.enabled = false;
        }

        _blueAnimation.Stop();
        _redAnimation.Stop();
        _audioSource.Stop();

        Red.enabled = false;
        Blue.enabled = false;
    }

    private void OnDestroy()
    {
        //if (_statusGame != null) _statusGame.Unsubscribe<StatusGameSignal>(FakeSubscrube);

        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
    }
}
