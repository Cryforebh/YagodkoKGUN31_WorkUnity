using UnityEngine;
using Zenject;

public class Bag : MonoBehaviour
{
    [Inject]
    public Player Player { get; private set; } // �������� � ������� �����

    void Start()
    {
        // Rigidbody �������������� ���� ��-�� [RequireComponent]
        Player.GetComponent<Rigidbody>().isKinematic = true;
        Player.GetComponent<Player>().Speed = 20;
        Player.GetComponent<Player>().GetCharacterLog();
        Debug.Log("�� ��������!");
    }
}
