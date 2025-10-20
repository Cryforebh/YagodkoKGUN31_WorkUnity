using UnityEngine;
using Zenject;

public class WS_GameInstaller : MonoInstaller
{
    [SerializeField] private WS_CellManager _cellManagerPrefab;
    [SerializeField] private WS_Board _board;
    [SerializeField] private WS_GameData _gameData;
    [SerializeField] private WS_SettingObjectManager _settingObjectManager;

    public override void InstallBindings()
    {
        Container.Bind<AllPlayer>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<IPlayerManager>().To<WS_PlayerManager>().FromNewComponentOnNewGameObject().AsSingle();
        Container.BindInstance(_settingObjectManager).AsSingle().NonLazy();
        Container.Bind<WS_Board>()
            .FromInstance(_board)
            .AsSingle()
            .NonLazy();
        Container.Bind<WS_CellManager>()
            .FromInstance(_cellManagerPrefab)
            .AsSingle()
            .NonLazy();
        Container.BindInstance(_gameData).AsSingle();
    }
}
