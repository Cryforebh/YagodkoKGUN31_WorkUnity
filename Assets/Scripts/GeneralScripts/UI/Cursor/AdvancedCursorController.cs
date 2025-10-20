using System.Collections.Generic;
using UnityEngine;

public class AdvancedCursorController : MonoBehaviour
{
    [Header("State Settings")]
    [SerializeField] private CursorState[] _cursorStates;
    [SerializeField] private EnumStatusCursor _initialState = EnumStatusCursor.Default;

    [Header("Debug")]
    [SerializeField] private bool _enableLogs = true;

    private Dictionary<EnumStatusCursor, CursorState> _stateMachine = new Dictionary<EnumStatusCursor, CursorState>();
    private EnumStatusCursor _currentState;
    private int _currentFrame;
    private float _timer;
    private bool _isLocked;

    private void Awake()
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Start()
    {
        InitializeStateMachine();
        SetCursorState(_initialState);
        //Cursor.visible = false;
    }

    private void InitializeStateMachine()
    {
        foreach (var state in _cursorStates)
        {
            if (_stateMachine.ContainsKey(state.stateType))
            {
                LogError($"Duplicate state {state.stateType}!");
                continue;
            }

            if (state.frames == null || state.frames.Length == 0)
            {
                LogError($"No frames for state {state.stateType}!");
                continue;
            }

            _stateMachine.Add(state.stateType, state);
        }
    }

    public void SetCursorState(EnumStatusCursor newState)
    {
        if (_isLocked || !_stateMachine.ContainsKey(newState)) return;

        _currentState = newState;
        _currentFrame = 0;
        _timer = 0f;
        ForceCursorUpdate();
    }

    public void LockCursor(bool locked) => _isLocked = locked;

    private void Update()
    {
        if (!_stateMachine.TryGetValue(_currentState, out CursorState state)) return;

        _timer += Time.deltaTime;
        if (_timer >= state.frameRate)
        {
            UpdateAnimationFrame(state);
            UpdateCursorTexture(state);
            _timer = 0f;
        }
    }

    private void UpdateAnimationFrame(CursorState state)
    {
        switch (state.animationType)
        {
            case CursorAnimationType.Loop:
                _currentFrame = (_currentFrame + 1) % state.frames.Length;
                break;

            case CursorAnimationType.PingPong:
                _currentFrame = (int)Mathf.PingPong(_currentFrame + 1, state.frames.Length - 1);
                break;

            case CursorAnimationType.Static:
                _currentFrame = Mathf.Clamp(_currentFrame, 0, state.frames.Length - 1);
                break;
        }
    }

    private void UpdateCursorTexture(CursorState state)
    {
        if (_currentFrame >= state.frames.Length || state.frames[_currentFrame] == null)
        {
            LogError("Invalid frame index or missing texture!");
            return;
        }

        Cursor.SetCursor(
            state.frames[_currentFrame],
            state.hotspot,
            CursorMode.Auto
        );
    }

    private void ForceCursorUpdate()
    {
        if (!_stateMachine.TryGetValue(_currentState, out CursorState state)) return;

        _currentFrame = Mathf.Clamp(_currentFrame, 0, state.frames.Length - 1);
        UpdateCursorTexture(state);
    }

    private void OnDisable()
    {
        ResetToSystemCursor();
    }

    public void ResetToSystemCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
    }

    private void LogError(string message)
    {
        if (_enableLogs) Debug.LogError($"[Cursor Controller] {message}", this);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var state in _cursorStates)
        {
            state.frameRate = Mathf.Max(0.01f, state.frameRate);
            state.hotspot.x = Mathf.Clamp(state.hotspot.x, 0, 512);
            state.hotspot.y = Mathf.Clamp(state.hotspot.y, 0, 512);
        }
    }
#endif
}

public enum CursorAnimationType { Static, Loop, PingPong }

[System.Serializable]
public class CursorState
{
    public EnumStatusCursor stateType;
    public Texture2D[] frames;
    public Vector2 hotspot = new Vector2(50, 50);
    public float frameRate = 0.1f;
    public CursorAnimationType animationType;
}