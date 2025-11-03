
using UnityEngine;
using Zenject;

public class C_SceneInstaller : MonoInstaller
{
    [SerializeField] private C_MaterialsContainer _materialsUnitContainer;
    [SerializeField] private C_PlayerController _playerController;
    [SerializeField] private C_Battlefield _battlefield;
    [SerializeField] private VoiceUnitManager _voiceUnitManager;
    [SerializeField] private BoomEffectController _boomEffectController;
    [SerializeField] private C_Setting _setting;
    //[SerializeField] private ColorPlayersManager _colorPlayersContainer;

    public override void InstallBindings()
    {
        Container.Bind<IGameData>().To<C_GameData>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<EnumGameEvent>();

        //Container.BindInstance(_colorPlayersContainer).AsSingle().NonLazy();
        Container.BindInstance(_materialsUnitContainer).AsSingle().NonLazy();
        Container.BindInstance(_playerController).AsSingle();
        Container.BindInstance(_voiceUnitManager).AsSingle();
        Container.BindInstance(_boomEffectController).AsSingle().NonLazy();
        Container.BindInstance(_setting).AsSingle().NonLazy();

        Container.Bind<AllPlayer>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<IPlayerManager>().To<C_PlayerManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Container.Bind<C_Battlefield>().FromInstance(_battlefield).AsSingle();

        Container.Bind<GameEvent>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<C_MoveCommand>().FromNewComponentOnNewGameObject().AsSingle();

        Container.Bind<ISpawner>().To<C_Spawner>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

    }
}