using Zenject;

public class SoundInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SoundManager>()
            .FromNewComponentOnNewGameObject() // Создаем новый GameObject
            .AsSingle()                        // Единственный экземпляр
            .NonLazy();                        // Создаем сразу, а не при первом обращении
    }
}