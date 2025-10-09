using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private CellManager _cellManagerPrefab;
    [SerializeField] private Board _board;
    [SerializeField] private GameData _gameData;
    [SerializeField] private SettingObjectManager _settingObjectManager;

    public override void InstallBindings()
    {
        Container.BindInstance(_settingObjectManager).AsSingle().NonLazy();
        Container.Bind<Board>()
            .FromInstance(_board)
            .AsSingle()
            .NonLazy();
        Container.Bind<CellManager>()
            .FromInstance(_cellManagerPrefab)
            .AsSingle()
            .NonLazy();
        Container.BindInstance(_gameData).AsSingle();
    }
}
