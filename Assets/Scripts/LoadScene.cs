using UnityEngine;
using Zenject;

public class LoadScene : MonoBehaviour
{
    [Inject]
    private SceneController controller;
    private int index = 0;

    public void Load()
    {
        switch (index)
        {
            case 0:
                controller.OpenGameScene();
                index = 1;
                break;
            case 1:
                controller.OpenMainScene();
                index = 0;
                break;
            default:
                Debug.Log("Индекс сцены не обозначен. Что-то пошло не так!");
                break;
        }
    }
}
