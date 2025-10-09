using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StatisticsUnitsVisualManager : MonoBehaviour
{
    [Inject] private PlayerManager _playerManager;

    [Header("Настройки отображения уровня здоровья")]
    [SerializeField] private Vector3 _offcet = new Vector3(1, 1.5f, 0);
    [SerializeField] private Canvas _canvasPlayerOne;
    [SerializeField] private Image _healthBarImageOnePlayer;

    [Header("Настройки отображения уровня персонажа")]
    [SerializeField] private Image _levelTwo;
    [SerializeField] private Image _levelThree;
    [SerializeField] private Image _levelFore;
    [SerializeField] private Image _levelFive;

    [Header("Настройки отображения Модификаций")]
    [SerializeField] private Image _modifierDefense;
    [SerializeField] private Image _modifierDamage;
    [SerializeField] private Image _modifierDrowRange;

    [Header("Настройки Canvas")]
    [SerializeField] private RenderMode _renderMode = RenderMode.ScreenSpaceCamera;

    public Image HealthBarImageOnePlayer => _healthBarImageOnePlayer;
    public Vector3 Offcet => _offcet;
    public Canvas CanvasPlayerOne => _canvasPlayerOne;
    public Image LevelTwo => _levelTwo;
    public Image LevelThree => _levelThree;
    public Image LevelFore => _levelFore;
    public Image LevelFive => _levelFive;
    public Image ModifierDefense => _modifierDefense;
    public Image ModifierDamage => _modifierDamage;
    public Image ModifierDrowRange => _modifierDrowRange;

    private void Start()
    {
        _canvasPlayerOne.enabled = false;

        ConfigureCanvas(_canvasPlayerOne, _playerManager.GetCameraPlayer(EnumPlayers.PlayerOne));

        _levelTwo.enabled = false;
        _levelThree.enabled = false;
        _levelFore.enabled = false;
        _levelFive.enabled = false;

        _modifierDefense.enabled = false;
        _modifierDamage.enabled = false;
        _modifierDrowRange.enabled = false;
    }

    private void ConfigureCanvas(Canvas canvas, Camera targetCamera)
    {
        canvas.renderMode = _renderMode;
        canvas.worldCamera = targetCamera;
        canvas.planeDistance = 5f; // Оптимальное значение для 3D объектов
        canvas.enabled = false;
    }
}
