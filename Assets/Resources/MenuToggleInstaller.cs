using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;

public class MenuToggleInstaller : MonoInstaller
{

    public override void InstallBindings()
    {
        Container.Bind<MenuToggleManager>()
            .FromNewComponentOnNewGameObject() // Создаем новый GameObject
            .AsSingle()                        // Единственный экземпляр
            .NonLazy();                        // Создаем сразу, а не при первом обращении
    }
}

