using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR

/// <summary>
/// Окно читов
/// </summary>
public class EditorCheckersCheatWindow : EditorWindow
{
    //Контролс "OnlyEditor"
    private EditorControls _controls;
    private bool _isPlayMode = false;


    [MenuItem("WoodenSamurai/Windows/Cheats/EditorCheckersCheatWindow", priority = 1)]
    public static void ShowExample()
        => GetWindow<EditorCheckersCheatWindow>(false, "Editor Checkers Cheat Window ", true);

    //Вызывается при переходе в плеймод и при открытии редактора
    private void OnEnable()
    {
        if (EditorApplication.isPlaying) ProccesAddControl(true);
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    //Вызывается при выходе из плеймода и при закрытии редактора
    private void OnDisable()
    {
        ProccesAddControl(false);
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            //Вход в плеймод (происходит после выхода из EditMode)
            case PlayModeStateChange.EnteredPlayMode:
                //На время игры создаем контролс и подписываемся на его экшены
                _isPlayMode = true;
                ProccesAddControl(_isPlayMode);
                break;
            //Выход из плеймода (происходит перед входом в EditMode)
            case PlayModeStateChange.ExitingPlayMode:
                //Для простоты, чтобы не париться с проверками - при выходе очищаем память
                _isPlayMode = false;
                ProccesAddControl(_isPlayMode);
                break;
            //Вход в режим редактора
            case PlayModeStateChange.EnteredEditMode:
            //Выход из режима редактора
            case PlayModeStateChange.ExitingEditMode:
                break;
        }
    }

    public void OnGUI()
    {
        //position = new Rect(100, 100, 800, 250); // x, y — координаты на экране, width, height — ширина и высота окна
        //var window = GetWindow<EditorCheatWindow>("Editor Checkers Cheat Window");
        minSize = new Vector2(600, 190); // Задаём минимальный размер окна

        GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
        boldStyle.fontStyle = FontStyle.Bold;

        GUILayout.Space(5);
        GUILayout.Label("== ЧИТЫ ДЛЯ ШАШЕК ==", boldStyle, GUILayout.ExpandWidth(true));

        GUILayout.Space(5);
        GUILayout.Label("Работает только в открытом состоянии. Для работы читов, не закрывайте окно!", GUILayout.ExpandWidth(true));

        GUILayout.Space(10);
        GUILayout.Label("Клавиши для изменения статуса игры:", boldStyle, GUILayout.ExpandWidth(true));

        GUILayout.Space(5);
        GUILayout.Label("- 1 : Меняет игрока, который должен ходить.", GUILayout.ExpandWidth(true));

        GUILayout.Space(10);
        GUILayout.Label("Клавиши для взаимодействия с персонажами (Они не должны быть выбранными!):", boldStyle, GUILayout.ExpandWidth(true));

        GUILayout.Space(5);
        GUILayout.Label("- 2 : Уничтожает вражеского персонажа, на которого наведён курсор мыши.", GUILayout.ExpandWidth(true));

        GUILayout.Space(5);
        GUILayout.Label("- 3 : Улучшает дружеского персонажа, на которого наведён курсор мыши.", GUILayout.ExpandWidth(true));
    }

    private void ProccesAddControl(bool onEnable)
    {
        if (onEnable)
        {
            _controls = new EditorControls();
            _controls.CheatMenu.Enable();
            _controls.CheatMenu.NextTurn.performed += OnNextTurnPerformed;
            _controls.CheatMenu.Kill.performed += OnKillPerformed;
            _controls.CheatMenu.MaxUpgradeUnit.performed += OnMaxUpgradeUnitPerformed;
        }
        else
        {
            if (_controls == null) return;
            _controls.Disable();
            _controls.CheatMenu.NextTurn.performed -= OnNextTurnPerformed;
            _controls.CheatMenu.Kill.performed -= OnKillPerformed;
            _controls.CheatMenu.MaxUpgradeUnit.performed -= OnMaxUpgradeUnitPerformed;
            _controls.Dispose();
            _controls = null;
        }
    }

    //Чит на лучшие характеристики персонажа в центре курсора мыши (Делает нашу пешку Дамкой)
    private void OnMaxUpgradeUnitPerformed(InputAction.CallbackContext obj)
    {
        if (!FindEnemyUnit(out var unit)) return;

        if (!unit.IsEnemy && !unit.IsDamka)
        {
            unit.IsDamka = true;
            unit.UpdateMeshRenderDamka();
        }
    }

    //Чит на смену хода
    private void OnNextTurnPerformed(InputAction.CallbackContext obj)
    {
        if (!FindGameTurn(out var controller))
            return;

        controller.ForcedMoveUnit(EnumGameEvent.EndMove);
    }

    //Чит на убийство юнита, который находится в центре курсора мыши (не нашего, а противника)
    private void OnKillPerformed(InputAction.CallbackContext obj)
    {
        if (!FindEnemyUnit(out var unit)) return;

        if (unit.IsEnemy) unit.Death();
    }

    private bool FindGameTurn(out C_PlayerController controllerOut)
    {
        var controller = FindObjectOfType<C_PlayerController>();
        if (controller == null)
        {
            Debug.LogWarning($"Тип <b>{nameof(C_PlayerController)}</b> - не найден! Использование команд допустимо только на игровом поле для Шашек!");
            controllerOut = default;
            return false;
        }
        controllerOut = controller;
        return true;
    }

    private bool FindEnemyUnit(out C_Unit unit)
    {
        var battl = FindObjectOfType<C_Battlefield>();
        if (battl == null)
        {
            Debug.LogWarning($"Тип <b>{nameof(C_Battlefield)}</b> - не найден! Использование команд допустимо только на игровом поле для Шашек!");
            unit = default;
            return false;
        }

        var type = battl.GetType();
        var field = type.GetField("_unitEnter", BindingFlags.Instance | BindingFlags.NonPublic);

        unit = field.GetValue(battl) as C_Unit;

        if (unit == null)
        {
            Debug.LogWarning($"Чит не сработал - Вражеский персонаж не находится в центре курсора или является выбраным!");
            return false;
        }

        return true;
    }
}
#endif