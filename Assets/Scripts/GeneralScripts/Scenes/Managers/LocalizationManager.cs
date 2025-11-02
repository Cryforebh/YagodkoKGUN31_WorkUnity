using System;
using System.Collections.Generic;
using UnityEngine;


public class LocalizationManager : MonoBehaviour
{
    private LocalizationData translations = new LocalizationData();

    private string _language = "en";
    private int _currentCountLanguage = 0;

    private Dictionary<string, string> _translationsTest = new Dictionary<string, string>();

    public int GetCurrentCountLanguage => _currentCountLanguage;
    public event Action<int> ChangeLanguageEvent;

    private void Awake()
    {
        Language(_currentCountLanguage);
    }

    public void Language(int count)
    {
        switch (count)
        {
            case 1:
                _language = "ru";
                break;
            default:
                _language = "en";
                break;
        }
        _currentCountLanguage = count;
        LoadTranslations(_language);
        //LoadTranslationsTest("test");
        ChangeLanguageEvent?.Invoke(_currentCountLanguage);
    }

    public void LoadTranslations(string language)
    {

        // Загружаем текст из ресурса
        TextAsset textAsset = Resources.Load<TextAsset>($"Language/{language}");


        if (textAsset != null)
        {
            // Получаем строку JSON из TextAsset
            string json = textAsset.text;

            // Преобразуем JSON в объект C#
            LocalizationData localizationData = JsonUtility.FromJson<LocalizationData>(json);

            // Заполняем словарь переводов
            translations = localizationData;
        }
        else
        {
            Debug.LogError("Файл переводов не найден!");
        }
    }

    /// <summary>
    /// Не работает, из за ограничений в JsonUtility с Dictionary...
    /// </summary>
    /// <param name="language"></param>
    private void LoadTranslationsTest(string language)
    {
        // Загружаем текст из ресурса
        TextAsset textAsset = Resources.Load<TextAsset>($"Language/{language}");

        if (textAsset != null)
        {
            // Получаем строку JSON из TextAsset
            string json = textAsset.text;

            // Преобразуем JSON в объект C#
            _translationsTest = JsonUtility.FromJson<Dictionary<string, string>>(json);

            if (_translationsTest.Count > 0)
            {
                foreach (var item in _translationsTest)
                {
                    Debug.LogWarning($"{item.Value}");
                }
            }
            else Debug.LogWarning($"В {_translationsTest} - {_translationsTest.Count} значений.");
        }
        else
        {
            Debug.LogError("Файл переводов не найден!");
        }
    }

    /// <summary>
    /// Не работает, из за ограничений в JsonUtility с Dictionary...
    /// </summary>
    /// <param name="language"></param>
    public string GetTextTest(string key)
    {
        if (_translationsTest.ContainsKey(key))
        {
            Debug.LogWarning($"{_translationsTest[key]} - test");
            return _translationsTest[key];
        }
        else
        {
            Debug.LogWarning($"{_translationsTest.Count} - test");

            return key; // Возвращаем ключ, если перевод не найден
        }
    }

    public string GetText(EnumTextLocalization Localization)
    {
        if (translations == null)
        {
            Debug.LogError("Словарь переводов не инициализирован!");
            return "Словарь переводов не инициализирован!";
        }
        else
        {
            switch (Localization)
            {
                case EnumTextLocalization.None:
                    return "TextNull";

                case EnumTextLocalization.welcome_message:
                    return translations.welcome_message;
                case EnumTextLocalization.menu_start_button:
                    return translations.menu_start_button;
                case EnumTextLocalization.menu_setting_button:
                    return translations.menu_setting_button;
                case EnumTextLocalization.menu_exit_button:
                    return translations.menu_exit_button;
                case EnumTextLocalization.menu_restart_button:
                    return translations.menu_restart_button;
                case EnumTextLocalization.menu_manual_button:
                    return translations.menu_manual_button;
                case EnumTextLocalization.menu_mainmenu_button:
                    return translations.menu_mainmenu_button;
                case EnumTextLocalization.menu_continue_button:
                    return translations.menu_continue_button;
                case EnumTextLocalization.menu_back_button:
                    return translations.menu_back_button;
                case EnumTextLocalization.setting_quality:
                    return translations.setting_quality;
                case EnumTextLocalization.setting_sounds:
                    return translations.setting_sounds;
                case EnumTextLocalization.setting_game:
                    return translations.setting_game;
                case EnumTextLocalization.setting_language:
                    return translations.setting_language;
                case EnumTextLocalization.quality_verylow:
                    return translations.quality_verylow;
                case EnumTextLocalization.quality_low:
                    return translations.quality_low;
                case EnumTextLocalization.quality_medium:
                    return translations.quality_medium;
                case EnumTextLocalization.quality_high:
                    return translations.quality_high;
                case EnumTextLocalization.quality_veryhigh:
                    return translations.quality_veryhigh;
                case EnumTextLocalization.quality_ultra:
                    return translations.quality_ultra;
                case EnumTextLocalization.sounds_units:
                    return translations.sounds_units;
                case EnumTextLocalization.sounds_modifications:
                    return translations.sounds_modifications;
                case EnumTextLocalization.sounds_ambient:
                    return translations.sounds_ambient;
                case EnumTextLocalization.sounds_other:
                    return translations.sounds_other;
                case EnumTextLocalization.game_dynamiccamera:
                    return translations.game_dynamiccamera;
                case EnumTextLocalization.game_madnessmode:
                    return translations.game_madnessmode;
                case EnumTextLocalization.game_immersiveobject:
                    return translations.game_immersiveobject;

                case EnumTextLocalization.sushi:
                    return translations.sushi;
                case EnumTextLocalization.sushi_info:
                    return translations.sushi_info;
                case EnumTextLocalization.sushi_infotwo:
                    return translations.sushi_infotwo;
                case EnumTextLocalization.sake:
                    return translations.sake;
                case EnumTextLocalization.sake_info:
                    return translations.sake_info;

                case EnumTextLocalization.win_menu:
                    return translations.win_menu;
                case EnumTextLocalization.win_restart:
                    return translations.win_restart;

                case EnumTextLocalization.ws_samurai:
                    return translations.ws_samurai;
                case EnumTextLocalization.ws_ranger:
                    return translations.ws_ranger;

                case EnumTextLocalization.ws_health:
                    return translations.ws_health;
                case EnumTextLocalization.ws_damage:
                    return translations.ws_damage;
                case EnumTextLocalization.ws_attackrangesamurai:
                    return translations.ws_attackrangesamurai;
                case EnumTextLocalization.ws_attackrangeranger:
                    return translations.ws_attackrangeranger;
                case EnumTextLocalization.ws_moverange:
                    return translations.ws_moverange;

                case EnumTextLocalization.ws_modifi_damage:
                    return translations.ws_modifi_damage;
                case EnumTextLocalization.ws_modifi_attackdamage:
                    return translations.ws_modifi_attackdamage;
                case EnumTextLocalization.ws_modifi_resistance:
                    return translations.ws_modifi_resistance;

                case EnumTextLocalization.ws_eat_info_onetry:
                    return translations.ws_eat_info_onetry;
                case EnumTextLocalization.ws_eat_info_twotry:
                    return translations.ws_eat_info_twotry;

                case EnumTextLocalization.player_one_name:
                    return translations.player_one_name;
                case EnumTextLocalization.player_two_name:
                    return translations.player_two_name;
                case EnumTextLocalization.game_turn:
                    return translations.game_turn;

                case EnumTextLocalization.ui_color_red:
                    return translations.ui_color_red;
                case EnumTextLocalization.ui_color_blue:
                    return translations.ui_color_blue;
                case EnumTextLocalization.ui_color_white:
                    return translations.ui_color_white;
                case EnumTextLocalization.ui_color_black:
                    return translations.ui_color_black;
                case EnumTextLocalization.ui_color_orange:
                    return translations.ui_color_orange;
                case EnumTextLocalization.ui_color_green:
                    return translations.ui_color_green;
                case EnumTextLocalization.ui_color_cyan:
                    return translations.ui_color_cyan;
                case EnumTextLocalization.ui_color_violet:
                    return translations.ui_color_violet;

                case EnumTextLocalization.win:
                    return translations.win;

                case EnumTextLocalization.selectgame:
                    return translations.selectgame;
                case EnumTextLocalization.selectgame_ws:
                    return translations.selectgame_ws;
                case EnumTextLocalization.selectgame_checkers:
                    return translations.selectgame_checkers;

                case EnumTextLocalization.play_whogoesfirst:
                    return translations.play_whogoesfirst;

                case EnumTextLocalization.menu_color_playes:
                    return translations.menu_color_playes;
                case EnumTextLocalization.menu_color_playerone:
                    return translations.menu_color_playerone;
                case EnumTextLocalization.menu_color_playertwo:
                    return translations.menu_color_playertwo;

                case EnumTextLocalization.draw:
                    return translations.draw;

                default:
                    return "TextNull";
            }
        }
    }
}
