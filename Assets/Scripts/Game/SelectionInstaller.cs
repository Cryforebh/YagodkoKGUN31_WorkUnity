using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SelectionInstaller : MonoInstaller
{
    [SerializeField]
    private SelectionMaterialManager _selectionPointerColors;
    [SerializeField]
    private AllCell _allCell;
    [SerializeField]
    private SpawnUnitOnCell _spawnUnitOnCell;
    [SerializeField]
    private AllPlayer _player;
    [SerializeField]
    private PlayerManager _playerManager;
    [SerializeField]
    private HealthBarManager _healthBarManager;
    [SerializeField]
    private AdvancedCursorController _cursor;

    public override void InstallBindings()
    {
        Container.Bind<AdvancedCursorController>().FromInstance(_cursor).AsSingle().NonLazy();
        Container.Bind<HealthBarManager>().FromInstance(_healthBarManager).AsSingle().NonLazy();
        Container.Bind<SelectionMaterialManager>().FromInstance(_selectionPointerColors).AsSingle().NonLazy();
        Container.Bind<AllCell>().FromInstance(_allCell).AsSingle();
        Container.Bind<SpawnUnitOnCell>().FromInstance(_spawnUnitOnCell).AsSingle();
        Container.Bind<AllPlayer>().FromInstance(_player).AsSingle();
        Container.Bind<PlayerManager>().FromInstance(_playerManager).AsSingle();

    }
}
