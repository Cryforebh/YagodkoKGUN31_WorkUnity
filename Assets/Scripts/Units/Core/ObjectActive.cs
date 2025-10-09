using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(Collider))]
public class ObjectActive : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private AdvancedCursorController _cursor;
    [Inject] private AllCell _allCell;
    [Inject] private PlayerManager _playerManager;
    [Inject] private GameData _gameData;

    [Header("Настройки взаимодейсвтия с обьектом:")]
    [SerializeField, Tooltip("Где воспроизводится звук?")] private AudioSource _audioSource;
    [SerializeField, Tooltip("Какой звук производить, при активации обьекта?")] private AudioClip[] _soundStartActive;
    [SerializeField, Tooltip("Какой Курсор показывать, при наведении на обьект?")] private EnumStatusCursor _cursorSelect;
    [SerializeField, Tooltip("Удалять этот обьект после активации?")] private bool _deleteThisObject = false;
    [SerializeField, Tooltip("Удалять дочерний обьект после активации?")] private bool _deleteLocalObject = false;
    [SerializeField, Tooltip("Дочерний обьект для удаления.")] private GameObject _LocalObjectDelete;

    [Header("Настройки взаимодейсвтия с обьектом (Подстветка):")]
    [SerializeField, Tooltip("Подсвечивать?")] private bool _turnBacklight = false;
    [SerializeField, Tooltip("Рендер который подсвечивает обьект при наведении курсора.")] private MeshRenderer _objectRenderer;
    [SerializeField, Tooltip("Материал рендера.")] private Material _selectMaterial;

    [Header("Настройки Модификаций:")]
    [SerializeField, Tooltip("Какой модификатор применить?")] private EnumModifier _modifier = EnumModifier.None;
    [SerializeField, Tooltip("Какой звук производить, когда модификация закончиться?")] private AudioClip _endModifireAudioClip;


    [Header("Настройки ограничений по игрокам:")]
    [SerializeField, Tooltip("Зависит ли активация обьекта от того, какой игрок кликнул?")] private bool _playerAddiction = false;
    [SerializeField, Tooltip("Пренадлежит какому - то конкретному игроку?")] private EnumPlayers _player = EnumPlayers.None;
    [SerializeField, Tooltip("Сколько активаций может быть у каждого игрока? (Если есть зависимость)")] private int _countActive = 1;
    [SerializeField, Tooltip("Какой звук производить, когда у конкретного игрока закончатся активации?")] private AudioClip _noActive;



    //private AudioSource _audioSource;
    private int _currentIndex;
    private EnumPlayers _currentPlayer;
    private MeshRenderer _origanalMeshRenderer;
    private Material _originalMaterial;

    private bool _isCursorEnterTarget;

    private int _countActivePlayerOne;
    private int _countActivePlayerTwo;

    private void Awake()
    {
        Construct();
    }

    private void Construct()
    {
        //_audioSource = GetComponent<AudioSource>();
        _origanalMeshRenderer = _objectRenderer;
        _originalMaterial = _objectRenderer.material;

        _currentIndex = _soundStartActive.Length;

        _countActivePlayerOne = _countActive;
        _countActivePlayerTwo = _countActive;
    }

    private void OnEnable()
    {
        StartCoroutine(VerificationProcessLock());
    }

    private void OnDisable()
    {
        StopCoroutine(VerificationProcessLock());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Click();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _gameData.TargetObjectActiveEnter = this;
        Enter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _gameData.TargetObjectActiveEnter = null;
        Exit();
    }

    private void Enter()
    {
        if (_gameData.Lock == true) return;

        if (_player == EnumPlayers.None || _playerManager.ActivePlayer == _player)
        {
            _cursor.SetCursorState(_cursorSelect);

            if (_turnBacklight) _objectRenderer.material = _selectMaterial;
        }
    }

    private void Click()
    {
        if (_gameData.Lock == true || _gameData.LockClick) return;

        if (_playerAddiction && !AvailableAttempts()) return;

        if (_soundStartActive.Length == 0) return;

        _currentIndex = (_currentIndex + 1) % _soundStartActive.Length;

        _audioSource.Stop();
        _audioSource.PlayOneShot(_soundStartActive[_currentIndex]);

        //_currentPlayer = _playerManager.ActivePlayer;

        switch (_modifier)
        {
            case EnumModifier.None:
                break;
            case EnumModifier.Drink:
                _allCell.SetModifierAllUnits(EnumModifier.Drink, true, _player);
                _playerManager.OnActivePlayerChanged += RemoveMofifire;
                break;
            case EnumModifier.UpHelth:
                _allCell.SetModifierAllUnits(EnumModifier.UpHelth, true, _player);
                break;
            default:
                break;
        }

        if (_deleteLocalObject && _LocalObjectDelete != null)
        {
            _LocalObjectDelete.SetActive(false);
        }
        if (_deleteThisObject) gameObject.SetActive(false);
    }

    private void RemoveMofifire(EnumPlayers obj)
    {
        ModifireSakeEnd(obj);
    }

    private void ModifireSakeEnd(EnumPlayers players)
    {
        _playerManager.OnActivePlayerChanged -= RemoveMofifire;

        _allCell.SetModifierAllUnits(EnumModifier.Drink, false, _player);
        if (_endModifireAudioClip != null)
        {
            Debug.Log("Модификатор закончился!");
            _audioSource.PlayOneShot(_endModifireAudioClip);
        }
    }

    private void Exit()
    {
        _cursor.SetCursorState(EnumStatusCursor.Default);
        if (_turnBacklight) _objectRenderer.material = _originalMaterial;
    }

    private bool AvailableAttempts()
    {
        if (_playerManager.ActivePlayer == EnumPlayers.PlayerOne && (_player == EnumPlayers.None || _player == EnumPlayers.PlayerOne))
        {
            if (_countActivePlayerOne <= 0)
            {
                Debug.Log($"Активации для {EnumPlayers.PlayerOne} закончились!");
                if (_endModifireAudioClip != null) _audioSource.PlayOneShot(_noActive);
                return false;
            }
            _countActivePlayerOne -= 1;
            return true;
        }

        else if (_playerManager.ActivePlayer == EnumPlayers.PlayerTwo && (_player == EnumPlayers.None || _player == EnumPlayers.PlayerTwo))
        {
            if (_countActivePlayerTwo <= 0)
            {
                Debug.Log($"Активации для {EnumPlayers.PlayerTwo} закончились!");
                if (_endModifireAudioClip != null) _audioSource.PlayOneShot(_noActive);
                return false;
            }
            _countActivePlayerTwo -= 1;
            return true;
        }

        else return false;
    }

    /// <summary>
    /// Убирает признаки выделения обьекта во время блокировки, и возвращает при отмене блокировки (если указатель направлен на него)
    /// </summary>
    private IEnumerator VerificationProcessLock()
    {
        while (true)
        {
            if (_gameData.Lock == true)
            {
                _isCursorEnterTarget = true;
                _cursor.SetCursorState(EnumStatusCursor.Default);
                if (_turnBacklight) _objectRenderer.material = _originalMaterial;
            }
            else if (_isCursorEnterTarget)
            {
                _isCursorEnterTarget = false;

                if (_gameData.TargetObjectActiveEnter == this)
                {
                    Enter();
                    _gameData.TargetObjectActiveEnter = null;
                }
            }

            yield return null;
        }
    }
}
