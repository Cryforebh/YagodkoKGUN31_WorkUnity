using UnityEngine;
using Zenject;

public class MenuInGame : MonoBehaviour/*, IPointerEnterHandler, IPointerExitHandler*/
{
    //[Inject] private AdvancedCursorController _cursor;
    //[Inject] private SpawnUnitOnCell _spawn;
    [Inject] private InputLevelManager _levelManager;
    [Inject] private SceneController _sceneController;

    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _manual;
    [SerializeField] private GameObject _manualUnits;
    [SerializeField] private GameObject _manualModifi;
    [SerializeField] private GameObject _manualEat;
    [SerializeField] private GameObject _seting;


    //public void SelectedPlayerOne()
    //{
    //    //_spawn.SetWhoGoesFirst(EnumPlayers.PlayerOne);
    //    //OnDisable();
    //}

    //public void SelectedPlayerTwo()
    //{
    //    //_spawn.SetWhoGoesFirst(EnumPlayers.PlayerTwo);
    //    //OnDisable();
    //}

    private void Awake()
    {
        _menu.SetActive(false);
        _manual.SetActive(false);
        _manualUnits.SetActive(false);
        _manualModifi.SetActive(false);
        _manualEat.SetActive(false);
        _seting.SetActive(false);
    }

    private void Start()
    {
        //_levelManager.Contlols.Game.Menu.started += _ => MenuShow();
        _levelManager.Contlols.Game.Menu.canceled += _ => MenuShow();
    }

    //public void GoExit()
    //{

    //}

    public void MenuHide()
    {
        _menu.SetActive(false);
    }

    public void MenuShow()
    {
        _manual.SetActive(false);
        _manualUnits.SetActive(false);
        _manualModifi.SetActive(false);
        _manualEat.SetActive(false);
        _seting.SetActive(false);

        if (_menu.activeSelf == false) _menu.SetActive(true);
        else _menu.SetActive(false);
    }

    public void MenuRestart()
    {
        _sceneController.RestartScene();
    }

    public void MenuExit()
    {
        Application.Quit();
    }

    public void MenuManualShow()
    {
        _menu.SetActive(false);
        _manual.SetActive(true);
        _manualUnits.SetActive(true);
    }

    public void MenuManualLeft()
    {
        if (_manualUnits.activeSelf == true)
        {
            _manualUnits.SetActive(false);
            _manualEat.SetActive(true);
            return;
        }
        else if (_manualEat.activeSelf == true)
        {
            _manualEat.SetActive(false);
            _manualModifi.SetActive(true);
            return;
        }
        else if (_manualModifi.activeSelf == true)
        {
            _manualModifi.SetActive(false);
            _manualUnits.SetActive(true);
            return;
        }
    }

    public void MenuManualRight()
    {
        if (_manualUnits.activeSelf == true)
        {
            _manualUnits.SetActive(false);
            _manualModifi.SetActive(true);
            return;
        }
        else if (_manualModifi.activeSelf == true)
        {
            _manualModifi.SetActive(false);
            _manualEat.SetActive(true);
            return;
        }
        else if (_manualEat.activeSelf == true)
        {
            _manualEat.SetActive(false);
            _manualUnits.SetActive(true);
            return;
        }
    }

    public void MenuSettingShow()
    {
        _menu.SetActive(false);
        _seting.SetActive(true);
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    _cursor.SetCursorState(EnumStatusCursor.Select);
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    _cursor.SetCursorState(EnumStatusCursor.Default);
    //}
}
