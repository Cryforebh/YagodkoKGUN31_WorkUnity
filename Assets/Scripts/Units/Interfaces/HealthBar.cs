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
        //_canvas.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_unit.IsDead) return;

        Debug.Log($"Event camera: {eventData.enterEventCamera?.name}");
        Debug.Log($"Press camera: {eventData.pressEventCamera?.name}");
        Debug.Log($"Raycast camera: {eventData.pointerCurrentRaycast.module?.eventCamera?.name}");

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
