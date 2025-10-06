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
    private StatisticsUnitsVisualManager _healthBarManager;
    [SerializeField]
    private AdvancedCursorController _cursor;
    [SerializeField]
    private SoundsUnit _soundsUnitManager;
    [SerializeField]
    private BoomEffectController _explosionController;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<StatusGameSignal>();

        Container.Bind<AdvancedCursorController>().FromInstance(_cursor).AsSingle().NonLazy();
        Container.Bind<StatisticsUnitsVisualManager>().FromInstance(_healthBarManager).AsSingle().NonLazy();
        Container.Bind<SelectionMaterialManager>().FromInstance(_selectionPointerColors).AsSingle().NonLazy();
        Container.Bind<AllCell>().FromInstance(_allCell).AsSingle();
        Container.Bind<SpawnUnitOnCell>().FromInstance(_spawnUnitOnCell).AsSingle();
        Container.Bind<AllPlayer>().FromInstance(_player).AsSingle();
        Container.Bind<PlayerManager>().FromInstance(_playerManager).AsSingle();
        Container.Bind<SoundsUnit>().FromInstance(_soundsUnitManager).AsSingle();
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
