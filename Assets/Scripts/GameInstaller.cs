using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;


public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Player должен быть на сцене заранее
        Container.Bind<Player>()
            .FromComponentInHierarchy() // Берём существующий объект
            .AsSingle();
            //.NonLazy(); // Создаём сразу, а не при первом запросе
        // Говорим Zenject: "Когда кто-то попросит Inventory, создай новый экземпляр"
        Container.Bind<Inventory>().AsSingle();
    }
}
