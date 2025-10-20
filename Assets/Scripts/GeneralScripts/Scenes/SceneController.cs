using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitOfMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void EnterPlayGame(int index)
    {
        //SceneManager.LoadScene(1);
        StartCoroutine(LoadAsync(index, 1));
    }

    private IEnumerator LoadAsync(int sceneIndex, float timeOut)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false; // Запрещаем авто-переход

        yield return new WaitForSeconds(timeOut);

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true; // Разрешаем переход
            }

            yield return null;
        }
    }

    public Scene GetCurrentScene()
    {
       return SceneManager.GetActiveScene();
    }
}
