using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Unit))]
public class HealthBar : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private PlayerManager _playerManager;
    [Inject] private HealthBarManager _healthBarManager;

    private Vector3 _offcet;
    private Unit _unit;
    private Canvas _canvasPlayerOne;
    private Canvas _canvasPlayerTwo;
    private float _maxHealth;
    private float _currentHealth;
    private Camera _cameraOnePlayer;
    private Camera _cameraTwoPlayer;

    private void Awake()
    {
        _healthBarManager = FindObjectOfType<HealthBarManager>();
        _playerManager = FindObjectOfType<PlayerManager>();

        _cameraOnePlayer = _playerManager.GetCameraPlayer(EnumPlayers.PlayerOne);
        _cameraTwoPlayer = _playerManager.GetCameraPlayer(EnumPlayers.PlayerTwo);
    }

    private void Start()
    {
        _offcet = _healthBarManager.Offcet;
        _unit = GetComponent<Unit>();

        _canvasPlayerOne = _healthBarManager.CanvasPlayerOne;
        _canvasPlayerTwo = _healthBarManager.CanvasPlayerTwo;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _canvasPlayerOne.enabled = false;
        if (_unit.IsDead) return;
        _canvasPlayerOne.enabled = true;
        DisplayOne();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_unit.IsDead) return;

        //Debug.Log($"Event camera: {eventData.enterEventCamera?.name}");
        //Debug.Log($"Press camera: {eventData.pressEventCamera?.name}");
        //Debug.Log($"Raycast camera: {eventData.pointerCurrentRaycast.module?.eventCamera?.name}");

        if (eventData.pressEventCamera == _cameraOnePlayer)
        {
            _canvasPlayerOne.enabled = true;
            DisplayOne();
        }
        if (eventData.pressEventCamera == _cameraTwoPlayer)
        {
            _canvasPlayerTwo.enabled = true;
            DisplayTwo();
        }

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

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pressEventCamera == _cameraOnePlayer)
        {
            _canvasPlayerOne.enabled = false;
        }
        if (eventData.pressEventCamera == _cameraTwoPlayer)
        {
            _canvasPlayerTwo.enabled = false;
        }

        _healthBarManager.LevelTwo.enabled = false;
        _healthBarManager.LevelThree.enabled = false;
        _healthBarManager.LevelFore.enabled = false;
        _healthBarManager.LevelFive.enabled = false;

        _healthBarManager.ModifierDefense.enabled = false;
        _healthBarManager.ModifierDamage.enabled = false;
        _healthBarManager.ModifierDrowRange.enabled = false;
    }

    private void PositionOnUnitOne()
    {
        _canvasPlayerOne.transform.position = _unit.transform.position + _offcet;

        //_canvas.transform.LookAt(_camera.transform);
        //_canvas.transform.Rotate(0, 180f, 0); // Корректировка ориентации
    }

    private void PositionOnUnitTwo()
    {
        _canvasPlayerTwo.transform.position = _unit.transform.position + _offcet + new Vector3(-2, 0, 0);
    }

    private void DisplayOne()
    {
        PositionOnUnitOne();

        _maxHealth = _unit.GetMaxHealth;
        _currentHealth = _unit.Health;

        _healthBarManager.HealthBarImageOnePlayer.fillAmount = _currentHealth / _maxHealth;
    }

    private void DisplayTwo()
    {
        PositionOnUnitTwo();

        _maxHealth = _unit.GetMaxHealth;
        _currentHealth = _unit.Health;

        _healthBarManager.HealthBarImageTwoPlayer.fillAmount = _currentHealth / _maxHealth;
    }

}
