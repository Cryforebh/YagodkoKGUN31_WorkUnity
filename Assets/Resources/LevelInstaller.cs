using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    //[SerializeField] private SceneController _sceneController;
    //[SerializeField] private InputManager _inputManager;
    [SerializeField] private AdvancedCursorController _advancedCursorController;

    public override void InstallBindings()
    {
        Container.Bind<AdvancedCursorController>().FromInstance(_advancedCursorController).AsSingle();
        Container.Bind<SceneController>().AsSingle().NonLazy();
        Container.Bind<InputManager>().AsSingle().NonLazy();

        Debug.Log("Привязано!");
    }
}