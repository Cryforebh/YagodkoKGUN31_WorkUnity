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

    public override void InstallBindings()
    {
        // Привязываем существующий экземпляр на этом же GameObject
        Container.Bind<SceneController>()
            .FromInstance(_sceneController)
            .AsSingle();

        Container.Bind<LevelRestartPanel>().FromInstance(_restartPanel).AsSingle();
        Container.Bind<InputLevelManager>().FromInstance(_inputManager).AsSingle();
        Container.Bind<ContainerStatusGame>().FromInstance(_containerStatusGame).AsSingle();
    }
}
