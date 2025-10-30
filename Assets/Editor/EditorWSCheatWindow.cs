using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorWSCheatWindow : EditorWindow
{
    private EditorControls _controls;
    private bool _isPlayMode = false;

    [MenuItem("Cheats/Windows/EditorWSCheatWindow", priority = 1)]
    public static void ShowExample()
        => GetWindow<EditorWSCheatWindow>(false, "Editor WoodenSamurai Cheat Window ", true);

    private void OnEnable()
    {
        if (EditorApplication.isPlaying) ProccesAddControl(true);
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        ProccesAddControl(false);
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredPlayMode:
                _isPlayMode = true;
                ProccesAddControl(_isPlayMode);
                break;
            case PlayModeStateChange.ExitingPlayMode:
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
        minSize = new Vector2(650, 210);

        GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
        boldStyle.fontStyle = FontStyle.Bold;

        GUILayout.Space(5);
        GUILayout.Label("== ЧИТЫ ДЛЯ ДЕРЕВЯНЫХ САМУРАЕВ ==", boldStyle, GUILayout.ExpandWidth(true));

        GUILayout.Space(5);
        GUILayout.Label("Работает только в открытом состоянии. Для работы читов, не закрывайте окно!", GUILayout.ExpandWidth(true));
        GUILayout.Label("Имеет свои проблеммы из-за плохого кода. Если убьете всех юнитов врага через чит, смените ход дважды.", GUILayout.ExpandWidth(true));

        GUILayout.Space(10);
        GUILayout.Label("Клавиши для изменения статуса игры:", boldStyle, GUILayout.ExpandWidth(true));

        GUILayout.Space(5);
        GUILayout.Label("- 1 : Меняет игрока, который должен ходить.", GUILayout.ExpandWidth(true));

        GUILayout.Space(10);
        GUILayout.Label("Клавиши для взаимодействия с персонажами:", boldStyle, GUILayout.ExpandWidth(true));

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

        unit.LevelUp(1);
        unit.LevelUp(1);
        unit.LevelUp(1);
        unit.LevelUp(1);
        unit.LevelUp(1);
    }

    //Чит на смену хода
    private void OnNextTurnPerformed(InputAction.CallbackContext obj)
    {
        if (!FindGameTurn(out var controller))
            return;

        controller.CheckWinner();
        controller.ChangePlayer();
    }

    //Чит на убийство юнита, который находится в центре курсора мыши (не нашего, а противника)
    private void OnKillPerformed(InputAction.CallbackContext obj)
    {
        if (!FindEnemyUnit(out var unit)) return;

        if (unit.IsEnemy) unit.Death();
    }

    private bool FindGameTurn(out WS_PlayerManager controllerOut)
    {
        var controller = FindObjectOfType<WS_PlayerManager>();
        if (controller == null)
        {
            Debug.LogWarning($"Тип <b>{nameof(WS_PlayerManager)}</b> - не найден! Использование команд допустимо только на игровом поле для Деревянных Самураев!");
            controllerOut = default;
            return false;
        }
        else if (controller.ActivePlayer == EnumPlayers.None)
        {
            Debug.LogWarning($"Чит не сработал - <b>Стартовый Игрок</b> еще не назначен!");
            controllerOut = default;
            return false;
        }

        controllerOut = controller;
        return true;
    }

    private bool FindEnemyUnit(out WS_Unit unit)
    {
        var data = FindObjectOfType<WS_GameData>();
        if (data == null)
        {
            Debug.LogWarning($"Тип <b>{nameof(WS_GameData)}</b> - не найден! Использование команд допустимо только на игровом поле для Деревянных Самураев!");
            unit = default;
            return false;
        }

        unit = data.TargetUnitEnter;

        if (unit == null)
        {
            Debug.LogWarning($"Чит не сработал - Вражеский персонаж не находится в центре курсора или является выбраным!");
            return false;
        }

        return true;
    }
}
