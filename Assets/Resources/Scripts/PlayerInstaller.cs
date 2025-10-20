using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private ColorPlayersManager playerManager;

    public override void InstallBindings()
    {
        //Container.Bind<ColorPlayersManager>().FromComponentInNewPrefab(playerManager).AsSingle().NonLazy();
        Container.Bind<ColorPlayersManager>().FromInstance(playerManager).AsSingle().NonLazy();
        //Container.Bind<AllPlayer>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
}
