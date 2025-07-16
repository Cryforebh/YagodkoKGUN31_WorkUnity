using UnityEngine;
using Zenject;

public class Bag : MonoBehaviour
{
    [Inject]
    public Player Player { get; private set; } // Свойство с большой буквы

    void Start()
    {
        // Rigidbody гарантированно есть из-за [RequireComponent]
        Player.GetComponent<Rigidbody>().isKinematic = true;
        Player.GetComponent<Player>().Speed = 20;
        Player.GetComponent<Player>().GetCharacterLog();
        Debug.Log("Всё работает!");
    }
}
