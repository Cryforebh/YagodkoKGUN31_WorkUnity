using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SelectionInstaller : MonoInstaller
{
    [SerializeField]
    private SelectionManager _selectionPointerColors;

    public override void InstallBindings()
    {
        Container.Bind<SelectionManager>().FromInstance(_selectionPointerColors).AsSingle();
    }
}
