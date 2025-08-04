using UnityEngine;
using Zenject;

public class LoadScene : MonoBehaviour
{
    [Inject]
    private SceneController controller;
    [SerializeField]
    private int index = 0;

    public void Load()
    {
        controller.OpenGameScene(index);
    }
}
