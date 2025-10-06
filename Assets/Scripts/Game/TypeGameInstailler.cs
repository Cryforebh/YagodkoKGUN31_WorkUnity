using UnityEngine;
using Zenject;

public class TypeGameInstailler : MonoInstaller
{
    [SerializeField] private TypeGameManager _typeGameManager;

    public override void InstallBindings()
    {
        Container.Bind<TypeGameManager>()
            .FromInstance(_typeGameManager)
            .AsSingle()
            .NonLazy();
    }
}
