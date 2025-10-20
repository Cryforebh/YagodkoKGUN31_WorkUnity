using UnityEngine;
using Zenject;

public class WS_SelectionInstaller : MonoInstaller
{
    [SerializeField]
    private WS_SelectionMaterialManager _selectionPointerColors;
    [SerializeField]
    private WS_AllCell _allCell;
    [SerializeField]
    private WS_SpawnUnitOnCell _spawnUnitOnCell;
    //[SerializeField]
    //private AllPlayer _player;
    //[SerializeField]
    //private WS_PlayerManager _playerManager;
    [SerializeField]
    private WS_StatisticsUnitsVisualManager _healthBarManager;
    [SerializeField]
    private AdvancedCursorController _cursor;
    [SerializeField]
    private WS_SoundsUnit _soundsUnitManager;
    [SerializeField]
    private BoomEffectController _explosionController;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<StatusGameSignal>();

        Container.Bind<AdvancedCursorController>().FromInstance(_cursor).AsSingle().NonLazy();
        Container.Bind<WS_StatisticsUnitsVisualManager>().FromInstance(_healthBarManager).AsSingle().NonLazy();
        Container.Bind<WS_SelectionMaterialManager>().FromInstance(_selectionPointerColors).AsSingle().NonLazy();
        Container.Bind<WS_AllCell>().FromInstance(_allCell).AsSingle();
        Container.Bind<WS_SpawnUnitOnCell>().FromInstance(_spawnUnitOnCell).AsSingle();
        //Container.Bind<AllPlayer>().FromInstance(_player).AsSingle();
        //Container.Bind<WS_PlayerManager>().FromInstance(_playerManager).AsSingle();
        Container.Bind<WS_SoundsUnit>().FromInstance(_soundsUnitManager).AsSingle();
        Container.Bind<BoomEffectController>().FromInstance(_explosionController).AsSingle().NonLazy();
    }
}

public enum StatusGameSignal
{
    Return = 0,
    SelectUnit = 1,
    SelectCell = 2,
    SelectAttack = 3,
}
