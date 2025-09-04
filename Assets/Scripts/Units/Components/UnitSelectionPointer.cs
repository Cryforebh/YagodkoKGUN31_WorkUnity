using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class UnitSelectionPointer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Inject] private ContainerStatusGame _statusGame;
    [Inject] private SelectionManager _selectionPointerColors;

    private static UnitSelectionPointer _currentSelectedUnit; // Статическая ссылка на выбранный юнит

    private Material _oldMaterial;
    private MeshRenderer _meshRenderer;
    private bool _selected = false;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _oldMaterial = _meshRenderer.material;

        // Подписываемся на событие "изменения стадии игры"
        _statusGame.OnStatusChanged += OnGameStatusChanged;
    }

    private void OnDestroy()
    {
        // Важно: отписываемся при уничтожении
        _statusGame.OnStatusChanged -= OnGameStatusChanged;
    }

    private void OnGameStatusChanged(EnumStatusGame newStatus)
    {
        if (newStatus >= EnumStatusGame.SelectedCell && _selected)
        {
            Deselect(); // Автоматическое снятие выбора
        }
    }

    private void Focus()
    {
        if (!_selected && _statusGame.Status < EnumStatusGame.SelectedCell) // Фокус только для невыбранных юнитов
        {
            _meshRenderer.material = _selectionPointerColors.GetFocusMaterialUnit;
        }
    }

    private void Select()
    {
        if (_statusGame.Status >= EnumStatusGame.SelectedCell) return;

        // Если кликнули на новый юнит
        if (!_selected)
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
