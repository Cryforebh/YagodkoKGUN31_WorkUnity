using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HealthBarManager : MonoBehaviour
{
    [Inject] private PlayerManager _playerManager;

    [Header("Настройки отображения уровня здоровья")]
    [SerializeField] private Vector3 _offcet = new Vector3(1, 1.5f, 0);
    [SerializeField] private Canvas _canvasPlayerOne;
    [SerializeField] private Image _healthBarImageOnePlayer;
    [SerializeField] private Canvas _canvasPlayerTwo;
    [SerializeField] private Image _healthBarImageTwoPlayer;

    [Header("Настройки Canvas")]
    [SerializeField] private RenderMode _renderMode = RenderMode.ScreenSpaceCamera;

    public Image HealthBarImageOnePlayer => _healthBarImageOnePlayer;
    public Image HealthBarImageTwoPlayer => _healthBarImageTwoPlayer;
    public Vector3 Offcet => _offcet;
    public Canvas CanvasPlayerOne => _canvasPlayerOne;
    public Canvas CanvasPlayerTwo => _canvasPlayerTwo;

    private void Start()
    {
        _canvasPlayerOne.enabled = false;
        _canvasPlayerTwo.enabled = false;

        ConfigureCanvas(_canvasPlayerOne, _playerManager.GetCameraPlayer(EnumPlayers.PlayerOne));
        ConfigureCanvas(_canvasPlayerTwo, _playerManager.GetCameraPlayer(EnumPlayers.PlayerTwo));
    }

    private void ConfigureCanvas(Canvas canvas, Camera targetCamera)
    {
        canvas.renderMode = _renderMode;
        canvas.worldCamera = targetCamera;
        canvas.planeDistance = 5f; // Оптимальное значение для 3D объектов
        canvas.enabled = false;
    }
}
