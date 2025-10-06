using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(Unit))]
public class HealthBar : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private StatisticsUnitsVisualManager _healthBarManager;
    [Inject] private GameData _gameData;

    private Vector3 _offcet;
    private Unit _unit;
    private Canvas _canvasPlayer;
    private float _maxHealth;
    private float _currentHealth;

    private bool _isCursorEnterTarget;

    private void Awake()
    {
        _healthBarManager = FindObjectOfType<StatisticsUnitsVisualManager>();
        _gameData = FindObjectOfType<GameData>();
    }

    private void Start()
    {
        _offcet = _healthBarManager.Offcet;
        _unit = GetComponent<Unit>();

        _canvasPlayer = _healthBarManager.CanvasPlayerOne;
    }

    private void Update()
    {
        VerificationProcessLock();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _gameData.StatisticUnitVisual = this;
        Click();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _gameData.StatisticUnitVisual = this;
        Enter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _gameData.StatisticUnitVisual = null;
        Exit();
    }

    private void Enter()
    {
        if (_gameData.Lock == true) return;
        if (_unit.IsDead) return;

        _canvasPlayer.enabled = true;
        Display();

        if (_unit.Level == 2)
        {
            _healthBarManager.LevelTwo.enabled = true;
        }
        if (_unit.Level == 3)
        {
            _healthBarManager.LevelThree.enabled = true;
        }
        if (_unit.Level == 4)
        {
            _healthBarManager.LevelFore.enabled = true;
        }
        if (_unit.Level == 5)
        {
            _healthBarManager.LevelFive.enabled = true;
        }

        if (_unit.ModifierDefense) _healthBarManager.ModifierDefense.enabled = true;
        if (_unit.ModifierDamage > 0 && _unit.Class == EnumStatusUnitClass.Samurai) _healthBarManager.ModifierDamage.enabled = true;
        if (_unit.ModifierDrowRange && _unit.Class == EnumStatusUnitClass.Ranger) _healthBarManager.ModifierDrowRange.enabled = true;
    }

    private void Click()
    {
        if (_gameData.Lock == true /*|| _gameData.LockClick*/) return;

        _canvasPlayer.enabled = false;
        if (_unit.IsDead) return;
        _canvasPlayer.enabled = true;
        Display();
    }

    private void Exit()
    {
        _canvasPlayer.enabled = false;

        _healthBarManager.LevelTwo.enabled = false;
        _healthBarManager.LevelThree.enabled = false;
        _healthBarManager.LevelFore.enabled = false;
        _healthBarManager.LevelFive.enabled = false;

        _healthBarManager.ModifierDefense.enabled = false;
        _healthBarManager.ModifierDamage.enabled = false;
        _healthBarManager.ModifierDrowRange.enabled = false;
    }

    private void PositionOnUnit()
    {
        _canvasPlayer.transform.position = _unit.transform.position + _offcet;
    }

    private void Display()
    {
        PositionOnUnit();

        _maxHealth = _unit.GetMaxHealth;
        _currentHealth = _unit.Health;

        _healthBarManager.HealthBarImageOnePlayer.fillAmount = _currentHealth / _maxHealth;
    }

    /// <summary>
    /// Убирает признаки выделения обьекта во время блокировки, и возвращает при отмене блокировки (если указатель направлен на него)
    /// </summary>
    private void VerificationProcessLock()
    {
        if (_gameData.Lock == true)
        {
            _isCursorEnterTarget = true;
            _canvasPlayer.enabled = false;
        }
        else if (_isCursorEnterTarget)
        {
            _isCursorEnterTarget = false;

            if (this == _gameData.StatisticUnitVisual)
            {
                Enter();
                _gameData.StatisticUnitVisual = null;
            }
        }
    }
}
