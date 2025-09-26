using UnityEngine;
using Zenject;

public class SpawnUnitOnCell : MonoBehaviour
{
    [Inject] private Board _board;
    [Inject] private AllCell _allCell;
    [Inject] private AllPlayer _player;
    [Inject] private PlayerManager _playerManager;
    [Inject] private TypeGameManager _gameManager;
    [Inject] private MoveSystem _moveSystem;
    //[Inject] private UnitManager _unitManager;
    [SerializeField] private Unit _unit;
    
    private Unit _unitPrefab;
    private Camera _cameraPlayerOne;
    private Camera _cameraPlayerTwo;

    [SerializeField] private EnumTypeGame _typeGame;
    [SerializeField] private EnumPlayers _playerGoesFirst;

    private void Awake()
    {
        _cameraPlayerOne = _playerManager.GetCameraPlayer(EnumPlayers.PlayerOne);
        _cameraPlayerTwo = _playerManager.GetCameraPlayer(EnumPlayers.PlayerTwo);

        // Как только создано будет меню с выбором этих элементов - убрать !!!
        SetTypeGame(_typeGame);
        //SetWhoGoesFirst(_playerGoesFirst);
        //

        TypeGameCreate();
    }

    private void Start()
    {


        //CreateUnits();
        ////Debug.Log($"Создан {cell.CurrentUnit.name}, у него {cell.CurrentUnit.GetHealth} здоровья и {cell.CurrentUnit.GetDamage} урона.\n" +
        ////    $"Это {cell.CurrentUnit.GetStatusUnit}!");


    }

    public void CreateUnits()
    {
        foreach (var cell in _allCell.Cells)
        {
            if (cell.CreateUnit)
            {
                cell.ResetAll();
                
                Unit newUnit;

                // Создаём Юнита в кординатах клетки
                /* - Префаб Юнита устанавливается в самой Клетке если он есть */
                /* - Если не установлен - то создается Юнит из SpawnUnitOnCell */
                if (cell.InstalledStartUnit == null) newUnit = Instantiate(_unit, cell.transform.position + Vector3.up /** 2*/, cell.transform.rotation);
                else
                {
                    _unitPrefab = cell.InstalledStartUnit;
                    newUnit = Instantiate(_unitPrefab, cell.transform.position + Vector3.up /** 2*/, cell.transform.rotation);
                }

                // Присваиваем юниту владельца (игрока из клетки)
                newUnit.SetPlayer(cell.Player);

                // Поворачиваем согласно стороны владельца
                if (cell.Player == EnumPlayers.PlayerTwo)
                    newUnit.transform.Rotate(0f, 180f, 0f);

                // Добавляем юнита в коллекцию игрока (в список, который принадлежит Player'у указанного в Cell)
                _player.AddUnitOnPlayer(cell.Player, newUnit);

                // Установка статуса при создании
                bool isMy = cell.Player == _playerManager.ActivePlayer;
                newUnit.SetStatusUnit(isMy ? EnumStatusUnitEnemy.My : EnumStatusUnitEnemy.Enemy);

                // Востанавливаем активность дочерних коллекций внутри юнита
                newUnit.gameObject.SetActive(true);
                if (newUnit.gameObject.GetComponent<Ranger>()) newUnit.gameObject.GetComponent<Ranger>().enabled = true;
                if (newUnit.gameObject.GetComponent<Samurai>()) newUnit.gameObject.GetComponent<Samurai>().enabled = true;

                // Привязываем юнита к клетке
                cell.SetUnit(newUnit); 
            }
        }
        
    }

    private void TypeGameCreate()
    {
        if (_typeGame == EnumTypeGame.PVP)
        {
            _cameraPlayerTwo.gameObject.SetActive(false);
            _cameraPlayerOne.gameObject.SetActive(true);
        }
        if (_typeGame == EnumTypeGame.PVE)
        {
            _board.transform.Rotate(0f, 0f, 90f);
            _cameraPlayerTwo.gameObject.SetActive(false);
            _cameraPlayerOne.gameObject.SetActive(true);
        }
        if (_typeGame == EnumTypeGame.PVPLocal)
        {
            _board.transform.Rotate(0f, 0f, 90f);
            _cameraPlayerTwo.gameObject.SetActive(true);
            _cameraPlayerOne.gameObject.SetActive(true);
        }
    }

    public void SetTypeGame(EnumTypeGame typeGame)
    {
        _typeGame = typeGame;
        _gameManager.SetTypeGame(_typeGame);
        Debug.Log($"Тип игры назначен - это {typeGame}!");
    }
    public void SetWhoGoesFirst(EnumPlayers player)
    {
        _playerGoesFirst = player;
        _playerManager.ActivePlayer = _playerGoesFirst;
        Debug.Log($"Стартовый Игрок назначен - это {player}!");
    }

    public void Respawn()
    {
        foreach (var cell in _allCell.Cells)
        {
            var unit = cell.CurrentUnit;
            if (unit != null)
            {
                if (_player.PlayersCollection.Count != 0) _player.RemoveUnitOnPlayer(cell.Player, unit);
                cell.ClearUnit();
                unit.OnDelete();
            }
        }

        CreateUnits();
    }
}
