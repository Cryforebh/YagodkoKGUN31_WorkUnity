using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private CellManager _cellManagerPrefab;

    public override void InstallBindings()
    {
        Container.Bind<CellManager>()
            .FromComponentInNewPrefab(_cellManagerPrefab)
            .AsSingle()
            .NonLazy();
    }
}
