using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void OpenMainScene()
    {
        // Выгружаем все сцены кроме основной (если нужно)
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void OpenGameScene(int index)
    {
        if (index == 0) OpenMainScene();

        // Аддитивно загружаем игровую сцену
        SceneManager.LoadScene(index, LoadSceneMode.Additive);
    }
}
