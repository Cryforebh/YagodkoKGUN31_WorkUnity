using Zenject;

public class LocalizationInstaller : MonoInstaller
{

    public override void InstallBindings()
    {
        Container.Bind<LocalizationManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
}
