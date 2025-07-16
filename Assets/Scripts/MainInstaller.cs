using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;


public class MainInstaller : MonoInstaller
{
    [SerializeField] private SceneController _sceneController;

    public override void InstallBindings()
    {
        // Привязываем существующий экземпляр на этом же GameObject
        Container.Bind<SceneController>()
            .FromInstance(_sceneController)
            .AsSingle();
    }
}
