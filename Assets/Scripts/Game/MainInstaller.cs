using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private SceneController _sceneController;
    [SerializeField] private LevelRestartPanel _restartPanel;
    [SerializeField] private InputLevelManager _inputManager;
    [SerializeField] private ContainerStatusGame _containerStatusGame;
    [SerializeField] private MoveSystem _moveSystem;
    //[SerializeField] private SoundManager _soundManager;

    public override void InstallBindings()
    {
        // Привязываем существующий экземпляр на этом же GameObject
        Container.Bind<SceneController>()
            .FromInstance(_sceneController)
            .AsSingle();

        Container.Bind<LevelRestartPanel>().FromInstance(_restartPanel).AsSingle();
        Container.Bind<InputLevelManager>().FromInstance(_inputManager).AsSingle().NonLazy();
        Container.Bind<ContainerStatusGame>().FromInstance(_containerStatusGame).AsSingle().NonLazy();
        Container.Bind<MoveSystem>().FromInstance(_moveSystem).AsSingle().NonLazy();
        //Container.Bind<SoundManager>().FromInstance(_soundManager).AsSingle().NonLazy();
    }
}
