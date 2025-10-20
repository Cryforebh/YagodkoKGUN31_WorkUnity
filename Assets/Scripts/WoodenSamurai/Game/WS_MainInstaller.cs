using UnityEngine;
using Zenject;

public class WS_MainInstaller : MonoInstaller
{
    [SerializeField] private SceneController _sceneController;
    [SerializeField] private WS_ContainerStatusGame _containerStatusGame;
    [SerializeField] private WS_MoveSystem _moveSystem;
    //[SerializeField] private SoundManager _soundManager;

    public override void InstallBindings()
    {
        // Привязываем существующий экземпляр на этом же GameObject
        Container.Bind<SceneController>()
            .FromInstance(_sceneController)
            .AsSingle();
        Container.Bind<InputManager>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<WS_ContainerStatusGame>().FromInstance(_containerStatusGame).AsSingle().NonLazy();
        Container.Bind<WS_MoveSystem>().FromInstance(_moveSystem).AsSingle().NonLazy();
        //Container.Bind<SoundManager>().FromInstance(_soundManager).AsSingle().NonLazy();
    }
}
