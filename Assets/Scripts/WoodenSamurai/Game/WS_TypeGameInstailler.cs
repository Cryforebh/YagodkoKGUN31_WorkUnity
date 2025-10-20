using UnityEngine;
using Zenject;

public class WS_TypeGameInstailler : MonoInstaller
{
    [SerializeField] private WS_TypeGameManager _typeGameManager;

    public override void InstallBindings()
    {
        Container.Bind<WS_TypeGameManager>()
            .FromInstance(_typeGameManager)
            .AsSingle()
            .NonLazy();
    }
}
