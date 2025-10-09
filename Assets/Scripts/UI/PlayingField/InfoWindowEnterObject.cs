using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class InfoWindowEnterObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Inject] LocalizationManager _localizationManager;
    [Inject] SettingObjectManager _settingObjectManager;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private EnumModifier _modifier;
    [SerializeField] private float _timeShowInfo = 1;
    [SerializeField] private Vector3 _offcet = new Vector3(1.5f, 5.77f, 6.72f);

    private GameData _gameData;
    private PlayerManager _playerManager;
    private Coroutine _currentCoroutine;

    private TMP_Text _textMesh;
    
    private GameObject _this;

    private void Awake()
    {
        _this = this.gameObject;

        _textMesh = _canvas.GetComponentInChildren<TMP_Text>();
        _localizationManager.ChangeLanguageEvent += _ => TextCount(_modifier);

        TextCount(_modifier);
        ResetEnable();
    }

    [Inject]
    private void Construct(GameData gameData, PlayerManager playerManager)
    {
        _gameData = gameData;
        _playerManager = playerManager;

        _playerManager.OnActivePlayerChanged += _ => ResetEnable();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_gameData.Lock || _gameData.LockClick) return;
        _currentCoroutine = StartCoroutine(ProcessShowInfo());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetEnable();
    }

    private IEnumerator ProcessShowInfo()
    {
        yield return new WaitForSeconds(_timeShowInfo);

        PositionOnUnit();
        _canvas.enabled = true;
    }

    private void ResetEnable()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        _canvas.enabled = false;
    }

    private void TextCount(EnumModifier modifier)
    {
        if (!_textMesh) return;

        switch (modifier)
        {
            case EnumModifier.Drink:
                _textMesh.text = $"{_localizationManager.GetText(EnumTextLocalization.sake)} " +
                    $"- {_localizationManager.GetText(EnumTextLocalization.sake_info)}";
                break;
            case EnumModifier.UpHelth:
                _textMesh.text = $"{_localizationManager.GetText(EnumTextLocalization.sushi)} - " +
                    $"{_localizationManager.GetText(EnumTextLocalization.sushi_info)}" +
                    $"{_settingObjectManager.MofifiSushiHealthUp}" +
                    $"{_localizationManager.GetText(EnumTextLocalization.sushi_infotwo)}";
                break;
            default:
                break;
        }
    }

    private void PositionOnUnit()
    {
        _canvas.transform.position = _this.transform.position + _offcet;
    }

    private void OnDestroy()
    {
        _playerManager.OnActivePlayerChanged -= _ => ResetEnable();
        _localizationManager.ChangeLanguageEvent -= _ => TextCount(_modifier);
    }
}