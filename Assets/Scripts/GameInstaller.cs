using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;


public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Player ������ ���� �� ����� �������
        Container.Bind<Player>()
            .FromComponentInHierarchy() // ���� ������������ ������
            .AsSingle();
            //.NonLazy(); // ������ �����, � �� ��� ������ �������
        // ������� Zenject: "����� ���-�� �������� Inventory, ������ ����� ���������"
        Container.Bind<Inventory>().AsSingle();
    }
}
