using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(Unit))]
public class UnitSelectionPointer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private ContainerStatusGame _statusGame;
    [Inject] private SelectionMaterialManager _selectionPointerColors;
    [Inject] private MoveSystem _moveSystem;
    [Inject] private AllPlayer _allPlayer;
    [Inject] private PlayerManager _playerManager;

    private static UnitSelectionPointer _currentSelectedUnit; // Статическая ссылка на выбранный юнит

    private Unit _unit;
    private Material _oldMaterial;
    private MeshRenderer _meshRenderer;
    private bool _selected = false;

    private void Awake()
    {
        // Добавляем проверки
        if (_statusGame == null && _selectionPointerColors == null && _moveSystem == null)
        {
            _statusGame = FindObjectOfType<ContainerStatusGame>();
            _selectionPointerColors = FindObjectOfType<SelectionMaterialManager>();
            _moveSystem = FindObjectOfType<MoveSystem>();
            _allPlayer = FindObjectOfType<AllPlayer>();
            _playerManager = FindObjectOfType<PlayerManager>();
        }

        if (_statusGame == null)
        {
            Debug.LogError($"ContainerStatusGame не инжектирован в {gameObject.name}", this);
            enabled = false;
            return;
        }

        _unit = GetComponent<Unit>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _oldMaterial = _meshRenderer.material;

        if (_statusGame == null) Debug.Log("! Статуса еще не существует !");

        // Подписываемся на событие "изменения стадии игры"
        _statusGame.OnStatusChanged += TransferSelect;
        _statusGame.OnStatusChanged += OnGameStatusChanged;

        if (_unit == null)
        {
            Debug.LogError($"Unit не найден на {gameObject.name}", this);
        }
    }

    private void OnDestroy()
    {
        _statusGame.OnStatusChanged -= TransferSelect;
        _statusGame.OnStatusChanged -= OnGameStatusChanged;
    }

    private void OnGameStatusChanged(EnumStatusGame newStatus)
    {
        if (newStatus == EnumStatusGame.Hit)
        {
            //_moveSystem.SetUnitAction(_unit);
            Debug.Log("Данные о Вражеском Персонаже переданы...");
            Deselect(); // Автоматическое снятие выбора
        }
        if (newStatus >= EnumStatusGame.SelectedCell && _selected)
        {
            _moveSystem.SetUnitAction(_unit);
            //// Через Событие статуса игры, передаем выбранного персонажа на стадии выбора действия
            //_moveSystem.SetUnitAction(_unit);
            Debug.Log("Персонаж отвязан - была выбрана клетка.");
            Deselect(); // Автоматическое снятие выбора
        }
    }

    private void TransferSelect(EnumStatusGame newStatus)
    {
        if (newStatus == EnumStatusGame.SelectedUnit && _selected)
        {
            // Через Событие статуса игры, передаем нашего выбранного персонажа на стадии выбора персонажа
            _moveSystem.SetUnitSelected(_unit);
            Debug.Log("Данные о Персонаже переданы...");
        }
    }

    private void Focus()
    {
        if (!_selectionPointerColors) return;

        //if (_unit.GetStatusUnit == EnumStatusUnit.Enemy) return;

        if (!_selected && _statusGame.Status < EnumStatusGame.SelectedCell) // Фокус только для невыбранных юнитов
        {
            _meshRenderer.material = _selectionPointerColors.GetFocusMaterialUnit;
        }

        if (_unit.GetStatusUnit == EnumStatusUnit.Enemy)
        {
            _meshRenderer.material = _selectionPointerColors.GetFocusMaterialEnemyUnit;
        }
    }

    private void Select()
    {
        //// 1. Проверка текущего состояния игры
        //if (_statusGame.Status >= EnumStatusGame.SelectedCell)
        //    return;

        //// 2. Обработка выбора вражеского юнита
        //if (_unit.GetStatusUnit == EnumStatusUnit.Enemy)
        //{
        //    HandleEnemySelection();
        //    return;
        //}



        if (_statusGame.Status >= EnumStatusGame.SelectedCell) return;

        // ВЫбор вражеского юнита на стадии выбора персонажа
        if (_unit.GetStatusUnit == EnumStatusUnit.Enemy && _statusGame.Status == EnumStatusGame.SelectedUnit)
        {
            _statusGame.StatusUpdate(EnumStatusGame.Hit);
        }
        else if (_unit.GetStatusUnit == EnumStatusUnit.Enemy && _statusGame.Status == EnumStatusGame.SelectedCell)
        {
            _currentSelectedUnit?.Deselect();
        }
        else if (_unit.GetStatusUnit == EnumStatusUnit.Enemy && _statusGame.Status < EnumStatusGame.SelectedUnit) return;

        // Если кликнули на свой новый юнит
        if (!_selected && _unit.GetStatusUnit != EnumStatusUnit.Enemy)
        {
            // Снимаем выбор с предыдущего юнита
            _currentSelectedUnit?.Deselect();

            // Выбираем текущий
            _selected = true;

            _statusGame.StatusUpdate(EnumStatusGame.SelectedUnit);
            _meshRenderer.material = _selectionPointerColors.GetSelectMaterialUnit;
            _currentSelectedUnit = this;
        }
        // Если кликнули на уже выбранный юнит - снимаем выбор
        else
        {
            Deselect();
        }
    }

    private void Deselect()
    {
        // Если это новый юнит
        if (!_selected) return;

        _selected = false;

        // Если клетка небыла выбрана
        if (_statusGame.Status < EnumStatusGame.SelectedCell) _statusGame.StatusUpdate(EnumStatusGame.Empty);

        _meshRenderer.material = _oldMaterial;

        // Если это старый выбранный юнит
        if (_currentSelectedUnit == this)
        {
            // Очищаем переменную от значения
            _currentSelectedUnit = null;
        }
    }

    private void Exit()
    {
        // Если отводим курсор от невыбранного юнита (новый)
        if (!_selected)
        {
            _meshRenderer.material = _oldMaterial;
        }
    }

    public void OnPointerClick(PointerEventData eventData) => Select();

    public void OnPointerEnter(PointerEventData eventData) => Focus();

    public void OnPointerExit(PointerEventData eventData) => Exit();

    public bool Selected()
    {
        if (_currentSelectedUnit == this && _selected) return true;
        else return false;
    }
}




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
