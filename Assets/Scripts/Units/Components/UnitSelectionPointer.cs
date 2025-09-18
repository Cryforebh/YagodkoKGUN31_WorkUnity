using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

// Имеется проблема - если нажать на клетку для перемещения не попадая заранее мышкой на юнита - то юнит останется со старым материалом!

[RequireComponent(typeof(Unit))]
public class UnitSelectionPointer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private TypeGameManager _gameManager;
    [Inject] private ContainerStatusGame _statusGame;
    [Inject] private SelectionMaterialManager _selectionMaterials;
    [Inject] private MoveSystem _moveSystem;
    [Inject] private AdvancedCursorController _animatedCursor;
    [Inject] private SoundsUnit _soundUnit;
    //[Inject] private UnitManager _unitManager;

    private static UnitSelectionPointer _oldSelectedUnitStatic;
    private bool _selected = false;
    private Unit _unit;
    private Material _originalMaterial;
    private MeshRenderer _meshRenderer;
    private Cloth _cloth;
    
    //private Material _originalMaterialCloth;
    //private MeshRenderer _meshRendererCloth;

    public event Action<Unit> OnUnitSelected;
    public event Action<Unit> OnUnitEnter;
    public event Action<Unit> OnUnitExit;

    private void Awake()
    {
        _unit = GetComponent<Unit>();
        //_soundUnit = GetComponent<SoundsUnit>();
        //_meshRenderer = GetComponent<MeshRenderer>();

        //_originalMaterial = _meshRenderer.material;

        SetInjectionAndValidateDependencies(); // Проверка всех зависимостей и инженктирование DI при необходимости

        _statusGame.OnStatusChanged += ResetMaterialOldUnit;
    }

    private void Start()
    {
        _cloth = GetComponentInChildren<Cloth>();
        //_meshRendererCloth = _cloth.GetComponent<MeshRenderer>();
        _meshRenderer = _cloth.GetComponent<MeshRenderer>();
        _meshRenderer.material = _selectionMaterials.GetMaterialPlayer(_unit.Player);
        _originalMaterial = _meshRenderer.material;

        //_meshRendererCloth.material = _selectionMaterials.GetMaterialPlayer(_unit.Player);
        //_originalMaterialCloth = _meshRendererCloth.material;
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

        //_soundUnit.SoundPlayOnUnitAndStatusGame(_unit, nextStatusGame); // Воспроизводит звук персонажа в зависимости от стадии игры

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
        if (_unit.IsDead) return;
        if (_selectionMaterials == null) return;

        OnUnitEnter?.Invoke(_unit);

        // Стадия - выбор действия: Наведение курсора - на врага 
        if (_unit.StatusUnit == EnumStatusUnitEnemy.Enemy && _statusGame.Status == EnumStatusGame.SelectedUnit)
        {
            // Проверяем растояние до врага (чтобы наведение не фиксировалось, если враг далеко)
            if (!_moveSystem.IsAvailableCell(_unit)) return;


            // Курсор - Атака
            _animatedCursor.SetCursorState(EnumStatusCursor.Attack);

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_unit.IsDead) return;

        // Стадия - выбор персонажа: Наведение курсора - на врага (Атака)
        if (_statusGame.Status == EnumStatusGame.SelectedUnit
            && _unit.StatusUnit == EnumStatusUnitEnemy.Enemy)
        {
            // Проверяем растояние до врага (чтобы клик не фиксировался, если враг далеко)
            if (!_moveSystem.IsAvailableCell(_unit)) return;

            // Сюда можно добавлять любые действия

            

            // Курсор - Дефолт
            _animatedCursor.SetCursorState(EnumStatusCursor.Default);

            SelectAttack();

            //_soundUnit.SoundPlay();
        }
        // Стадия - выбор персонажа или выбор действия: Наведение курсора - на дружественного персонажа (Выбор для передвижения)
        else if ((_statusGame.Status == EnumStatusGame.SelectedUnit ||
            _statusGame.Status == EnumStatusGame.Empty)
            && _unit.StatusUnit == EnumStatusUnitEnemy.My)
        {
            // Сюда можно добавлять любые действия
            //_soundUnit.SoundPlay();

            _soundUnit.SoundPlayOnUnitAndStatusGame(_unit, EnumStatusGame.SelectedUnit);

            Select();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
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
            _gameManager = FindObjectOfType<TypeGameManager>();
            _statusGame = FindObjectOfType<ContainerStatusGame>();
            _selectionMaterials = FindObjectOfType<SelectionMaterialManager>();
            _moveSystem = FindObjectOfType<MoveSystem>();
            _animatedCursor = FindObjectOfType<AdvancedCursorController>();
            _soundUnit = FindObjectOfType<SoundsUnit>();
        }

        ValidateDependencies();
    }

    //private void IsTypeGame()
    //{
    //    if(_gameManager.GameType == EnumTypeGame.PVPLocal)
    //    {
    //        if (_unit.Player)
    //    }
    //}

    private void OnDestroy()
    {
        _statusGame.OnStatusChanged -= ResetMaterialOldUnit;
    }
}




//[RequireComponent(typeof(Unit))]
//public class UnitSelectionPointer : MonoBehaviour,
//    IPointerEnterHandler,
//    IPointerClickHandler,
//    IPointerExitHandler
//{
//    // Инжектим зависимости через DI-контейнер (Zenject или аналог)
//    [Inject] private ContainerStatusGame _statusGame;               // Хранит глобальное состояние игры
//    [Inject] private SelectionMaterialManager _selectionMaterials;  // Управление материалами выделения
//    [Inject] private MoveSystem _moveSystem;                        // Система управления перемещением

//    private Unit _unit;                 // Ссылка на родительский компонент Unit
//    private Material _originalMaterial; // Исходный материал объекта
//    private MeshRenderer _meshRenderer; // Рендерер для изменения материалов
//    private bool _isSelected;           // Флаг текущего выделения

//    // События для внешней подписки
//    public event Action<Unit> OnUnitSelected; // Вызывается при любом выборе юнита
//    public event Action<Unit> OnUnitEnter;    // Наведение курсора
//    public event Action<Unit> OnUnitExit;     // Уход курсора

//    private void Awake()
//    {
//        // Резервный поиск зависимостей если DI не сработал
//        if (_statusGame == null && _selectionMaterials == null && _moveSystem == null)
//        {
//            _statusGame = FindObjectOfType<ContainerStatusGame>();
//            _selectionMaterials = FindObjectOfType<SelectionMaterialManager>();
//            _moveSystem = FindObjectOfType<MoveSystem>();
//        }

//        // Инициализация компонентов
//        _unit = GetComponent<Unit>();
//        _meshRenderer = GetComponent<MeshRenderer>();
//        _originalMaterial = _meshRenderer.material;     // Сохраняем оригинальный материал

//        if (!ValidateDependencies()) enabled = false;   // Отключаем при ошибках
//    }

//    private bool ValidateDependencies()
//    {
//        // Проверка критически важных зависимостей
//        if (_statusGame == null || _moveSystem == null || _unit == null)
//        {
//            Debug.LogError("Отсутствуют критические зависимости!", this);
//            return false;
//        }
//        return true;
//    }

//    public void OnPointerClick(PointerEventData eventData)
//    {
//        if (!enabled) return;

//        // Сброс предыдущего выделения через контейнер состояния
//        if (_statusGame.LastSelectedPointer != null)
//        {
//            _statusGame.LastSelectedPointer.ResetSelection();
//        }

//        // Установка нового состояния выделения
//        _isSelected = true;
//        _statusGame.LastSelectedPointer = this;
//        _meshRenderer.material = GetSelectedMaterial();

//        // ОСНОВНАЯ ИГРОВАЯ ЛОГИКА ВЫБОРА:
//        // Атака
//        if (ShouldAttackEnemy())
//        {
//            _moveSystem.SetActionTarget(_unit);
//            _statusGame.StatusUpdate(EnumStatusGame.Hit);

//            // Запуск корутины для сброса после атаки
//            StartCoroutine(ResetAfterAttack());
//        }
//        // Перемещение
//        else if (ShouldSelectAlly())
//        {
//            _moveSystem.SetSelectedUnit(_unit);
//            _statusGame.StatusUpdate(EnumStatusGame.SelectedUnit);
//        }

//        // Уведомление подписчиков о выборе
//        OnUnitSelected?.Invoke(_unit);
//    }

//    private IEnumerator ResetAfterAttack()
//    {
//        // Ожидание завершения логики атаки в следующем кадре
//        yield return null;

//        // Специальный сброс только для вражеских юнитов
//        //if (_unit.GetStatusUnit == EnumStatusUnit.My)
//        //{
//            ResetSelection();
//            _statusGame.LastSelectedPointer = null;
//        //}
//    }

//    // Условие для атаки врага
//    private bool ShouldAttackEnemy() =>
//        _statusGame.Status == EnumStatusGame.SelectedUnit &&    // Должен быть выбран союзник
//        _unit.GetStatusUnit == EnumStatusUnit.Enemy;            // Цель - враг

//    // Условие для выбора союзника
//    private bool ShouldSelectAlly() =>
//        _unit.GetStatusUnit == EnumStatusUnit.My;               // Только свои юниты

//    public void ResetSelection()
//    {
//        _isSelected = false;
//        _meshRenderer.material = _originalMaterial;             // Восстановление исходного материала
//    }

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        if (!_isSelected)
//        {
//            OnUnitEnter?.Invoke(_unit);
//            _meshRenderer.material = GetHoverMaterial();        // Временное выделение при наведении
//        }
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//        if (!_isSelected)
//        {
//            OnUnitExit?.Invoke(_unit);
//            _meshRenderer.material = _originalMaterial;         // Сброс при уходе курсора
//        }
//    }

//    // Выбор материала в зависимости от типа юнита
//    private Material GetSelectedMaterial() =>
//        _unit.GetStatusUnit == EnumStatusUnit.Enemy
//            ? _selectionMaterials.GetFocusMaterialEnemyUnit     // Красное выделение для врагов
//            : _selectionMaterials.GetFocusMaterialUnit;         // Синее для союзников

//    // Аналогично для состояния hover
//    private Material GetHoverMaterial() =>
//        _unit.GetStatusUnit == EnumStatusUnit.Enemy
//            ? _selectionMaterials.GetFocusMaterialEnemyUnit
//            : _selectionMaterials.GetFocusMaterialUnit;
//}



//[RequireComponent(typeof(Unit))]
//public class UnitSelectionPointer : MonoBehaviour,
//IPointerEnterHandler,
//IPointerClickHandler,
//IPointerExitHandler
//{


//    [SerializeField] private Material _focusMaterial;
//    [SerializeField] private Material _selectedMaterial;
//    private Material _originalMaterial;

//    private MeshRenderer _meshRenderer;
//    private Unit _unit;
//    private bool _isSelected;

//    public bool IsSelected => _isSelected;
//    public event Action<Unit> OnSelected;
//    public event Action<Unit> OnDeselected;

//    // Статическое событие для отслеживания смены выбора
//    public static event Action<Unit> OnSelectionChanged;


//    private void Awake()
//    {
//        _unit = GetComponent<Unit>();
//        _meshRenderer = GetComponent<MeshRenderer>();
//        _originalMaterial = _meshRenderer.material;

//        // Автоматическая подписка на свои же события
//        OnSelectionChanged += HandleSelectionChange;
//    }

//    private void OnDestroy()
//    {
//        OnSelectionChanged -= HandleSelectionChange;
//    }

//    public void OnPointerClick(PointerEventData eventData)
//    {
//        if (_isSelected)
//        {
//            Deselect();
//        }
//        else
//        {
//            Select();
//        }
//    }

//    private void Select()
//    {
//        _isSelected = true;
//        _meshRenderer.material = _selectedMaterial;
//        OnSelected?.Invoke(_unit);
//        OnSelectionChanged?.Invoke(_unit);
//    }

//    private void Deselect()
//    {
//        _isSelected = false;
//        _meshRenderer.material = _originalMaterial;
//        OnDeselected?.Invoke(_unit);
//        OnSelectionChanged?.Invoke(null);
//    }

//    private void HandleSelectionChange(Unit selectedUnit)
//    {
//        if (selectedUnit != _unit && _isSelected)
//        {
//            _isSelected = false;
//            _meshRenderer.material = _originalMaterial;
//        }
//    }

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        if (!_isSelected)
//        {
//            _meshRenderer.material = _focusMaterial;
//        }
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//        if (!_isSelected)
//        {
//            _meshRenderer.material = _originalMaterial;
//        }
//    }
//}





//[RequireComponent(typeof(Unit))]
//public class UnitSelectionPointer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
//{
//    [Inject] private ContainerStatusGame _statusGame;
//    [Inject] private SelectionMaterialManager _selectionMaterials;
//    [Inject] private MoveSystem _moveSystem;
//    //[Inject] private UnitManager _unitManager;

//    private Unit _unit;
//    private Material _originalMaterial;
//    private MeshRenderer _meshRenderer;

//    public event Action<Unit> OnUnitSelected;
//    public event Action<Unit> OnUnitEnter;
//    public event Action<Unit> OnUnitExit;

//    private void Awake()
//    {
//        // Добавляем проверки
//        if (_statusGame == null && _selectionMaterials == null && _moveSystem == null)
//        {
//            _statusGame = FindObjectOfType<ContainerStatusGame>();
//            _selectionMaterials = FindObjectOfType<SelectionMaterialManager>();
//            _moveSystem = FindObjectOfType<MoveSystem>();
//            //_unitManager = FindObjectOfType<UnitManager>();
//            //_allPlayer = FindObjectOfType<AllPlayer>();
//            //_playerManager = FindObjectOfType<PlayerManager>();
//        }

//        if (_statusGame == null)
//        {
//            Debug.LogError($"ContainerStatusGame не инжектирован в {gameObject.name}", this);
//            enabled = false;
//            return;
//        }

//        // Явная инициализация обязательных компонентов
//        _unit = GetComponent<Unit>();
//        _meshRenderer = GetComponent<MeshRenderer>();

//        if (_unit == null || _meshRenderer == null)
//        {
//            Debug.LogError($"Критические компоненты не найдены на {gameObject.name}", this);
//            enabled = false;
//            return;
//        }

//        _originalMaterial = _meshRenderer.material;
//    }

//    public void OnPointerClick(PointerEventData eventData)
//    {
//        // Проверка всех зависимостей
//        if (!ValidateDependencies()) return;

//        // Основная логика
//        // Удар
//        if (_statusGame.Status == EnumStatusGame.SelectedUnit
//            && _unit.GetStatusUnit == EnumStatusUnit.Enemy)
//        {
//            _moveSystem.SetActionTarget(_unit);
//            _statusGame.StatusUpdate(EnumStatusGame.Hit);
//            OnUnitSelected?.Invoke(_unit);
//        }
//        // Перемещение
//        else if (_unit.GetStatusUnit == EnumStatusUnit.My)
//        {
//            _moveSystem.SetSelectedUnit(_unit);
//            _statusGame.StatusUpdate(EnumStatusGame.SelectedUnit);
//            OnUnitSelected?.Invoke(_unit);
//        }
//    }

//    private bool ValidateDependencies()
//    {
//        var isValid = true;

//        if (_statusGame == null)
//        {
//            Debug.LogError("ContainerStatusGame не инжектирован", this);
//            isValid = false;
//        }

//        if (_moveSystem == null)
//        {
//            Debug.LogError("MoveSystem не инжектирован", this);
//            isValid = false;
//        }

//        if (_unit == null)
//        {
//            Debug.LogError("Unit компонент отсутствует", this);
//            isValid = false;
//        }

//        return isValid;
//    }

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        if (_selectionMaterials == null) return;

//        OnUnitEnter?.Invoke(_unit);

//        _meshRenderer.material = _unit.GetStatusUnit == EnumStatusUnit.Enemy
//            ? _selectionMaterials.GetFocusMaterialEnemyUnit
//            : _selectionMaterials.GetFocusMaterialUnit;
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//        OnUnitExit?.Invoke(_unit);
//        _meshRenderer.material = _originalMaterial;
//    }
//}




//public class Unit : MonoBehaviour,
//    IPointerEnterHandler,
//    IPointerClickHandler,
//    IPointerExitHandler
//{
//    [Header("Settings")]
//    [SerializeField] private float _moveSpeed = 3f;
//    [SerializeField] private float _heightOffset = 0.5f;

//    private Cell _currentCell;
//    private bool _isMoving = false;
//    public event System.Action<Unit> OnMoveEndCallback;

//    public void Initialize(Cell startCell)
//    {
//        _currentCell = startCell;
//        SnapToCell(_currentCell);
//    }

//    public void Move(Cell targetCell)
//    {
//        if (!CanMoveTo(targetCell) || _isMoving) return;

//        StartCoroutine(MovementRoutine(targetCell));
//    }

//    private IEnumerator MovementRoutine(Cell target)
//    {
//        _isMoving = true;
//        Vector3 startPos = transform.position;
//        Vector3 endPos = GetCellPosition(target);
//        float journeyTime = Vector3.Distance(startPos, endPos) / _moveSpeed;
//        float elapsedTime = 0f;

//        while (elapsedTime < journeyTime)
//        {
//            transform.position = Vector3.Lerp(
//                startPos,
//                endPos,
//                elapsedTime / journeyTime
//            );
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }

//        SnapToCell(target);
//        _currentCell = target;
//        _isMoving = false;
//        OnMoveEndCallback?.Invoke(this);
//    }

//    private void SnapToCell(Cell cell)
//    {
//        transform.position = GetCellPosition(cell);
//    }

//    private Vector3 GetCellPosition(Cell cell)
//    {
//        return cell.transform.position + Vector3.up * _heightOffset;
//    }

//    private bool CanMoveTo(Cell target)
//    {
//        return target != null && target.Unit == null;
//    }

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        _currentCell?.OnPointerEnter(eventData);
//    }

//    public void OnPointerClick(PointerEventData eventData)
//    {
//        _currentCell?.OnPointerClick(eventData);
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//        _currentCell?.OnPointerExit(eventData);
//    }
//}
