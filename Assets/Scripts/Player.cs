using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Inject]
    public Inventory Inventory { get; private set; }

    [SerializeField]
    public int Speed = 10;
    public int Strength = 3;
    public int Intelect = 2;

    void Start()
    {
        if (Inventory == null)
        {
            Debug.LogError("Inventory �� ��������!");
            return;
        }
        Debug.Log($"� ������ ���� ���������: {Inventory.Name}");
    }

    public override string ToString()
    {
        return $"" +
            $"�������� = {Speed}\n" +
            $"���� = {Strength}\n" +
            $"�������� = {Intelect}";
    }

    public void GetCharacterLog()
    {
        Debug.Log($"" +
            $"�������� = {Speed}\n" +
            $"���� = {Strength}\n" +
            $"�������� = {Intelect}");
    }
}
