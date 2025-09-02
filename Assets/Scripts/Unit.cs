using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Unit : MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler,
    IPointerExitHandler
{
    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _heightOffset = 0.5f;

    private Cell _currentCell;
    private bool _isMoving = false;
    public event System.Action<Unit> OnMoveEndCallback;

    public void Initialize(Cell startCell)
    {
        _currentCell = startCell;
        SnapToCell(_currentCell);
    }

    public void Move(Cell targetCell)
    {
        if (!CanMoveTo(targetCell) || _isMoving) return;

        StartCoroutine(MovementRoutine(targetCell));
    }

    private IEnumerator MovementRoutine(Cell target)
    {
        _isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = GetCellPosition(target);
        float journeyTime = Vector3.Distance(startPos, endPos) / _moveSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < journeyTime)
        {
            transform.position = Vector3.Lerp(
                startPos,
                endPos,
                elapsedTime / journeyTime
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SnapToCell(target);
        _currentCell = target;
        _isMoving = false;
        OnMoveEndCallback?.Invoke(this);
    }

    private void SnapToCell(Cell cell)
    {
        transform.position = GetCellPosition(cell);
    }

    private Vector3 GetCellPosition(Cell cell)
    {
        return cell.transform.position + Vector3.up * _heightOffset;
    }

    private bool CanMoveTo(Cell target)
    {
        return target != null && target.Unit == null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _currentCell?.OnPointerEnter(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _currentCell?.OnPointerClick(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _currentCell?.OnPointerExit(eventData);
    }
}
