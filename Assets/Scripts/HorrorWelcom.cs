using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class HorrorWelcom : MonoBehaviour
{

    [Inject] private MenuToggleManager _menuToggleManager;
    [Inject] private InputLevelManager _levelManager;

    [SerializeField] private bool _isStartActive = true;
    [SerializeField] private EnumHorrorType _enumHorrorType;
    [SerializeField] private bool _isAnimation;
    [SerializeField] private bool _isNextDisable;
    [SerializeField] private float _timeNextDisable;

    private AudioSource _audioSource;
    private bool isOneTry = false;

    private MeshRenderer _meshRenderer;
    private Animation _animation;

    private void Awake()
    {
        AwakeSetting();
    }

    private void Start()
    {
        switch (_enumHorrorType)
        {
            case EnumHorrorType.Welcom:
                break;
            case EnumHorrorType.HorrorHeadRight:
                _levelManager.Contlols.Game.HorrorHeadRight.canceled += _ => ShowControls();
                break;
            case EnumHorrorType.HorrorHeadLeft:
                _levelManager.Contlols.Game.HorrorHeadLeft.canceled += _ => ShowControls();
                break;
            case EnumHorrorType.HorrorHeadLong:
                _levelManager.Contlols.Game.HorrorHeadLong.canceled += _ => ShowControls();
                break;
            default:
                break;
        }

    }

    private void Update()
    {
        ShowWelcom();
        GetStatusMenu();
    }

    //----------------------------------
    
    private void AwakeSetting()
    {
        _audioSource = GetComponent<AudioSource>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _audioSource.enabled = false;
        _audioSource.loop = false;

        if (_enumHorrorType == EnumHorrorType.Welcom)
        {

        }
        else
        {
            if (_isAnimation == true)
            {
                _animation = GetComponent<Animation>();
                _animation.wrapMode = WrapMode.Once; // Анимация проигрывается один раз
            }
            if (_isStartActive == false) _meshRenderer.enabled = false;

        }
    }

    //----------------------------------

    private void ShowWelcom()
    {
        if (_enumHorrorType != EnumHorrorType.Welcom) return;
        if (_menuToggleManager.MadnessMode == false) return;
        if (isOneTry == true) return; 

        isOneTry = true;
        _audioSource.enabled = true;
        StartCoroutine(StartShowWelcomTime());
    }

    private IEnumerator StartShowWelcomTime()
    {
        yield return new WaitForSeconds(20);
        _audioSource.Play();
    }

    //----------------------------------

    private void ShowControls()
    {
        if (_menuToggleManager.MadnessMode == false) return;
        if (isOneTry == true) return;

        isOneTry = true;

        _audioSource.enabled = true;
        _meshRenderer.enabled = true;
        if (_audioSource.clip != null) _audioSource.Play();
        if (_animation != null) _animation.Play();

        if (_isNextDisable == false) return ;

        StartCoroutine(DisableControlsTime());
    }

    private IEnumerator DisableControlsTime()
    {
        yield return new WaitForSeconds(_timeNextDisable);
        _meshRenderer.enabled = false;
        if (gameObject.GetComponentInChildren<MeshRenderer>() != null) gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;

        if (_enumHorrorType == EnumHorrorType.HorrorHeadLong)
        {
            yield return new WaitForSeconds(_timeNextDisable + 2);
            _audioSource.enabled = false;
        }
    }

    //----------------------------------

    private void GetStatusMenu()
    {
        if (_menuToggleManager.IsEnterMenu == true)
        {
            _audioSource.Pause();

            if (_meshRenderer.enabled == true && _animation != null)
            {
                _animation.Stop();
                _audioSource.Stop();
            }

        }
        else if (_menuToggleManager.IsEnterMenu == false)
        {
            _audioSource.UnPause();
        }
    }

    private void OnDestroy()
    {
        switch (_enumHorrorType)
        {
            case EnumHorrorType.Welcom:
                break;
            case EnumHorrorType.HorrorHeadRight:
                _levelManager.Contlols.Game.HorrorHeadRight.canceled -= _ => ShowControls();
                break;
            case EnumHorrorType.HorrorHeadLeft:
                _levelManager.Contlols.Game.HorrorHeadLeft.canceled -= _ => ShowControls();
                break;
            case EnumHorrorType.HorrorHeadLong:
                _levelManager.Contlols.Game.HorrorHeadLong.canceled -= _ => ShowControls();
                break;
            default:
                break;
        }
    }
}

public enum EnumHorrorType
{
    Welcom = 0,
    HorrorHeadRight = 1,
    HorrorHeadLeft = 2,
    HorrorHeadLong = 3,
}
