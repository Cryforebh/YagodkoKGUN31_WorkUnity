using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class C_Unit : MonoBehaviour, IUnitMain, IUnitVoice, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Inject] private C_MaterialsContainer _materialContainer;
    [Inject] private AllPlayer _allPlayer;
    [Inject] private VoiceUnitManager _voiceUnitManager;
    [Inject] private BoomEffectController _boomEffect;
    [Inject] private ColorPlayersManager _colorPlayersManager;

    [SerializeField] private EnumPlayers _player;
    [SerializeField] private bool _testEnemy;
    [SerializeField] private bool _testDamka;
    [SerializeField] private float _deathDelay = 0.5f;

    [SerializeField] private MeshRenderer _meshRendererDamka;

    private AudioSource _audioSource;
    private C_Cell _currentCell;
    private C_Cloth _clothStandard;

    private MeshRenderer _meshRendererCloth;
    private Material _oldMaterialCloth;
    private Material _enterMaterialCloth;
    private Material _dontEnterMaterialCloth;
    private bool _isEnemy = false;
    private bool _damka = false;
    private bool _death = false;

    public C_Cell Cell { get => _currentCell; set => _currentCell = value; }
    public MeshRenderer MeshRendererCloth { get => _meshRendererCloth; set => _meshRendererCloth = value; }
    public bool IsEnemy { get => _isEnemy; set => _isEnemy = value; }
    public bool IsDamka { get => _damka; set => _damka = value; }

    public Material OldMaterialCloth => _oldMaterialCloth;
    public Material EnterMaterialCloth => _enterMaterialCloth;
    public Material DontEnterMaterialCloth => _dontEnterMaterialCloth;

    public EnumPlayers Player { get => _player; set => _player = value; }

    public AudioSource AudioSoundSource => _audioSource;

    public Transform CurrentTransform => transform;

    public event Action<C_Unit> OnEnterEvent;
    public event Action<C_Unit> OnClickEvent;
    public event Action<C_Unit> OnExitEvent;


    private void Awake()
    {
        Constuct();
    }

    private void OnEnable()
    {
        _colorPlayersManager.Subscribe(MaterialUpdateColorChanged);
    }

    private void MaterialUpdateColorChanged(Color obj)
    {
        MaterialUpdate();
    }

    public void UpdateMeshRenderDamka()
    {
        _clothStandard.gameObject.SetActive(false);
        _meshRendererDamka.gameObject.SetActive(true);
        _meshRendererDamka.material = _meshRendererCloth.material;
        _meshRendererCloth = _meshRendererDamka;
    }

    private void Constuct()
    {
        _clothStandard = GetComponentInChildren<C_Cloth>();

        _meshRendererCloth = _clothStandard.GetComponent<MeshRenderer>();

        _isEnemy = _testEnemy;
        _damka = _testDamka;
        if (_damka) UpdateMeshRenderDamka();

        _audioSource = GetComponent<AudioSource>();

        MaterialUpdate();
    }

    private void MaterialUpdate()
    {
        if (_meshRendererCloth == null)
        {
            Debug.LogError($"У {this} - нет Меш Рендера _meshRendererCloth, Дочерний обьект пустой!");
            OnDestroy();
            return;
        }

        if (_player == EnumPlayers.PlayerOne) _oldMaterialCloth = _meshRendererCloth.material = _colorPlayersManager.MaterialPlayerOne;
        else _oldMaterialCloth = _meshRendererCloth.material = _colorPlayersManager.MaterialPlayerTwo;

        _enterMaterialCloth = _materialContainer.MaterialUnitEnter;
        _dontEnterMaterialCloth = _materialContainer.MaterialUnitShow;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnterEvent?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExitEvent?.Invoke(this);
    }

    public void Death()
    {
        if (_death) return;

        _death = true;
        Cell.ClearUnit(this);
        Debug.Log($"Вражеский персонаж {this} - убит, и удален из своей клетки {Cell}.");

        StartCoroutine(DeathProcess());
    }

    private IEnumerator DeathProcess()
    {
        _boomEffect.PlayExplosion(this);
        _voiceUnitManager.VoicePlayDead(this);
        _allPlayer.RemoveUnitOnPlayer(Player, this);

        yield return new WaitForSeconds(_deathDelay);

        foreach (var objChild in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            objChild.enabled = false;
        }

        yield return new WaitForSeconds(0.5f);

        Debug.Log($"{this.name}: Умер окончательно!");
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _colorPlayersManager.Unsubscribe(MaterialUpdateColorChanged);
    }

    private void OnDestroy()
    {
        _colorPlayersManager.Unsubscribe(MaterialUpdateColorChanged);
    }
}
