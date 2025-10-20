using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private AdvancedCursorController _cursor;
    [SerializeField] private SceneController _sceneController;
    [SerializeField] private InputManager _inputManager;

    public override void InstallBindings()
    {
        Container.Bind<AdvancedCursorController>().FromInstance(_cursor).AsSingle().NonLazy();
        Container.Bind<SceneController>().FromInstance(_sceneController).AsSingle().NonLazy();
        Container.Bind<InputManager>().FromInstance(_inputManager).AsSingle().NonLazy();

        //if (_sceneController.GetCurrentScene() == SceneManager.GetSceneByBuildIndex(3))
        //{
        //    Container.Bind<ISpawner>().To<C_Spawner>().FromNewComponentOnNewGameObject().AsSingle();
        //}
    }

}
