using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(Unit))]
public class UnitSelectionPointer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private TypeGameManager _gameManager;
    [Inject] private ContainerStatusGame _statusGame;
    [Inject] private SelectionMaterialManager _selectionMaterials;
    [Inject] private MoveSystem _moveSystem;
    [Inject] private AdvancedCursorController _animatedCursor;
    [Inject] private SoundsUnit _soundUnit;
    [Inject] private GameData _gameData;

    public SignalBus SignalBusStatus;

    private static UnitSelectionPointer _oldSelectedUnitStatic;
    private bool _selected = false;
    private Unit _unit;
    private Material _originalMaterial;
    private MeshRenderer _meshRenderer;
    private Cloth _cloth;

    private bool _isCursorEnterTarget;
    private Unit _targetUnitEnter;

    public event Action<Unit> OnUnitSelected;
    public event Action<Unit> OnUnitEnter;
    public event Action<Unit> OnUnitExit;

    private void Awake()
    {
        _unit = GetComponent<Unit>();

        SetInjectionAndValidateDependencies(); // Проверка всех зависимостей и инженктирование DI при необходимости

        _statusGame.OnStatusChanged += ResetMaterialOldUnit;
    }

    private void Start()
    {
        _cloth = GetComponentInChildren<Cloth>();
        _meshRenderer = _cloth.GetComponent<MeshRenderer>();
        _meshRenderer.material = _selectionMaterials.GetMaterialPlayer(_unit.Player);
        _originalMaterial = _meshRenderer.material;
    }

    private void Update()
    {
        VerificationProcessLock();
    }

    /// <summary>
    /// Главная функция для выбора персонажа.
    /// </summary>
    /// <param name="selected">Отмечать персонажа, как выбранного?</param>
    /// <param name="setActionTarget">Передавать персонажа, как вражеского?</param>
    /// <param name="nextStatusGame">На какую стадию игры переводить?</param>
    /// <param name="setMaterial">Какой материал установить персонажу?</param>
    private void MainSelectAction(bool selected, bool setActionTarget, EnumStatusGame nextStatusGame, Material setMaterial)
    {
        _selected = selected;                                           // Статус выбранного персонажа

        if (setActionTarget) _moveSystem.SetTargetActionUnit(_unit);    // Если true, то передает выбранного персонажа как ActionTarget в _moveSystem
        else
        {
            _moveSystem.SetTargetSelectedUnit(_unit);                   // Если false, то передает выбранного персонажа как SelectedUnit в _moveSystem
            if (_oldSelectedUnitStatic != null)                         // и прошлого выбранного персонажа как OldUnit в _moveSystem
                _moveSystem.SetOldUnit(_oldSelectedUnitStatic._unit);

            _moveSystem.ForcedСallSelect();                             // Принудительный вызов смены доступных клеток если статус игры не менялся
        }

        _statusGame.StatusUpdate(nextStatusGame);                       // Переводит на новую стадию игры

        _meshRenderer.material = setMaterial;                           // Устанавливает материал для выбранного персонажа
    }

    private void SelectAttack()
    {
        MainSelectAction(false, true, EnumStatusGame.Hit, _originalMaterial);
        ResetOldUnit();
        OnUnitSelected?.Invoke(_unit);
    }

    private void Select()
    {
        MainSelectAction(true, false, EnumStatusGame.SelectedUnit, _selectionMaterials.GetFocusMaterialUnit);
        OnUnitSelected?.Invoke(_unit);
        ResetOldUnit();
        _oldSelectedUnitStatic = this;
    }

    private void ResetOldUnit()
    {
        if (_oldSelectedUnitStatic && _oldSelectedUnitStatic._unit != _unit)
        {
            _oldSelectedUnitStatic._meshRenderer.material = _oldSelectedUnitStatic._originalMaterial;
            _oldSelectedUnitStatic._selected = false;
            _oldSelectedUnitStatic = null;
        }
        if (_statusGame.Status == EnumStatusGame.SelectedCell
            && _unit.StatusUnit == EnumStatusUnitEnemy.My)
            _meshRenderer.material = _originalMaterial;
    }

    private void ResetMaterialOldUnit(EnumStatusGame obj) => ResetOldUnit();

    public void OnPointerEnter(PointerEventData eventData)
    {
        _gameData.TargetUnitEnter = _unit;
        Enter();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Click();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _gameData.TargetUnitEnter = null;
        Exit();
    }

    private void Enter()
    {
        if (_gameData.Lock == true) return;
        if (_unit.IsDead) return;
        if (_selectionMaterials == null) return;

        OnUnitEnter?.Invoke(_unit);

        // Стадия - выбор действия: Наведение курсора - на врага 
        if (_unit.StatusUnit == EnumStatusUnitEnemy.Enemy && _statusGame.Status == EnumStatusGame.SelectedUnit)
        {
            // Проверяем растояние до врага (чтобы наведение не фиксировалось, если враг далеко)
            if (!_moveSystem.IsAvailableCell(_unit)) return;


            // Курсор - Атака
            if (_oldSelectedUnitStatic._unit.Class == EnumStatusUnitClass.Samurai) _animatedCursor.SetCursorState(EnumStatusCursor.Attack);
            else _animatedCursor.SetCursorState(EnumStatusCursor.AttackDrow);

            _meshRenderer.material = _selectionMaterials.GetFocusMaterialEnemyUnit;
        }
        // Стадия - выбор персонажа: Наведение курсора - на врага 
        else if (_unit.StatusUnit == EnumStatusUnitEnemy.Enemy && _statusGame.Status != EnumStatusGame.SelectedUnit)
        {

        }
        // Стадия - любая: Наведение курсора - на дружественного персонажа
        else
        {
            _animatedCursor.SetCursorState(EnumStatusCursor.Select);
            _meshRenderer.material = _selectionMaterials.GetFocusMaterialUnit;
        }

    }

    private void Click()
    {
        if (_gameData.Lock == true || _gameData.LockClick) return;
        if (_unit.IsDead) return;

        // Стадия - выбор персонажа: Нажатие курсором - на врага (Атака)
        if (_statusGame.Status == EnumStatusGame.SelectedUnit
            && _unit.StatusUnit == EnumStatusUnitEnemy.Enemy)
        {
            // Проверяем растояние до врага (чтобы клик не фиксировался, если враг далеко)
            if (!_moveSystem.IsAvailableCell(_unit)) return;

            // Сюда можно добавлять любые действия

            SignalBusStatus.Fire(StatusGameSignal.SelectAttack);

            // Курсор - Дефолт
            _animatedCursor.SetCursorState(EnumStatusCursor.Default);

            SelectAttack();
        }
        // Стадия - выбор персонажа или выбор действия: Нажатие курсором - на дружественного персонажа (Выбор для передвижения)
        else if ((_statusGame.Status == EnumStatusGame.SelectedUnit ||
            _statusGame.Status == EnumStatusGame.Empty)
            && _unit.StatusUnit == EnumStatusUnitEnemy.My)
        {
            // Сюда можно добавлять любые действия

            SignalBusStatus.Fire(StatusGameSignal.SelectUnit);

            _soundUnit.SoundPlayOnUnitAndStatusGame(_unit, EnumStatusGame.SelectedUnit);

            Select();
        }
    }

    private void Exit()
    {
        if (_unit.IsDead) return;

        _animatedCursor.SetCursorState(EnumStatusCursor.Default);
        OnUnitExit?.Invoke(_unit);
        if (!_selected || (_selected && _unit.StatusUnit == EnumStatusUnitEnemy.Enemy) || (_oldSelectedUnitStatic != this && _unit.StatusUnit == EnumStatusUnitEnemy.My))
            _meshRenderer.material = _originalMaterial;
    }

    private bool ValidateDependencies()
    {
        var isValid = true;

        if (_statusGame == null)
        {
            Debug.LogError("ContainerStatusGame не инжектирован", this);
            isValid = false;
        }

        if (_moveSystem == null)
        {
            Debug.LogError("MoveSystem не инжектирован", this);
            isValid = false;
        }

        if (_selectionMaterials == null)
        {
            Debug.LogError("SelectionMaterialManager не инжектирован", this);
            isValid = false;
        }

        if (_animatedCursor == null)
        {
            Debug.LogError("AnimatedCursor не инжектирован", this);
            isValid = false;
        }

        if (_unit == null)
        {
            Debug.LogError("Unit компонент отсутствует", this);
            isValid = false;
        }
        return isValid;
    }

    private void SetInjectionAndValidateDependencies()
    {
        if (_statusGame == null && _selectionMaterials == null && _moveSystem == null && _animatedCursor == null)
        {
            _gameData = FindObjectOfType<GameData>();
            _gameManager = FindObjectOfType<TypeGameManager>();
            _statusGame = FindObjectOfType<ContainerStatusGame>();
            _selectionMaterials = FindObjectOfType<SelectionMaterialManager>();
            _moveSystem = FindObjectOfType<MoveSystem>();
            _animatedCursor = FindObjectOfType<AdvancedCursorController>();
            _soundUnit = FindObjectOfType<SoundsUnit>();
        }

        ValidateDependencies();
    }

    /// <summary>
    /// Убирает признаки выделения обьекта во время блокировки, и возвращает при отмене блокировки (если указатель направлен на него)
    /// </summary>
    private void VerificationProcessLock()
    {
        if (_gameData.Lock == true)
        {
            _isCursorEnterTarget = true;
            _animatedCursor.SetCursorState(EnumStatusCursor.Default);
            _meshRenderer.material = _originalMaterial;
        }
        else if (_isCursorEnterTarget)
        {
            _isCursorEnterTarget = false;

            if (_gameData.TargetUnitEnter == _unit)
            {
                Enter();
                _gameData.TargetUnitEnter = null;
            }
        }
    }

    private void OnDestroy()
    {
        _statusGame.OnStatusChanged -= ResetMaterialOldUnit;
    }
}