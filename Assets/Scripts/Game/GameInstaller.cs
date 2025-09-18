using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private CellManager _cellManagerPrefab;
    [SerializeField] private Board _board;

    public override void InstallBindings()
    {
        Container.Bind<Board>()
            .FromInstance(_board)
            .AsSingle()
            .NonLazy();
        Container.Bind<CellManager>()
            .FromInstance(_cellManagerPrefab)
            .AsSingle()
            .NonLazy();
    }
}
