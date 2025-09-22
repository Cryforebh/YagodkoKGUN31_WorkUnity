using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(Collider))]
public class ObjectActive : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private AdvancedCursorController _cursor;
    [Inject] private AllCell _allCell;
    [Inject] private PlayerManager _playerManager;

    [Header("Настройки взаимодейсвтия с обьектом:")]
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
    [SerializeField, Tooltip("Сколько будет длиться модификация?")] private float _modifireDelay = 10f;
    [SerializeField, Tooltip("Какой звук производить, когда модификация закончиться?")] private AudioClip _endModifireAudioClip;


    [Header("Настройки ограничений по игрокам:")]
    [SerializeField, Tooltip("Зависит ли активация обьекта от того, какой игрок кликнул?")] private bool _playerAddiction = false;
    [SerializeField, Tooltip("Пренадлежит какому - то конкретному игроку?")] private EnumPlayers _player = EnumPlayers.None;
    [SerializeField, Tooltip("Сколько активаций может быть у каждого игрока? (Если есть зависимость)")] private int _countActive = 1;
    [SerializeField, Tooltip("Какой звук производить, когда у конкретного игрока закончатся активации?")] private AudioClip _noActive;



    private AudioSource _audioSource;
    private int _currentIndex;
    private EnumPlayers _currentPlayer;
    private MeshRenderer _origanalMeshRenderer;
    private Material _originalMaterial;

    private int _countActivePlayerOne;
    private int _countActivePlayerTwo;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _origanalMeshRenderer = _objectRenderer;
        _originalMaterial = _objectRenderer.material;

        _currentIndex = _soundStartActive.Length;

        _countActivePlayerOne = _countActive;
        _countActivePlayerTwo = _countActive;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (!gameObject.activeSelf || !_LocalObjectDelete.activeSelf) return;

        if (_playerAddiction && !AvailableAttempts()) return;

        if (_soundStartActive.Length == 0) return;

        _currentIndex = (_currentIndex + 1) % _soundStartActive.Length;
        _audioSource.PlayOneShot(_soundStartActive[_currentIndex]);

        _currentPlayer = _playerManager.ActivePlayer;

        switch (_modifier)
        {
            case EnumModifier.None:
                break;
            case EnumModifier.Drink:
                _allCell.SetModifierAllUnits(EnumModifier.Drink, true, _currentPlayer);
                StartCoroutine(ModifireTime());
                break;
                case EnumModifier.UpHelth:
                _allCell.SetModifierAllUnits(EnumModifier.UpHelth, true, _currentPlayer);
                break;
            default:
                break;
        }

        if (_deleteLocalObject && _LocalObjectDelete != null) _LocalObjectDelete.SetActive(false);
        if (_deleteThisObject) gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (gameObject.activeSelf == false || _LocalObjectDelete.activeSelf == false) return;

        if (_player == EnumPlayers.None || _playerManager.ActivePlayer == _player)
        {
            _cursor.SetCursorState(_cursorSelect);
            
            if (_turnBacklight) _objectRenderer.material = _selectMaterial;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _cursor.SetCursorState(EnumStatusCursor.Default);
        if (_turnBacklight) _objectRenderer.material = _originalMaterial;
    }

    private IEnumerator ModifireTime()
    {
        yield return new WaitForSeconds(_modifireDelay);
        _allCell.SetModifierAllUnits(EnumModifier.Drink, false, _currentPlayer);
        if (_endModifireAudioClip != null)
        {
            Debug.Log("Модификатор закончился!");
            _audioSource.PlayOneShot(_endModifireAudioClip);
        }
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
}
